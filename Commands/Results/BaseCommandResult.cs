using System;
using System.Collections.Generic;

using kimandtodd.DG200CSharp.commands;
using kimandtodd.DG200CSharp.logging;

namespace kimandtodd.DG200CSharp.commandresults
{
    public abstract class BaseCommandResult : ICommandResult
    {
        protected List<CommandBuffer> _buffers;

        public static int COMMAND_LOCATION = 4;
        public static int PAYLOAD_START = 5;

        protected BaseCommandResult()
        {
            DG200FileLogger.Log("BaseCommandResult constructor.", 3);
            this._buffers = new List<CommandBuffer>();
        }

        /// <summary>
        /// This may not need to be overridden, if I just move that logic into the process buffer command. 
        /// </summary>
        /// <param name="c">The new buffer to read from.</param>
        public void addResultBuffer(CommandBuffer c)
        {
            this._buffers.Add(c);
            this.processBuffer();
        }

        /// <summary>
        /// Overridable result method that decides what to do with the buffer. 
        /// </summary>
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
