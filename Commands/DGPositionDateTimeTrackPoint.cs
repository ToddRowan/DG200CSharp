using System;

using kimandtodd.DG200CSharp.logging;

namespace kimandtodd.DG200CSharp.commandresults.resultitems
{
    public class DGPositionDateTimeTrackPoint : DGPositionTrackPoint
    {
        public DGPositionDateTimeTrackPoint(byte[] input) : base(input)
        {
            this._format = BaseDGTrackPoint.FORMAT_POSITION_DATE_SPEED;
        }

        protected override void processDateTime()
        {
            // time is bytes 8-11, date is bytes 12-15.
            ArraySegment<byte> timeSeg = new ArraySegment<byte>(this._rawData, 8, 4);
            string t = this.intToSixDigitString(DG200Utils.bigEndianArrayToInt32(timeSeg));

            ArraySegment<byte> dateSeg = new ArraySegment<byte>(this._rawData, 12, 4);
            string d = this.intToSixDigitString(DG200Utils.bigEndianArrayToInt32(dateSeg));

            try
            {
                this._dt = new DateTime(this.getYear(d), Int32.Parse(d.Substring(2, 2)), Int32.Parse(d.Substring(0, 2)),
                    Int32.Parse(t.Substring(0, 2)), Int32.Parse(t.Substring(2, 2)), Int32.Parse(t.Substring(4, 2)), DateTimeKind.Utc);
            }
            catch(Exception e)
            {
                DG200FileLogger.Log("DGPositionDateTimeTrackPoint: Unable to process date value (" + t + "). Setting to GPS epoch.", 2);
                base.processDateTime();
            }
        }

        private int getYear(string d)
        {
            int y = int.Parse(d.Substring(4));
            // If the year value is 80, that means it doesn't know (1980). 
            if (y!=80)
            {
                return y + 2000;
            }
            else
            {
                return y + 1900;
            }
            
        }

        private void processSpeed()
        {
            // speed is bytes 16-19
            ArraySegment<byte> speedSeg = new ArraySegment<byte>(this._rawData, 16, 4);
            string s = this.intToSixDigitString(DG200Utils.bigEndianArrayToInt32(speedSeg));
            this._speed = UInt32.Parse(s);
        }
    }
}
