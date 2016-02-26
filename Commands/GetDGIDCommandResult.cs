using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using kimandtodd.DG200CSharp.commands;

namespace kimandtodd.DG200CSharp.commandresults
{
    public class GetDGIDCommandResult : BaseCommandResult
    {
        /// <summary>
        /// The array that stores the retrieved ID.
        /// </summary>
        private byte[] _retrievedId;
        /// <summary>
        /// The expected length of the retrievable ID.
        /// </summary>
        private static int ID_BYTE_LENGTH = 8;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultBuf">The buffer with the result of the command.</param>
        public GetDGIDCommandResult(CommandBuffer resultBuf) : base(resultBuf)
        {
            this.init();
        }

        /// <summary>
        /// Read the buffer and fill in our local variables.
        /// </summary>
        private void init()
        {
            this._retrievedId = new byte[GetDGIDCommandResult.ID_BYTE_LENGTH];

            if (this._buf.Length > 12)
            {
                this._buf.Position = BaseCommandResult.PAYLOAD_START;
                this._buf.Read(this._retrievedId, 0, GetDGIDCommandResult.ID_BYTE_LENGTH);
            }
        }

        /// <summary>
        /// Publicly accessible method to retrieve the set ID.
        /// </summary>
        /// <returns>An 8-byte array with the ID.</returns>
        public byte[] getID()
        {
            return this._retrievedId;
        }
    }
}
