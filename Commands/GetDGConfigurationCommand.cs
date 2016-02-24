using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kimandtodd.DG200CSharp
{
    public class GetDGConfigurationCommand : BaseCommandData
    {
        private static byte commandId = 0xB7;
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
