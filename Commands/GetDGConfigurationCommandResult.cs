using System;

using kimandtodd.DG200CSharp.commands;

namespace kimandtodd.DG200CSharp.commandresults
{
    public class GetDGConfigurationCommandResult : BaseCommandResult
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="resultBuf">The buffer with the result of the command.</param>
        public GetDGConfigurationCommandResult(CommandBuffer resultBuf)
            : base(resultBuf)
        {
            this.init();
        }

        private void init()
        {

        }
    }
}
