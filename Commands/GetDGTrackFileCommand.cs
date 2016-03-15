using System;

using kimandtodd.DG200CSharp.commandresults;
using kimandtodd.DG200CSharp.logging;
using kimandtodd.DG200CSharp.sessions;

namespace kimandtodd.DG200CSharp.commands
{
    public class GetDGTrackFileCommand : BaseCommand
    {
        private static byte commandId = 0xB5;
        private int _trackIndex;

        /// <summary>
        /// Constructor. Calls the parent to initialize. 
        /// </summary>
        public GetDGTrackFileCommand() : base()
        {
            this._trackIndex = 0;

            this._currentResult = new GetDGTrackFileCommandResult();
            this._session = new GetDGTrackFileSession();
            this._session.setResult(this._currentResult);
        }

        public override byte[] getCommandData()
        {
            return buildCommandArray(assembleCommandData());
        }

        public void setTrackIndex(int newIndex)
        {
            this._trackIndex = newIndex;
        }

        public int getTrackIndex()
        {
            return this._trackIndex;
        }

        private byte[] assembleCommandData()
        {
            byte[] fullArray = new byte[3];
            fullArray[0] = GetDGTrackFileCommand.commandId;

            byte[] convertedTrack = BitConverter.GetBytes(this._trackIndex);

            fullArray[2] = convertedTrack[0];
            fullArray[1] = convertedTrack[1];

            return fullArray;
        }
    }
}
