using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kimandtodd.DG200CSharp
{
    public class GetDGTrackFileCommand : BaseCommandData
    {
        private static byte commandId = 0xB5;
        private int trackIndex = 0;

        public new byte[] getCommandData()
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

        /// <summary>
        /// This command returns two complete payloads, one after the other.
        /// This method allows that command to notify the serial processor to keep reading 
        /// from the buffer after the first session completes. 
        /// </summary>
        /// <returns>2</returns>
        new public int getExpectedSessionCount()
        {
            return 2;
        }
    }
}
