using System;
using System.Collections.Generic;

using kimandtodd.DG200CSharp.commands;
using kimandtodd.DG200CSharp.commandresults.resultitems;

namespace kimandtodd.DG200CSharp.commandresults
{
    public class GetDGTrackHeadersCommandResult : BaseCommandResult
    {
        // The number of headers the device reports.
        private int _headerCount;
        // The next available track
        private int _nextTrackId;
        // The array of headers
        private List<DGTrackHeader> _trackHeaders;

        private bool _startSession;

        // A header block in the buffer is 12 bytes.
        private static int HEADERSIZE = 12;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultBuf">The buffer with the result of the command.</param>
        public GetDGTrackHeadersCommandResult(CommandBuffer resultBuf)
            : base(resultBuf)
        {
            this._headerCount = 0;
            this._trackHeaders = new List<DGTrackHeader>();
            this._startSession = true;

            this.init();
        }

        /// <summary>
        /// Read the buffer and fill in our local variables.
        /// </summary>
        private void init()
        {
            this.processBuffer();
        }

        protected override void processBuffer()
        {
            this.getCurrentBuffer().Position = BaseCommandResult.PAYLOAD_START;
            byte[] tmpArr = new byte[2];

            this.getCurrentBuffer().Read(tmpArr, 0, tmpArr.Length);
            int sessionHeaderCount = bigEndianArrayToInt16(tmpArr);

            // If the session said no track headers remain, don't parse and set the start val to false.
            if (sessionHeaderCount != 0)
            {
                this._headerCount += sessionHeaderCount;
                this.getCurrentBuffer().Read(tmpArr, 0, tmpArr.Length);
                _nextTrackId = bigEndianArrayToInt16(tmpArr);

                for (int inx = 0; inx < _headerCount; inx++)
                {
                    byte[] headerBuf = new byte[GetDGTrackHeadersCommandResult.HEADERSIZE];

                    this.getCurrentBuffer().Read(headerBuf, 0, headerBuf.Length);

                    this._trackHeaders.Add(new DGTrackHeader(headerBuf));
                }
            }
            else
            {
                this._startSession = false;
            }
        }

        private int bigEndianArrayToInt16(byte[] arr)
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

        /// <summary>
        /// Whether or not to order a second session.
        /// </summary>
        /// <returns>True if yes, false otherwise.</returns>
        public override bool startSession()
        {
            return this._startSession;
        }
    }
}
