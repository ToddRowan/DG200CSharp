using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kimandtodd.DG200CSharp
{
    public class GetDGIDCommand : BaseCommandData
    {
        private static byte[] commandArray = new byte[] { 0xBF };
        public new byte[] getCommandData()
        {
            return buildCommandArray(GetDGIDCommand.commandArray);
        }
    }
}
