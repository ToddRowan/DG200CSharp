using System;
using kimandtodd.DG200CSharp.commandresults;

namespace kimandtodd.DG200CSharp.commands
{
    public class BaseCommand : ICommandData
    {
        // The byte header for every command.
        private static byte[] commandHeader = new byte[] { 0xA0, 0xA2 };
        // The byte footer for every command.
        private static byte[] commandFooter = new byte[] { 0xB0, 0xB3 };
        // Lengths of those command brackets. 
        private static int HEADERLENGTH = commandHeader.Length;
        private static int FOOTERLENGTH = commandFooter.Length;

        protected CommandBuffer _buf;
        private bool _commandHeaderDataEvaluated;
        private bool _sizeDataEvaluated;

        protected int _expectedByteCount;

        /// <summary>
        /// Constructor. Protected because we don't want anyone creating a base class instance.
        /// </summary>
        protected BaseCommand()
        {
            this._buf = new CommandBuffer();
            this._commandHeaderDataEvaluated = false;
            this._sizeDataEvaluated = false;
            this._expectedByteCount = -1; // We don't know yet.
        }

        /// <summary>
        /// Generates the command header for building the complete comamnd.
        /// </summary>
        /// <returns>The array of bytes in the header.</returns>
        protected byte[] getCommandHeader()
        {
            return BaseCommand.commandHeader;
        }

        /// <summary>
        /// Generates the command footer for building the complete comamnd.
        /// </summary>
        /// <returns>The array of bytes in the footer.</returns>
        protected byte[] getCommandFooter()
        {
            return BaseCommand.commandFooter;
        }

        /// <summary>
        /// Builds the complete input array for sending a command to the device. 
        /// </summary>
        /// <param name="command">The array of command-identifying bytes to be sandwiched within the header and footer.</param>
        /// <returns>The completed command array.</returns>
        protected byte[] buildCommandArray(byte[] command)
        {
            byte[] payloadLength = getPayloadLengthArray(command);
            byte[] _full = new byte[BaseCommand.HEADERLENGTH + payloadLength.Length + command.Length + DG200CheckSumCalculator.CHECKSUMLENGTH + BaseCommand.FOOTERLENGTH];
            this.getCommandHeader().CopyTo(_full, 0);
            payloadLength.CopyTo(_full, BaseCommand.HEADERLENGTH);
            command.CopyTo(_full, BaseCommand.HEADERLENGTH + payloadLength.Length);
            DG200CheckSumCalculator.CalculateCheckSum(command).CopyTo(_full, BaseCommand.HEADERLENGTH + payloadLength.Length + command.Length);
            this.getCommandFooter().CopyTo(_full, BaseCommand.HEADERLENGTH + payloadLength.Length + command.Length + DG200CheckSumCalculator.CHECKSUMLENGTH);
            return _full;
        }

        /// <summary>
        /// The overridable method that must be implemented by subcommands to return the command-identifying array.
        /// </summary>
        /// <returns>At this level it just returns an empty array.</returns>
        public virtual byte[] getCommandData()
        {
            return new byte[] { };
        }

        /// <summary>
        /// The shared method that returns the completed result of the command.
        /// </summary>
        /// <returns>The command result object.</returns>
        public virtual BaseCommandResult getLastResult()
        {
            return null;
        }

        /// <summary>
        /// Figures out if the byte array matches the command header. 
        /// </summary>
        /// <param name="bytes">The array of bytes to evaluate.</param>
        /// <returns>True if the arrays match, false otherwise.</returns>
        private bool isCommandHeader(byte[] bytes)
        {
            for (int inx = 0; inx < BaseCommand.commandHeader.Length; inx++)
            {
                if (bytes[inx] != BaseCommand.commandHeader[inx])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Evaluates the stream as it comes in. Initially used to determine if the array starts as expected,
        /// but also calculates the expected payload size for the entire stream. Either continues on merrily,
        /// or throws exceptions if the expectation is the data can't be used. 
        /// </summary>
        private void evaluateData()
        {
            // If we don't even have two bytes, we can't get started. 
            if (this._buf.Length < 2)
            {
                return;
            }

            if (!this._commandHeaderDataEvaluated)
            {
                this._commandHeaderDataEvaluated = true;

                byte[] header = new byte[2];
                this._buf.Position = 0;
                int count = this._buf.Read(header, 0, 2);
                this._buf.Position = this._buf.Length;
                if (!this.isCommandHeader(header))
                {
                    throw new kimandtodd.DG200CSharp.commands.exceptions.CommandException("The connection attempt failed. Either the device is not a DG200 or it is a DG200 that is not turned on.");
                }
            }
            // Once we get enough data in the buffer, evaluate the payload size. 
            if (!this._sizeDataEvaluated && this._buf.Length > 3 )
            {
                this._sizeDataEvaluated = true;
                byte[] sizeArr = new byte[2];
                this._buf.Position = 2;
                this._buf.Read(sizeArr, 0, 2);
                this._buf.Position = this._buf.Length;
                this._expectedByteCount = this.calculateExpectedBytes(sizeArr);
                this.overrideExpectedByteCount();
            }
        }

        /// <summary>
        /// Allows descendant classes to override the expected byte count, 
        /// as some command results lie about their payload sizes. 
        /// </summary>
        protected void overrideExpectedByteCount()
        {
            // No-op here. 
        }

        /// <summary>
        /// Add command binary data to the interal buffer. 
        /// </summary>
        /// <param name="bytes">The array of bytes to store.</param>
        public void addCommandResultData(byte[] bytes, Int32 byteCount)
        {
            // Copy the incoming stream to our local store.
            this._buf.Write(bytes, 0, byteCount);
            // Read the bytes to throw any errors and measure progress.
            this.evaluateData();
        }

        /// <summary>
        /// Calculates the length of the input payload. This tells the device how much data to expect.
        /// </summary>
        /// <param name="payload">The complete input payload array.</param>
        /// <returns>A two-byte array that contains the payload length value.</returns>
        private byte[] getPayloadLengthArray(byte[] payload)
        {
            byte[] payloadLength = new byte[2];

            byte[] convertedLength = BitConverter.GetBytes(payload.Length);

            payloadLength[1] = convertedLength[0];
            payloadLength[0] = convertedLength[1];

            return payloadLength;
        }

        /// <summary>
        /// One of the DG200 commands returns two complete payloads, one after the other.
        /// This method allows that command to notify the serial processor to keep reading 
        /// from the buffer after the first session completes. 
        /// </summary>
        /// <returns>One if no override is necessary. Otherwise returns the true session count.</returns>
        public int getExpectedSessionCount()
        {
            return 1;
        }

        /// <summary>
        /// Asks the command it if has received enough data.
        /// </summary>
        /// <returns>True if keep going, false otherwise.</returns>
        public bool continueReading()
        {
            // If we haven't even seen the payload size data, say yes.
            if (!this._sizeDataEvaluated)
            {
                return true;
            }
            else
            {
                return this._buf.Length < this._expectedByteCount;
            }
        }

        /// <summary>
        /// Converts the two-byte, big-endian, payload size value into an x64 little-endian integer. 
        /// </summary>
        /// <param name="payloadSize">The two byte array with the payload value in it.</param>
        /// <returns>The integer of the payload size. Includes eight bytes of padding included in every message.</returns>
        private int calculateExpectedBytes(byte[] payloadSize)
        {
            int total = 0;

            total = payloadSize[0] << 8;
            total += payloadSize[1] & 255;

            // Add eight bytes for the following (2 bytes each):  start and end sequences, the payload length, the checksum
            total += 8;

            return total;
        }
    }
}
