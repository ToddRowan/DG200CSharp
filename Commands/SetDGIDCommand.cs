﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using kimandtodd.DG200CSharp.commandresults;

namespace kimandtodd.DG200CSharp.commands
{
    public class SetDGIDCommand : BaseCommand
    {
        private static byte commandId = 0xC0;
        private byte[] newId = { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0};

        /// <summary>
        /// Constructor. Calls the parent to initialize. 
        /// </summary>
        public SetDGIDCommand() : base()
        {

        }

        public override byte[] getCommandData()
        {
            return buildCommandArray(assembleCommandData());
        }

        /// <summary>
        /// Set the new ID by supplying eight bytes.
        /// </summary>
        /// <param name="input">The byte array to set on the DG200 as the new ID. Any less than 8 bytes is padded with zeroes, any more is truncated.</param>
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

        protected override void initializeResult(CommandBuffer c)
        {
            // Is there any reason why I want to store the buffer?
            this._buffers.Add(c);
            // We should probably never do a conditional add here. 
            this._currentResult = new SetDGIDCommandResult(c);                        
        }
    }
}
