using System;

using kimandtodd.DG200CSharp.commandresults;
using kimandtodd.DG200CSharp.sessions;

namespace kimandtodd.DG200CSharp.commands
{
    public class SetDGStartTypeCommand : BaseCommand
    {
        private static byte commandId = 0x80;

        private byte _currentStartType;

        public static byte COLD_START = 0x84;
        public static byte WARM_START = 0x83;
        public static byte HOT_START = 0xc0;
        public static byte FACTORY_RESET = 0x88;

        /// <summary>
        /// Constructor. Calls the parent to initialize. 
        /// </summary>
        public SetDGStartTypeCommand() : base()
        {
            this._currentStartType = SetDGStartTypeCommand.COLD_START;
            this._currentResult = new SetDGStartTypeCommandResult();
            this._session = new BaseSession();
            this._session.setResult(this._currentResult);
        }

        public override byte[] getCommandData()
        {
            return buildCommandArray(assembleCommandData());
        }

        public void setStartType(byte newType)
        {
            this._currentStartType = newType;
        }

        public UInt16 getStartType()
        {
            return this._currentStartType;
        }

        private byte[] assembleCommandData()
        {
            byte[] fullArray = new byte[25];
            fullArray[0] = SetDGStartTypeCommand.commandId;

            for (int x = 1; x < 23; x++)
            {
                fullArray[x] = 0x0;
            }
            fullArray[23] = 0x0c;

            fullArray[24] = this._currentStartType;

            return fullArray;
        }
    }
}
