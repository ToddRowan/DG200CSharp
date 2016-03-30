using kimandtodd.DG200CSharp.commandresults;
using kimandtodd.DG200CSharp.sessions;

namespace kimandtodd.DG200CSharp.commands
{
    public class DeleteAllDGTrackFilesCommand : BaseCommand
    {
        private static byte commandId = 0xBA;

        /// <summary>
        /// Constructor. Calls the parent to initiailize internal data.
        /// </summary>
        public DeleteAllDGTrackFilesCommand() : base()
        {
            this._currentResult = new DeleteAllTrackFilesCommandResult();
            this._session = new BaseSession();
            this._session.setResult(this._currentResult);
        }
        
        public override byte[] getCommandData()
        {
            return buildCommandArray(assembleCommandData());
        }

        private byte[] assembleCommandData()
        {
            byte[] fullArray = new byte[3];
            fullArray[0] = DeleteAllDGTrackFilesCommand.commandId;

            fullArray[1] = 0xFF;
            fullArray[2] = 0xFF;

            return fullArray;
        }
    }
}
