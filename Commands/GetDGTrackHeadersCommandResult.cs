using System;

using kimandtodd.DG200CSharp.commands;

namespace kimandtodd.DG200CSharp.commandresults
{
    public class GetDGTrackHeadersCommandResult : BaseCommandResult
    {
        // The number of headers the device reports.
        private int _headerCount;
        // The next available track
        private int _nextTrackId;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultBuf">The buffer with the result of the command.</param>
        public GetDGTrackHeadersCommandResult(CommandBuffer resultBuf)
            : base(resultBuf)
        {
            this.init();
        }

        /// <summary>
        /// Read the buffer and fill in our local variables.
        /// </summary>
        private void init()
        {
            this._buf.Position = BaseCommandResult.PAYLOAD_START;
            byte[] tmpArr = new byte[2];

            this._buf.Read(tmpArr, 0, tmpArr.Length);
            _headerCount = bigEndianArrayToInt(tmpArr);

            this._buf.Read(tmpArr, 0, tmpArr.Length);
            _nextTrackId = bigEndianArrayToInt(tmpArr);
        }

        private int bigEndianArrayToInt(byte[] arr)
        {
            byte[] tmpArr = new byte[2];
            tmpArr[0] = arr[1];
            tmpArr[1] = arr[0];
            return BitConverter.ToInt16(tmpArr, 0);
        }

        public int getHeaderCount()
        {
            return this._headerCount;
        }

        public int getNextTrackId()
        {
            return this._nextTrackId;
        }
    }
}
