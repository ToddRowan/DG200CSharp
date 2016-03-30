using System;

using kimandtodd.DG200CSharp.commandresults;
using kimandtodd.DG200CSharp.sessions;

namespace kimandtodd.DG200CSharp.commands
{
    public class SetDGConfigurationCommand : BaseCommand
    {
        private static byte commandId = 0xB8;

        private DG200Configuration _config;

        /// <summary>
        /// Constructor. Calls the parent to initiailize internal data.
        /// </summary>
        public SetDGConfigurationCommand(DG200Configuration config) : base()
        {
            this._config = config;
            this._currentResult = new SetDGConfigurationCommandResult();
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
            byte[] fullArray = new byte[43];

            // Zero out the contents. 
            Array.Clear(fullArray,0,fullArray.Length);

            fullArray[0] = SetDGConfigurationCommand.commandId;

            fullArray[1] = Convert.ToByte(this._config.getTrackingType());

            fullArray[2] = Convert.ToByte(this._config.getUseSpeedThreshold());

            Array.Copy(DG200Utils.int32ToBigEndianArray(Convert.ToInt32(this._config.getSpeedThresholdValue())), 0, fullArray, 3, 4);

            fullArray[7] = Convert.ToByte(this._config.getUseDistanceThreshold());

            Array.Copy(DG200Utils.int32ToBigEndianArray(Convert.ToInt32(this._config.getDistanceThresholdValue())), 0, fullArray, 8, 4);

            Array.Copy(DG200Utils.int32ToBigEndianArray(Convert.ToInt32(this._config.getTimeInterval())), 0, fullArray, 12, 4);

            fullArray[26] = Convert.ToByte(this._config.getUseTimeInterval());

            Array.Copy(DG200Utils.int32ToBigEndianArray(Convert.ToInt32(this._config.getDistanceInterval())), 0, fullArray, 29, 4);

            fullArray[41] = Convert.ToByte(this._config.getOperationMode());

            fullArray[42] = Convert.ToByte(this._config.getEnableWaas());


            return fullArray;
        }
    }
}

