﻿using System;

using kimandtodd.DG200CSharp.commandresults;
using kimandtodd.DG200CSharp.logging;

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

        protected override void processResult()
        {
            // We should probably never do a conditional add here. 
            this._currentResult = new SetDGIDCommandResult(this._buf);
        }

        /// <summary>
        /// This command requires a manual override on the data size value b/c it comes back wrong. 
        /// </summary>
        protected override void overrideExpectedByteCount()
        {            
            this._expectedByteCount += 4;
            DG200FileLogger.Log("SetDGIDCommand overriding expected byte count: " + this._expectedByteCount, 3);
        }
    }
}
