using System;

namespace kimandtodd.DG200CSharp.commandresults.resultitems
{
    public class DGPositionDateTimeAltitudeTrackPoint : DGPositionDateTimeTrackPoint
    {
        public DGPositionDateTimeAltitudeTrackPoint(byte[] input) : base(input)
        {
            this._format = BaseDGTrackPoint.FORMAT_POSITION_DATE_SPEED_ALTITUDE;
            this.processAltitude();
        }

        private void processAltitude()
        {
            // altitude is bytes 20-23
            ArraySegment<byte> altSeg = new ArraySegment<byte>(this._rawData, 20, 4);
            string a = this.intToSixDigitString(DG200Utils.bigEndianArrayToInt32(altSeg));

            this._altitude = UInt32.Parse(a);
        }
    }
}
