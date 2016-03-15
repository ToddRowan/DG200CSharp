using System;

namespace kimandtodd.DG200CSharp.commandresults.resultitems
{
    public class DGFirstTrackPoint : DGPositionDateTimeAltitudeTrackPoint
    {
        public DGFirstTrackPoint(byte[] input) : base(input)
        {
            this.processFormat();
        }

        private void processFormat()
        {
            // time is bytes 8-11, date is bytes 12-15.
            ArraySegment<byte> formatSeg = new ArraySegment<byte>(this._rawData, 28, 4);
            string f = this.intToSixDigitString(this.bigEndianArrayToInt32(formatSeg));

            this._format = UInt16.Parse(f);
        }
    }
}
