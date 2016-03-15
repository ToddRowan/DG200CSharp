using System;

using kimandtodd.DG200CSharp.commands;
using kimandtodd.DG200CSharp.logging;
using kimandtodd.DG200CSharp.commands.exceptions;

namespace kimandtodd.DG200CSharp.sessions
{
    class SetDGIDSession : BaseSession
    {
        public SetDGIDSession() : base()
        {

        }

        /// <summary>
        /// This command requires a manual override on the data size value b/c it comes back wrong. 
        /// </summary>
        protected override void overrideExpectedByteCount()
        {
            this._expectedByteCount += 4;
            DG200FileLogger.Log("SetDGIDSession overriding expected byte count: " + this._expectedByteCount, 2);
        }
    }
}
