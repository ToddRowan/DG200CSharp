using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kimandtodd.DG200CSharp
{
    public class DeleteAllDGTrackFilesCommand : BaseCommandData
    {
        private static byte commandId = 0xBA;
        public new byte[] getCommandData()
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
