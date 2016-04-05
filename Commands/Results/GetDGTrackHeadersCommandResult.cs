using System;
using System.Collections.Generic;

using kimandtodd.DG200CSharp.commands;
using kimandtodd.DG200CSharp.commandresults;
using kimandtodd.DG200CSharp.commandresults.resultitems;
using kimandtodd.DG200CSharp.logging;

namespace kimandtodd.DG200CSharp.commandresults
{
    public class GetDGTrackHeadersCommandResult : BaseCommandResult, ITrackHeaderResult
    {
        // The number of headers the device reports.
        private int _headerCount;
        // The next available track
        private int _nextTrackId;
        // The array of headers
        private List<DGTrackHeader> _trackHeaders;
        // Whether or not we should run another session
        private bool _additionalSession;

        // A header block in the buffer is 12 bytes.
        private static int HEADERSIZE = 12;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultBuf">The buffer with the result of the command.</param>
        public GetDGTrackHeadersCommandResult() : base()
        {
            DG200FileLogger.Log("GetDGTrackHeadersCommandResult constructor.", 3);
            this._headerCount = 0;
            this._additionalSession = true; // We always do at least two. 
            this._trackHeaders = new List<DGTrackHeader>();
        }

        /// <summary>
        /// Process our local buffer.
        /// </summary>
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
                DG200FileLogger.Log("GetDGTrackHeadersCommandResult found " + sessionHeaderCount + " track headers this iteration. Total now: " + this._headerCount, 3);
                this.getCurrentBuffer().Read(tmpArr, 0, tmpArr.Length);
                this._nextTrackId = bigEndianArrayToInt16(tmpArr);
                this._additionalSession = true;

                for (int inx = 0; inx < _headerCount; inx++)
                {
                    byte[] headerBuf = new byte[GetDGTrackHeadersCommandResult.HEADERSIZE];

                    this.getCurrentBuffer().Read(headerBuf, 0, headerBuf.Length);

                    this._trackHeaders.Add(new DGTrackHeader(headerBuf));
                }
            }
            else
            {
                //this._nextTrackId = 0;
                DG200FileLogger.Log("GetDGTrackHeadersCommandResult no additional headers found.", 3);
                this._additionalSession = false;
            }
        }

        private int bigEndianArrayToInt16(byte[] arr)
        {
            byte[] tmpArr = new byte[2];
            tmpArr[0] = arr[1];
            tmpArr[1] = arr[0];
            return BitConverter.ToInt16(tmpArr, 0);
        }

        /// <summary>
        /// Get the current number of headers that have been retrieved. 
        /// </summary>
        /// <returns>The total number of headers retrieved in all of the recent sessions.</returns>
        public int getHeaderCount()
        {
            return this._headerCount;
        }

        /// <summary>
        /// Get the track ID to start the next session. Used by the command to initiate the session.
        /// </summary>
        /// <returns>The next trackID to retrieve. First track number is zero.</returns>
        public int getNextTrackHeaderId()
        {
            return this._nextTrackId;
        }

        /// <summary>
        /// Generate a list of track header start times, formatted in UTC.
        /// </summary>
        /// <returns>A newline-delimited list of track header start times.</returns>
        public override string ToString()
        {
            string headers = "";
            foreach (DGTrackHeader th in this._trackHeaders )
            {
                headers += th.ToString() + "\n";
            }

            return headers;
        }

        public bool requestAdditionalSession()
        {
            return this._additionalSession;
        }

        /// <summary>
        /// Generates the track headers from the internal buffer. 
        /// </summary>
        /// <returns>Yields track headers that are stored.</returns>
        public IEnumerable<DGTrackHeader> getTrackHeaders()
        {
            foreach(DGTrackHeader head in this._trackHeaders)
            {
                yield return head;
            }
        }
    }
}
