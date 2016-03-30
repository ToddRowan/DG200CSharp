using System;

using kimandtodd.DG200CSharp.commandresults;

namespace kimandtodd.DG200CSharp.commands
{
    public class DG200Configuration
    {
        private CommandBuffer _buf;

        private int _informationType;
        private bool _speedThreshold;
        private UInt32 _speedThresholdValue;
        private bool _distanceThreshold;
        private UInt32 _distanceThresholdValue;
        private UInt32 _timeInterval;
        private bool _overwriteData;
        private bool _useTimeInterval; // figure out what this is and pick a better name
        private UInt32 _distanceInterval;
        private int _operationMode;
        private bool _enableWaas;
        private int _memoryUsage;
        private int _modelType;

        public DG200Configuration()
        {
            // Find out what the defaults are and set those here.
        }

        public DG200Configuration(CommandBuffer buf)
        {
            this._buf = buf;

            this.readConfig();
        }

        private void readConfig()
        {
            // Array with the entire read config.
            byte[] fortyFiveBytes = new byte[45];

            this._buf.Position = BaseCommandResult.PAYLOAD_START;

            this._buf.Read(fortyFiveBytes, 0, fortyFiveBytes.Length);

            this.setTrackingType(Convert.ToInt32(fortyFiveBytes[0]));

            this.setUseSpeedThreshold(Convert.ToBoolean(fortyFiveBytes[1]));


            ArraySegment<byte> seg = new ArraySegment<byte>(fortyFiveBytes, 2, 4);
            this.setSpeedThresholdValue(Convert.ToUInt32(DG200Utils.bigEndianArrayToInt32(seg)));

            this.setUseDistanceThreshold(Convert.ToBoolean(fortyFiveBytes[6]));

            seg = new ArraySegment<byte>(fortyFiveBytes, 7, 4);
            this.setDistanceThresholdValue(Convert.ToUInt32(DG200Utils.bigEndianArrayToInt32(seg)));

            seg = new ArraySegment<byte>(fortyFiveBytes, 11, 4);
            this.setTimeInterval(Convert.ToUInt32(DG200Utils.bigEndianArrayToInt32(seg)));

            this.setOverwriteData(Convert.ToBoolean(fortyFiveBytes[24]));

            this.setUseTimeInterval(Convert.ToBoolean(fortyFiveBytes[25]));


            seg = new ArraySegment<byte>(fortyFiveBytes, 28, 4);
            this.setDistanceInterval(Convert.ToUInt32(DG200Utils.bigEndianArrayToInt32(seg)));

            this.setOperationMode(Convert.ToInt32(fortyFiveBytes[40]));

            this.setEnableWaas(Convert.ToBoolean(fortyFiveBytes[41]));

            this.setMemoryUsage(Convert.ToInt32(fortyFiveBytes[42]));

            this.setModelType(Convert.ToInt32(fortyFiveBytes[44]));
        }

        public void setTrackingType(int type)
        {
            this._informationType = type;
        }

        public int getTrackingType()
        {
            return this._informationType;
        }

        public void setUseSpeedThreshold (bool use)
        {
            this._speedThreshold = use;
        }

        public bool getUseSpeedThreshold()
        {
            return this._speedThreshold;
        }

        public void setSpeedThresholdValue(UInt32 threshold)
        {
            this._speedThresholdValue = threshold;
        }

        public UInt32 getSpeedThresholdValue()
        {
            return this._speedThresholdValue;
        }

        public void setUseDistanceThreshold(bool use)
        {
            this._distanceThreshold = use;
        }

        public bool getUseDistanceThreshold()
        {
            return this._distanceThreshold;
        }

        public void setDistanceThresholdValue(UInt32 threshold)
        {
            this._distanceThresholdValue = threshold;
        }

        public UInt32 getDistanceThresholdValue()
        {
            return this._distanceThresholdValue;
        }

        public void setTimeInterval(UInt32 interval)
        {
            this._timeInterval = interval;
        }

        public UInt32 getTimeInterval()
        {
            return this._timeInterval;
        }

        public void setOverwriteData(bool overwrite)
        {
            this._overwriteData = overwrite;
        }

        public bool getOverwriteData()
        {
            return this._overwriteData;
        }

        public void setUseTimeInterval(bool use)
        {
            this._useTimeInterval = use;
        }

        public bool getUseTimeInterval()
        {
            return this._useTimeInterval;
        }

        public void setDistanceInterval(UInt32 interval)
        {
            this._distanceInterval = interval;
        }

        public UInt32 getDistanceInterval()
        {
            return this._distanceInterval;
        }

        public void setOperationMode(int mode)
        {
            this._operationMode = mode;
        }

        public int getOperationMode()
        {
            return this._operationMode;
        }

        public void setEnableWaas(bool enable)
        {
            this._enableWaas = enable;
        }

        public bool getEnableWaas()
        {
            return this._enableWaas;
        }

        private void setMemoryUsage(int usage)
        {
            this._memoryUsage = usage;
        }

        public int getMemoryUsage()
        {
            return this._memoryUsage;
        }

        private void setModelType(int type)
        {
            this._modelType = type;
        }

        public int getModelType()
        {
            return this._modelType;
        }

        public override string ToString()
        {
            string s = "Settings for DG200:\n";
            s += "Tracking information type value: " + this.getTrackingType() + "\n";
            s += "Use speed threshold: " + (this.getUseSpeedThreshold()?"true":"false") + "\n";
            s += "Speed threshold value: " + this.getSpeedThresholdValue() + "\n";
            s += "Use distance threshold: " + (this.getUseDistanceThreshold() ? "true" : "false") + "\n";
            s += "Distance threshold value: " + this.getDistanceThresholdValue() + "\n";
            s += "Use time interval: " + (this.getUseTimeInterval() ? "true" : "false") + "\n";
            s += "Time interval value: " + this.getTimeInterval() + "\n";
            s += "Distance interval value: " + this.getDistanceInterval() + "\n";
            s += "Operation mode value: " + this.getOperationMode() + "\n";
            s += "Enable WAAS: " + (this.getEnableWaas() ? "true" : "false") + "\n";
            s += "Memory consumption value: " + this.getMemoryUsage() + "\n";
            s += "Model Type value: " + this.getModelType();


            return s;
        }
    }
}
