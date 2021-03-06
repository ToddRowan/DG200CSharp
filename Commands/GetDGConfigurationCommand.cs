﻿using kimandtodd.DG200CSharp.commandresults;
using kimandtodd.DG200CSharp.sessions;

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
            this._currentResult = new GetDGConfigurationCommandResult();
            this._session = new BaseSession();
            this._session.setResult(this._currentResult);
        }

        /// <summary>
        /// Builds the complete command to send to the system.
        /// </summary>
        /// <returns>The complete command byte array.</returns>
        public override byte[] getCommandData()
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
