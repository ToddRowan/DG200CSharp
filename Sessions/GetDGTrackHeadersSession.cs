using System;

using kimandtodd.DG200CSharp.commandresults;
using kimandtodd.DG200CSharp.commands;
using kimandtodd.DG200CSharp.logging;
using kimandtodd.DG200CSharp.commands.exceptions;

namespace kimandtodd.DG200CSharp.sessions
{
    class GetDGTrackHeadersSession : BaseSession
    {
        private ITrackHeaderResult _localRes;

        public GetDGTrackHeadersSession() : base()
        {

        }

        /// <summary>
        /// Should we execute another call to the device? For this command we do it at least twice.
        /// </summary>
        /// <returns>True if yes, false if no.</returns>
        public override bool callAgain()
        {
            bool again = this._localRes.requestAdditionalSession();
            if (again)
            {
                this._buffers.Add(new CommandBuffer());
                this.reset();
            }
            return again;
        }

        public override void setResult(ICommandResult newRes)
        {
            this._currentResult = newRes;
            // This is ugly, but I'm out of ideas.
            this._localRes = (ITrackHeaderResult)newRes;
        }
    }
}
