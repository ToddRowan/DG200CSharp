using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
