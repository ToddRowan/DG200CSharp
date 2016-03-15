using kimandtodd.DG200CSharp.commandresults;
using kimandtodd.DG200CSharp.sessions;

namespace kimandtodd.DG200CSharp.commands
{
    public class GetDGIDCommand : BaseCommand
    {
        private static byte[] commandArray = new byte[] { 0xBF };

        /// <summary>
        /// Constructor. Calls the parent to initialize. 
        /// </summary>
        public GetDGIDCommand() : base()
        {
            this._currentResult = new GetDGIDCommandResult();
            this._session = new BaseSession();
            this._session.setResult(this._currentResult);
        }

        /// <summary>
        /// Gets the command data for the serial connector. 
        /// </summary>
        /// <returns>A byte array with the command data.</returns>
        public override byte[] getCommandData()
        {
            return buildCommandArray(GetDGIDCommand.commandArray);
        }
    }
}
