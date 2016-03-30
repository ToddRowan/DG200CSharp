using System;

using kimandtodd.DG200CSharp.commands;

namespace kimandtodd.DG200CSharp.commandresults
{
    public class SetDGConfigurationCommandResult : BaseCommandResult
    {
        /// <summary>
        /// The array that stores the retrieved data.
        /// I have no idea what to expect on a return value. 
        /// </summary>
        private byte[] _retrievedData;

        private int _success;
        /// <summary>
        /// The expected length of the retrievable ID.
        /// </summary>
        private static int RETRIEVED_BYTE_LENGTH = 4;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SetDGConfigurationCommandResult() : base()
        {
            
        }

        /// <summary>
        /// Read the buffer and fill in our local variables.
        /// </summary>
        protected override void processBuffer()
        {
            this._retrievedData = new byte[SetDGConfigurationCommandResult.RETRIEVED_BYTE_LENGTH];

            this.getCurrentBuffer().Position = BaseCommandResult.PAYLOAD_START;
            this.getCurrentBuffer().Read(this._retrievedData, 0, SetDGConfigurationCommandResult.RETRIEVED_BYTE_LENGTH);

            this._success = DG200Utils.bigEndianArrayToInt32(this._retrievedData);
        }

        public bool getSuccess()
        {
            return this._success == 1;
        }        
    }
}