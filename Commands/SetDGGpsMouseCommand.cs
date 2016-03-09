using kimandtodd.DG200CSharp.commandresults;

namespace kimandtodd.DG200CSharp.commands
{
    public class SetDGGpsMouseCommand : BaseCommand
    {
        private static byte commandId = 0xBC;
        private bool _gpsMouse = false;

        /// <summary>
        /// Constructor. Calls the parent to initialize. 
        /// </summary>
        public SetDGGpsMouseCommand() : base()
        {

        }

        public override byte[] getCommandData()
        {
            return buildCommandArray(assembleCommandData());
        }

        public void setGpsMouseOn(bool gpsMouseOn)
        {
            this._gpsMouse = gpsMouseOn;
        }

        public bool getGpsMouseOn()
        {
            return this._gpsMouse;
        }

        private byte[] assembleCommandData()
        {
            byte[] fullArray = new byte[2];
            fullArray[0] = SetDGGpsMouseCommand.commandId;

            if (this._gpsMouse)
            {
                fullArray[1] = 0x01;
            }
            else
            {
                fullArray[1] = 0x0;
            }

            return fullArray;
        }

        /// <summary>
        /// Returns the result after executing a command.
        /// </summary>
        /// <returns>A GetDGIDCommandResult instance.</returns>
        protected override void processResult()
        {
            this._currentResult = new SetDGGpsMouseCommandResult(this._buf);
        }
    }
}
