using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kimandtodd.DG200CSharp
{
    public class GetDGTrackHeadersCommand : BaseCommandData
    {
        private static byte commandId = 0xBB;
        private int startingTrackIndex = 0;
        public new byte[] getCommandData()
        {
            return buildCommandArray(assembleCommandData());
        }

        public void setStartingTrackIndex(int newIndex)
        {
            this.startingTrackIndex = newIndex;
        }

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
    }
}
