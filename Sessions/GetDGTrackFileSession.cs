using System;
using kimandtodd.DG200CSharp.logging;

namespace kimandtodd.DG200CSharp.sessions
{
    class GetDGTrackFileSession : BaseSession
    {
        private bool _movedSessionPointer;

        private static int FIRST_PAYLOAD_END = 1029;
        private static int SECOND_SESSION_PAYLOAD_START = 1038;
        private static int SPACE_BETWEEN_PAYLOADS = GetDGTrackFileSession.SECOND_SESSION_PAYLOAD_START - GetDGTrackFileSession.FIRST_PAYLOAD_END;
        private static int ACTUAL_PAYLOAD_BYTE_COUNT = 2057;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GetDGTrackFileSession() : base()
        {
            this._movedSessionPointer = false;
        }
        
        /// <summary>
        /// Override the save-to-buffer method. Because we do sneaky math before passing to the result.
        /// </summary>
        /// <param name="bytes">The array of incoming bytes</param>
        /// <param name="byteCount">The number of useful bytes in the incoming array (not all of the array is always useful)</param>
        public override void Write(byte[] bytes, Int32 byteCount)
        {
            int bytesCopiedFromFirstSession = 0,
                newInputOffset = 0;
            
            // First, do we have enough bytes to start copying the second payload? 
            // We only want to do this once. 
            if (!this._movedSessionPointer && this.getCurrentBuffer().Length + byteCount >= GetDGTrackFileSession.SECOND_SESSION_PAYLOAD_START)
            {
                DG200FileLogger.Log("GetDGTrackFileSession evaluating data merge criteria.", 2);
                // If so, see if we need to copy any remaining data from the first session.
                if (this.getCurrentBuffer().Length < GetDGTrackFileSession.FIRST_PAYLOAD_END)
                {
                    DG200FileLogger.Log("GetDGTrackFileSession: First payload not saved, doing that now.", 2);
                    // Do the math and write with a custom offset. Casting, ugh.
                    bytesCopiedFromFirstSession = unchecked((int)(GetDGTrackFileSession.FIRST_PAYLOAD_END - this.getCurrentBuffer().Length));

                    this.getCurrentBuffer().Write(bytes, 0, bytesCopiedFromFirstSession);

                    // The new starting offset for the second read.
                    newInputOffset = bytesCopiedFromFirstSession + GetDGTrackFileSession.SPACE_BETWEEN_PAYLOADS;
                }
                else
                {
                    DG200FileLogger.Log("GetDGTrackFileSession: First payload saved, changing buffer pointer and copying.", 2);
                    // Calculate how many extraneous bytes were copied into the buffer on the last pass.
                    bytesCopiedFromFirstSession = unchecked((int)(this.getCurrentBuffer().Length - GetDGTrackFileSession.FIRST_PAYLOAD_END));
                    newInputOffset = GetDGTrackFileSession.SPACE_BETWEEN_PAYLOADS - bytesCopiedFromFirstSession;
 
                    // Move the pointer to the end of the first buffer
                    this.getCurrentBuffer().Position = GetDGTrackFileSession.FIRST_PAYLOAD_END;
                }

                DG200FileLogger.Log("GetDGTrackFileSession: Writing with modified buffer.", 2);
                this.getCurrentBuffer().Write(bytes, newInputOffset, byteCount - newInputOffset);

                this._movedSessionPointer = true;
            }
            else
            {
                DG200FileLogger.Log("GetDGTrackFileSession doing regular write.", 2);
                this.getCurrentBuffer().Write(bytes, 0, byteCount);
            }            

            // Figure out what we have. 
            this.evaluateData();
        }

        /// <summary>
        /// We override the byte count since the DG200 sends two sessions in response to one command request.
        /// </summary>
        protected override void overrideExpectedByteCount()
        {
            // The track file command sends two sessions at once.  
            this._expectedByteCount = GetDGTrackFileSession.ACTUAL_PAYLOAD_BYTE_COUNT;
            DG200FileLogger.Log("GetDGTrackFileSession overriding expected byte count: " + this._expectedByteCount, 2);
        }
    }
}
