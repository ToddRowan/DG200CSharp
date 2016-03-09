using System;
using System.Collections.Generic;

using kimandtodd.DG200CSharp.commandresults;
using kimandtodd.DG200CSharp.commands.exceptions;
using kimandtodd.DG200CSharp.logging;

namespace kimandtodd.DG200CSharp.commands
{
    public class BaseCommand : ICommandData
    {
        
        private bool _commandHeaderDataEvaluated;
        private bool _sizeDataEvaluated;
        protected bool _requestAdditionalSession;

        protected int _expectedByteCount;

        protected BaseCommandResult _currentResult;
        private DG200SerialConnection _serialConnection;

        /// <summary>
        /// Constructor. Protected because we don't want anyone creating a base class instance.
        /// </summary>
        protected BaseCommand()
        {
            DG200FileLogger.Log("BaseCommand constructor.", 3);
            this._buf = new CommandBuffer();
            this._currentResult = null;
            this._serialConnection = null;
            this._requestAdditionalSession = false;
            //this.initialize();
        }

        /// <summary>
        /// Initializes the command. Since commands can be reused, this method resets the command for reuse. 
        /// </summary>
        public void initialize()
        {
            DG200FileLogger.Log("BaseCommand initialize.", 3);
            // this resets the memory stream.
            this._buf.SetLength(0);

            this._commandHeaderDataEvaluated = false;
            this._sizeDataEvaluated = false;
            this._expectedByteCount = -1; // We don't know yet.            
        }

        /// <summary>
        /// Set the serial connection for the command.
        /// </summary>
        /// <param name="sc">The new serial connection.</param>
        public void setSerialConnection (DG200SerialConnection sc)
        {
            this._serialConnection = sc;
            this._serialConnection.setCommand(this);
        }

        /// <summary>
        /// Retrieves the current serial connection, if another command wants to reuse it. 
        /// </summary>
        /// <returns>The current connection.</returns>
        public DG200SerialConnection getSerialConnection()
        {
            return this._serialConnection;
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
        /// Run the command. 
        /// </summary>
        public void execute()
        {
            DG200FileLogger.Log("BaseCommand execute method.", 3);
            do
            {
                this.initialize();
                this._serialConnection.Execute();
            } 
            while(this._currentResult != null && this._currentResult.requestAdditionalSession());
            
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
        public BaseCommandResult getLastResult()
        {
            return this._currentResult;
        }

        /// <summary>
        /// Ask the command if it can continue. For all but two commands, this method is enough.
        /// </summary>
        /// <returns>True if yes, false otherwise.</returns>
        public virtual bool startSession()
        {
            return false; // this._sessionCounter == 0;
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
        protected virtual void evaluateData()
        {
            // If we don't even have two bytes, we can't get started. 
            if (this._buf.Length < 2)
            {
                return;
            }
            // Evaluate the payload as enough bytes come in. 
            this.evaluateCommandHeaderData();
            this.evaluatePayloadSizeData();            
        }

        /// <summary>
        /// Read the command header and make sure that it is something we are interested in.
        /// </summary>
        protected void evaluateCommandHeaderData()
        {
            if (!this._commandHeaderDataEvaluated)
            {
                this._commandHeaderDataEvaluated = true;

                byte[] header = new byte[2];
                this._buf.Position = 0;
                int count = this._buf.Read(header, 0, 2);
                this._buf.Position = this._buf.Length;
                if (!this.isCommandHeader(header))
                {
                    throw new CommandException("The connection attempt failed. Either the device is not a DG200 or it is a DG200 that is not turned on.");
                }
            }
        }

        /// <summary>
        /// Read the payload size data and store the result internally.
        /// </summary>
        protected void evaluatePayloadSizeData()
        {
            // Once we get enough data in the buffer, evaluate the payload size. 
            if (!this._sizeDataEvaluated && this._buf.Length > 3)
            {
                this._sizeDataEvaluated = true;
                byte[] sizeArr = new byte[2];
                this._buf.Position = 2;
                this._buf.Read(sizeArr, 0, 2);
                this._buf.Position = this._buf.Length;
                this._expectedByteCount = this.calculateExpectedBytes(sizeArr);
                DG200FileLogger.Log("BaseCommand calculating payload size: " + this._expectedByteCount, 3);
                this.overrideExpectedByteCount();
            }
        }

        /// <summary>
        /// Allows descendant classes to override the expected byte count, 
        /// as some commands lie about their payload sizes. 
        /// </summary>
        protected virtual void overrideExpectedByteCount()
        {
            // No-op here. 
        }

        /// <summary>
        /// Add command binary data to the interal buffer. 
        /// </summary>
        /// <param name="bytes">The array of bytes to store.</param>
        public void addCommandResultData(byte[] bytes, Int32 byteCount)
        {
            this.writeToCurrentBuffer(bytes, byteCount);
            // Read the bytes to throw any errors and measure progress.
            this.evaluateData();
        }

        private void writeToCurrentBuffer(byte[] bytes, Int32 byteCount)
        {
            // Copy the incoming stream to our local store.
            this._buf.Write(bytes, 0, byteCount);
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
        /// Asks the command it if has received enough data.
        /// </summary>
        /// <returns>True if keep going, false otherwise.</returns>
        public virtual bool continueReading()
        {            
            // If we haven't even seen the payload size data, say yes.
            if (!this._sizeDataEvaluated)
            {
                DG200FileLogger.Log("BaseCommand has not received enough bytes to evaluate size.", 3);
                return true;
            }
            else
            {
                DG200FileLogger.Log("BaseCommand comparing bytes read (" + this._buf.Length + ") to expected (" + this._expectedByteCount + ").", 3);
                bool _continue = this._buf.Length < this._expectedByteCount;
                if (!_continue)
                {
                    DG200FileLogger.Log("BaseCommand has read expected amount data and is starting the result processing.", 3);
                    // Initialize the result and save the value on requesting an additional session.
                    this.processResult();
                }
                
                return _continue;
            }
        }

        /// <summary>
        /// Must be overidden. 
        /// </summary>
        /// <returns>True if the command should initiate another session. False otherwise.</returns>
        protected virtual void processResult()
        {

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
