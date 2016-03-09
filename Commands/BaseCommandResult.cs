using System;
using System.Collections.Generic;

using kimandtodd.DG200CSharp.commands;
using kimandtodd.DG200CSharp.logging;

namespace kimandtodd.DG200CSharp.commandresults
{
    public abstract class BaseCommandResult : ICommandResult
    {
        private bool _successState;
        protected bool _additionalSession;
        protected List<CommandBuffer> _buffers;

        protected static int COMMAND_LOCATION = 4;
        protected static int PAYLOAD_START = 5;

        protected BaseCommandResult(CommandBuffer resultBuf)
        {
            DG200FileLogger.Log("BaseCommandResult constructor.", 3);
            this._buffers = new List<CommandBuffer>();

            this.addResultBuffer(resultBuf);
            this._successState = false;
            this._additionalSession = false;
        }

        public bool getSuccess()
        {
            return this._successState;
        }

        public String getErrorMessage()
        {
            return "";
        }

        public virtual bool requestAdditionalSession()
        {
            return this._additionalSession;
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

        protected virtual CommandBuffer getCurrentBuffer()
        {
            return this._buffers[this._buffers.Count - 1];
        }

        protected int getBufferCount()
        {
            return this._buffers.Count;
        }
    }
}
