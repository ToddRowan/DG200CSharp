using System;
using System.Collections.Generic;

using kimandtodd.DG200CSharp.commandresults;
using kimandtodd.DG200CSharp.commands.exceptions;
using kimandtodd.DG200CSharp.logging;
using kimandtodd.DG200CSharp.sessions;

namespace kimandtodd.DG200CSharp.commands
{
    public class BaseCommand : ICommandData
    {
        // The byte header for every command.
        public static byte[] commandHeader = new byte[] { 0xA0, 0xA2 };
        // The byte footer for every command.
        public static byte[] commandFooter = new byte[] { 0xB0, 0xB3 };
        // Lengths of those command brackets. 
        public static int HEADERLENGTH = commandHeader.Length;
        public static int FOOTERLENGTH = commandFooter.Length;

        protected BaseSession _session;

        protected BaseCommandResult _currentResult;
        private DG200SerialConnection _serialConnection;

        /// <summary>
        /// Constructor. Protected because we don't want anyone creating a base class instance.
        /// </summary>
        protected BaseCommand()
        {
            DG200FileLogger.Log("BaseCommand constructor.", 3);
            this._session = null;
            this._currentResult = null;
            this._serialConnection = null;
        }

        /// <summary>
        /// Initializes the command. Since commands can be reused, this method resets the command for reuse. 
        /// </summary>
        public void initialize()
        {
            DG200FileLogger.Log("BaseCommand initialize.", 3);                     
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
                this._serialConnection.Execute();
            } 
            while(this.doAnotherSession());
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
        /// Add command binary data to the interal buffer. 
        /// </summary>
        /// <param name="bytes">The array of bytes to store.</param>
        public void addCommandResultData(byte[] bytes, Int32 byteCount)
        {
            this.writeToCurrentSession(bytes, byteCount);
        }

        /// <summary>
        /// Write the incoming data to the current session.
        /// </summary>
        /// <param name="bytes">The array of bytes to save.</param>
        /// <param name="byteCount">The number of bytes to read.</param>
        private void writeToCurrentSession(byte[] bytes, Int32 byteCount)
        {
            // Copy the incoming stream to our local store.
            this._session.Write(bytes, byteCount);
        }        

        /// <summary>
        /// Asks the session it if has received enough data.
        /// </summary>
        /// <returns>True if keep going, false otherwise.</returns>
        public virtual bool continueReading()
        {
            return this._session.continueReading();
        }

        /// <summary>
        /// Asks the session if we should make another request to the DG200.
        /// </summary>
        /// <returns>True if keep going, false otherwise.</returns>
        public virtual bool doAnotherSession()
        {
            // Except for one case, we always do one session.
            return false;
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
    }
}
