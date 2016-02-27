using kimandtodd.DG200CSharp.commands;

namespace kimandtodd.DG200CSharp.commandresults
{
    /// <summary>
    /// This class is empty. Setting GPS Mouse on or off only changes how the system behaves.
    /// It doesn't actually return anything. 
    /// </summary>
    public class SetDGGpsMouseCommandResult : BaseCommandResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultBuf">The buffer with the result of the command.</param>
        public SetDGGpsMouseCommandResult(CommandBuffer resultBuf)
            : base(resultBuf)
        {

        }
    }
}
