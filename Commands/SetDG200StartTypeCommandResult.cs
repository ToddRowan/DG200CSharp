using kimandtodd.DG200CSharp.commands;

namespace kimandtodd.DG200CSharp.commandresults
{
    public class SetDG200StartTypeCommandResult : BaseCommandResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultBuf">The buffer with the result of the command.</param>
        public SetDG200StartTypeCommandResult(CommandBuffer resultBuf)
            : base(resultBuf)
        {

        }
    }
}
