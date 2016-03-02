using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kimandtodd.DG200CSharp.commands
{
    public class GetDGTrackFileCommand : BaseCommand
    {
        private static byte commandId = 0xB5;
        private int trackIndex = 0;

        /// <summary>
        /// Constructor. Calls the parent to initialize. 
        /// </summary>
        public GetDGTrackFileCommand() : base()
        {

        }

        public override byte[] getCommandData()
        {
            return buildCommandArray(assembleCommandData());
        }

        public void setTrackIndex(int newIndex)
        {
            this.trackIndex = newIndex;
        }

        public int getTrackIndex()
        {
            return this.trackIndex;
        }

        private byte[] assembleCommandData()
        {
            byte[] fullArray = new byte[3];
            fullArray[0] = GetDGTrackFileCommand.commandId;

            byte[] convertedTrack = BitConverter.GetBytes(this.trackIndex);

            fullArray[2] = convertedTrack[0];
            fullArray[1] = convertedTrack[1];

            return fullArray;
        }
    }
}
