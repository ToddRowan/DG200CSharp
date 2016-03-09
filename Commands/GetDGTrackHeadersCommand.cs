using System;

using kimandtodd.DG200CSharp.commandresults;
using kimandtodd.DG200CSharp.logging;

namespace kimandtodd.DG200CSharp.commands
{
    public class GetDGTrackHeadersCommand : BaseCommand
    {
        private static byte commandId = 0xBB;
        private int _startingTrackIndex;
        private GetDGTrackHeadersCommandResult _result;

        /// <summary>
        /// Constructor. Calls the parent to initialize. 
        /// </summary>
        public GetDGTrackHeadersCommand() : base()
        {
            DG200FileLogger.Log("GetDGTrackHeadersCommand constructor.", 3);
            this._startingTrackIndex = 0;
        }

        /// <summary>
        /// Gets the command data for the serial connector. 
        /// </summary>
        /// <returns>A byte array with the command data.</returns>
        public override byte[] getCommandData()
        {
            if (this._currentResult != null)
            {
                this.setStartingTrackIndex(this._result.getNextTrackId());
            }
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

        protected override void processResult()
        {
            if (this._currentResult == null)
            {
                DG200FileLogger.Log("GetDGTrackHeadersCommand processing first result.", 3);
                this._currentResult = this._result = new GetDGTrackHeadersCommandResult(this._buf);
            }
            else
            {
                DG200FileLogger.Log("GetDGTrackHeadersCommand processing subsequent result.", 3);
                this._currentResult.addResultBuffer(this._buf);
                this._requestAdditionalSession = this._currentResult.requestAdditionalSession();
            }  
        }

        /// <summary>
        /// Ask the command if it can continue. For this command we have some extra math to consider, 
        /// so we ask the result what we should do. It knows the current state.
        /// </summary>
        /// <returns>True if yes, false otherwise.</returns>
        public override bool startSession()
        {
            return this._result.requestAdditionalSession();
        }
    }
}
