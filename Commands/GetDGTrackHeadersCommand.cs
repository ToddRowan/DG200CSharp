using System;

using kimandtodd.DG200CSharp.commandresults;

namespace kimandtodd.DG200CSharp.commands
{
    public class GetDGTrackHeadersCommand : BaseCommand
    {
        private static byte commandId = 0xBB;
        private int startingTrackIndex = 0;

        /// <summary>
        /// Constructor. Calls the parent to initialize. 
        /// </summary>
        public GetDGTrackHeadersCommand() : base()
        {

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
            this.startingTrackIndex = newIndex;
        }

        /// <summary>
        /// Get the current starting track index. 
        /// </summary>
        /// <returns>An integer value of the starting index.</returns>
        public int getStartingTrackIndex()
        {
            return this.startingTrackIndex;
        }

        private byte[] assembleCommandData()
        {
            byte[] fullArray = new byte[3];
            fullArray[0] = GetDGTrackHeadersCommand.commandId;

            byte[] convertedStartTrack = BitConverter.GetBytes(this.startingTrackIndex);

            fullArray[2] = convertedStartTrack[0];
            fullArray[1] = convertedStartTrack[1];

            return fullArray;
        }

        /// <summary>
        /// Returns the result after executing a command.
        /// </summary>
        /// <returns>A GetDGIDCommandResult instance.</returns>
        public override BaseCommandResult getLastResult()
        {
            return new GetDGTrackHeadersCommandResult(this._buf);
        }
    }
}
