using System;

using kimandtodd.DG200CSharp.commandresults;
using kimandtodd.DG200CSharp.logging;

namespace kimandtodd.DG200CSharp.commands
{
    public class GetDGTrackFileCommand : BaseCommand
    {
        private static byte commandId = 0xB5;
        private int _trackIndex;
        private bool _overrideContinue;
        private GetDGTrackFileCommandResult _result;

        /// <summary>
        /// Constructor. Calls the parent to initialize. 
        /// </summary>
        public GetDGTrackFileCommand() : base()
        {
            this._trackIndex = 0;
            this._overrideContinue = false;
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

        /// <summary>
        /// Ask the command if it can continue. For this command we have some extra math to consider, 
        /// so we ask the result what we should do. It knows the current state.
        /// </summary>
        /// <returns>True if yes, false otherwise.</returns>
        public override bool startSession()
        {
            return this._result.requestAdditionalSession();
        }

        protected override void processResult()
        {
            if (this._currentResult == null)
            {
                this._currentResult = this._result = new GetDGTrackFileCommandResult(this._buf);
            }
            else
            {
                this._currentResult.addResultBuffer(this._buf);
                // TODO: Why do I set this? I don't think I actually use it. 
                this._requestAdditionalSession = this._currentResult.requestAdditionalSession();
            }
        }

        public override bool continueReading()
        {
            if (this._currentResult == null)
            {
                return base.continueReading();
            }
            else
            {
                return this._overrideContinue;
            }
            
        }

        protected override void evaluateData()
        {
            // If we still don't have the first result, use the parent command.
            if (this._currentResult == null)
            {
                base.evaluateData();
            }
            else
            {
                // This payload doesn't have the expected headers. So it 
                this.overrideExpectedByteCount();
            }
        }

        protected override void overrideExpectedByteCount()
        {
            DG200FileLogger.Log("GetTrackFileCommand overriding expected payload size on second session.", 3);
            this._expectedByteCount = 1024;
        }
    }
}
