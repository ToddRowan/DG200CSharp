using System;
using System.Collections.Generic;

using kimandtodd.DG200CSharp.commands;
using kimandtodd.DG200CSharp.commandresults.resultitems;


namespace kimandtodd.DG200CSharp.commandresults
{
    public class GetDGTrackFileCommandResult : BaseCommandResult
    {

        private int _trackCount;
        private UInt16 _trackStyle;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultBuf">The buffer with the result of the command.</param>
        public GetDGTrackFileCommandResult(CommandBuffer resultBuf) : base(resultBuf)
        {
            this._trackCount = 0;

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
            // we're going to want a way to just push into the original buffer. 
            // the spec says we should merge before reading. But the first buffer 
            // will also end with data we don't want (4 bytes of checksum and command end).
            // We also need to be ready only if we have two buffers. If there aren't many tracks in the file.
            // We need to know what we're getting if there's only one. 
            if (this.getBufferCount() == 1)
            {
                Console.WriteLine("Stored first buffer. It contains this many bytes: " + this._buffers[0].Length);

                // If this is just the first session buffer, set the additional session request to true,
                // but don't do anything else. Just return.                 
                this._additionalSession = true;

                return;
            }
            else
            {
                Console.WriteLine("Got the second buffer. It contains this many bytes: " + this._buffers[1].Length);
                // We only want two sessions here. So set that flag to not request another. 
                this._additionalSession = false;
                // merge in the second buffer, if it has any values in it. 
                this._buffers[0].Position = this._buffers[0].Length - 4;
                // Copy in anything past the starting point.
                this._buffers[1].Position = BaseCommandResult.PAYLOAD_START;
                this._buffers[1].CopyTo(this._buffers[0]);
                Console.WriteLine("Copy is complete. Length of first buffer is now: " + this._buffers[1].Length);
            }
        }

        protected override CommandBuffer getCurrentBuffer()
        {
            return this._buffers[0];
        }
    }
}
