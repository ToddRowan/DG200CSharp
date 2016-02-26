using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kimandtodd.DG200CSharp.commands
{
    public class GetDGSoftwareVerCommand : BaseCommand
    {
        private static byte commandId = 0xBC;
        private bool gpsMouse = false;

        /// <summary>
        /// Constructor. Calls the parent to initialize. 
        /// </summary>
        public GetDGSoftwareVerCommand() : base()
        {

        }

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
