using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using kimandtodd.DG200CSharp.commands;

namespace kimandtodd.DG200CSharp.commandresults
{
    public class SetDGIDCommandResult : BaseCommandResult
    {
        /// <summary>
        /// The array that stores the retrieved data.
        /// I have no idea what to expect on a return value. 
        /// </summary>
        private byte[] _retrievedData;
        /// <summary>
        /// The expected length of the retrievable ID.
        /// </summary>
        private static int RETRIEVED_BYTE_LENGTH = 4;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultBuf">The buffer with the result of the command.</param>
        public SetDGIDCommandResult(CommandBuffer resultBuf) : base(resultBuf)
        {
            this.init();
        }

        /// <summary>
        /// Read the buffer and fill in our local variables.
        /// </summary>
        private void init()
        {
            this._retrievedData = new byte[SetDGIDCommandResult.RETRIEVED_BYTE_LENGTH];

            if (this.getCurrentBuffer().Length > 9)
            {
                this.getCurrentBuffer().Position = BaseCommandResult.PAYLOAD_START;
                this.getCurrentBuffer().Read(this._retrievedData, 0, SetDGIDCommandResult.RETRIEVED_BYTE_LENGTH);
            }
        }

        public byte[] getRetrievedData()
        {
            return this._retrievedData;
        }
    }
}
