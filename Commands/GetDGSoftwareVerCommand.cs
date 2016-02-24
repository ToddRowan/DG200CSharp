using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kimandtodd.DG200CSharp
{
    public class GetDGSoftwareVerCommand : BaseCommandData
    {
        private static byte commandId = 0xBC;
        private bool gpsMouse = false;
        public new byte[] getCommandData()
        {
            return buildCommandArray(assembleCommandData());
        }

        public void setGpsMouseOn(bool gpsMouseOn)
        {
            this.gpsMouse = gpsMouseOn;
        }

        public bool getGpsMouseOn()
        {
            return this.gpsMouse;
        }

        private byte[] assembleCommandData()
        {
            byte[] fullArray = new byte[2];
            fullArray[0] = GetDGSoftwareVerCommand.commandId;

            if (this.gpsMouse)
            {
                fullArray[1] = 0x01;
            }
            else
            {
                fullArray[1] = 0x0;
            }

            return fullArray;
        }        
    }
}
