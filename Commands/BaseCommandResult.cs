using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using kimandtodd.DG200CSharp.commands;

namespace kimandtodd.DG200CSharp.commandresults
{
    public abstract class BaseCommandResult : ICommandResult
    {
        private bool _successState;
        protected CommandBuffer _buf;

        protected static int COMMAND_LOCATION = 4;
        protected static int PAYLOAD_START = 5;

        protected BaseCommandResult(CommandBuffer resultBuf)
        {
            this._buf = resultBuf;
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
    }
}
