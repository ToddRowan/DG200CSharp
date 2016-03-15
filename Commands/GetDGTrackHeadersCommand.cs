using System;

using kimandtodd.DG200CSharp.commandresults;
using kimandtodd.DG200CSharp.logging;
using kimandtodd.DG200CSharp.sessions;

namespace kimandtodd.DG200CSharp.commands
{
    public class GetDGTrackHeadersCommand : BaseCommand
    {
        private static byte commandId = 0xBB;
        private int _startingTrackIndex;
        private GetDGTrackHeadersCommandResult _locResult;

        /// <summary>
        /// Constructor. Calls the parent to initialize. 
        /// </summary>
        public GetDGTrackHeadersCommand() : base()
        {
            DG200FileLogger.Log("GetDGTrackHeadersCommand constructor.", 3);
            this._startingTrackIndex = 0;

            this._locResult = new GetDGTrackHeadersCommandResult();
            this._session = new GetDGTrackHeadersSession();
            this._session.setResult(this._locResult);
            this._currentResult = this._locResult;
        }

        /// <summary>
        /// Gets the command data for the serial connector. 
        /// </summary>
        /// <returns>A byte array with the command data.</returns>
        public override byte[] getCommandData()
        {
            return buildCommandArray(assembleCommandData());
        }

        /// <summary>
        /// You can request track headers by index value. If you don't the command will start at zero. 
        /// </summary>
        /// 
        /// <param name="newIndex">The track index to set.</param>
        public void setStartingTrackIndex(int newIndex)
        {
            this._startingTrackIndex = newIndex;
        }

        /// <summary>
        /// Get the current starting track index. 
        /// </summary>
        /// <returns>An integer value of the starting index.</returns>
        public int getStartingTrackIndex()
        {
            return this._startingTrackIndex;
        }

        private byte[] assembleCommandData()
        {
            byte[] fullArray = new byte[3];
            fullArray[0] = GetDGTrackHeadersCommand.commandId;

            byte[] convertedStartTrack = BitConverter.GetBytes(this._startingTrackIndex);

            fullArray[2] = convertedStartTrack[0];
            fullArray[1] = convertedStartTrack[1];

            return fullArray;
        }

        /// <summary>
        /// Asks the session if we should make another request to the DG200.
        /// </summary>
        /// <returns>True if keep going, false otherwise.</returns>
        public override bool doAnotherSession()
        {
            // Here we ask our session what we should do.
            // If the answer is yes, we need to set the input param accordingly.
            this.setStartingTrackIndex(this._locResult.getNextTrackHeaderId());
            return this._session.callAgain();
        }
    }
}
