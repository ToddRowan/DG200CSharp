using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kimandtodd.DG200CSharp.commands
{
    public class GetDGConfigurationCommand : BaseCommand
    {
        private static byte commandId = 0xB7;

        /// <summary>
        /// Constructor. Calls the parent to initiailize internal data.
        /// </summary>
        public GetDGConfigurationCommand() : base()
        {

        }
        public new byte[] getCommandData()
        {
            return buildCommandArray(assembleCommandData());
        }

        private byte[] assembleCommandData()
        {
            byte[] fullArray = new byte[1];
            fullArray[0] = GetDGConfigurationCommand.commandId;

            return fullArray;
        }
    }
}
