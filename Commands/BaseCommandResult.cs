using System;
using System.Collections.Generic;

using kimandtodd.DG200CSharp.commands;

namespace kimandtodd.DG200CSharp.commandresults
{
    public abstract class BaseCommandResult : ICommandResult
    {
        private bool _successState;
        private List<CommandBuffer> _buffers;

        protected static int COMMAND_LOCATION = 4;
        protected static int PAYLOAD_START = 5;

        protected BaseCommandResult(CommandBuffer resultBuf)
        {
            this._buffers = new List<CommandBuffer>();

            this.addResultBuffer(resultBuf);
            this._successState = false;
        }

        public bool getSuccess()
        {
            return this._successState;
        }

        public String getErrorMessage()
        {
            return "";
        }

        public virtual bool startSession()
        {
            return false;
        }

        public virtual void addResultBuffer(CommandBuffer c)
        {
            this._buffers.Add(c);
            if (this._buffers.Count>1)
            {
                // On our second buffer, autoinitiate.
                this.processBuffer();
            }
        }

        protected virtual void processBuffer()
        {

        }

        protected CommandBuffer getCurrentBuffer()
        {
            return this._buffers[this._buffers.Count - 1];
        }
    }
}
