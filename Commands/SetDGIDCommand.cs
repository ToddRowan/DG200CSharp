using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kimandtodd.DG200CSharp
{
    public class SetDGIDCommand : BaseCommandData
    {
        private static byte commandId = 0xC0;
        private byte[] newId = { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0};
        public new byte[] getCommandData()
        {
            return buildCommandArray(assembleCommandData());
        }

        public void setNewDGID(byte[] input)
        {            
            this.newId = input;
        }

        private byte[] assembleCommandData()
        {
            byte[] fullArray = new byte[9];
            fullArray[0] = SetDGIDCommand.commandId;

            this.fixIdArray();
            this.newId.CopyTo(fullArray, 1);

            return fullArray;
        }

        private void fixIdArray()
        {
            // if array is 8 bytes, do nothing.

            if (this.newId.Length == 8)
            {
                return;
            }
            
            // if array is too short, pad with zeros
            if (this.newId.Length < 8)
            {
                byte[] tmpArray = new byte[] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
                this.newId.CopyTo(tmpArray, 0);
                this.newId = tmpArray;

                return;
            }

            // if array is too long, truncate
            Array.Resize(ref this.newId, 8);
        }
    }
}
