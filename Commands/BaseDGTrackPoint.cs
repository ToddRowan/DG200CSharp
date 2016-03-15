using System;
using System.Collections.Generic;

namespace kimandtodd.DG200CSharp.commandresults.resultitems
{    
    public class BaseDGTrackPoint : IDGTrackPoint
    {
        protected DateTime _dt;
        protected Tuple<Int16, Double> _long;
        protected Tuple<Int16, Double> _lat;
        protected UInt32 _altitude;
        protected UInt32 _speed;
        protected UInt16 _format;
        protected bool _isWayPoint;
        protected byte[] _rawData;

        protected static UInt32 SPEED_MULTIPLIER = 100;
        protected static UInt32 ALTITUDE_MULTIPLIER = 10000;

        protected static UInt16 TRACK_LENGTH_NO_ALTITUDE = 20;
        protected static UInt16 TRACK_LENGTH_WITH_ALTITUDE = 32;

        public static UInt16 FORMAT_POSITION = 0;
        public static UInt16 FORMAT_POSITION_DATE_SPEED = 1;
        public static UInt16 FORMAT_POSITION_DATE_SPEED_ALTITUDE = 2;
        

        public BaseDGTrackPoint(byte[] input)
        {
            this._rawData = input;
            this._isWayPoint = false;

            this.readLatitude();
            this.readLongitude();
            this.processDateTime();
        }

        private void readLatitude()
        {
            ArraySegment<byte> latSeg = new ArraySegment<byte>(this._rawData, 0, 4);
            
            int raw = this.bigEndianArrayToInt32(latSeg);
            raw = this.processWaypoint(raw);

            string s = this.intToNineDigitString(raw);

            this._lat = this.makeCoordinate(s);
        }

        private void readLongitude()
        {
            ArraySegment<byte> lonSeg = new ArraySegment<byte>(this._rawData, 4, 4);
            string s = this.intToNineDigitString(this.bigEndianArrayToInt32(lonSeg));

            this._long = this.makeCoordinate(s);
        }

        // Create the coordinate tuple with the string we get from formatting. 
        private Tuple<Int16, Double> makeCoordinate(string coordinate)
        {
            int subPos = 3;
            if (coordinate.StartsWith("-"))
            {
                subPos++;
            }

            // get last 6 chars for coordinates
            string minSec = coordinate.Substring(subPos);
            string deg = coordinate.Substring(0, subPos);

            return new Tuple<Int16, Double>(Int16.Parse(deg), Double.Parse(minSec) / 10000);
        }

        // Does two things. Determines if a lat value is a waypoint,
        // and also strips off the excess waypoint value. 
        private int processWaypoint(int wp)
        {
            int multiplier = (wp < 0 ? -1 : 1);
            int abs = Math.Abs(wp);
            int wayPointVal = 100000000;
            if (abs > wayPointVal)
            {
                this._isWayPoint = true;
                abs = abs - wayPointVal;
            }

            return abs * multiplier;
        }

        // At this level, just create a fake date/time equivalent to the GPS Epoch. 
        protected virtual void processDateTime()
        {
            this._dt = new DateTime(1980,1,6,0,0,0, DateTimeKind.Utc);
        }

        /// <summary>
        /// Get the altitude reading for this track point, if any. 
        /// </summary>
        /// <returns>Meters * 10000. So divide by 10000 to get the value in meters.</returns>
        public virtual long getAltitude()
        {
            return 0;
        }

        /// <summary>
        /// Get the speed reading for this track point, if any. 
        /// </summary>
        /// <returns>Kilometers * 100. So divide by 100 to the value in km/hr.</returns>
        public virtual long getSpeed()
        {
            return 0;
        }

        /// <summary>
        /// Indicates if this track point was a saved waypoint. 
        /// </summary>
        /// <returns>True if yes, false otherwise.</returns>
        public bool isWayPoint()
        {
            return this._isWayPoint;
        }

        /// <summary>
        /// The latitude value for this track point. 
        /// </summary>
        /// <returns>The first value of the tuple is the latitude (-90 to 90), the second value is the decimal 
        /// representation of minutes, to four decimal points.</returns>
        public Tuple<Int16, Double> getLatitude()
        {
            return this._lat;
        }

        /// <summary>
        /// The latitude value for this track point. 
        /// </summary>
        /// <returns>The first value of the tuple is the longitude (-180 to 180), the second value is the decimal
        /// representation of minutes, to four decimal points.</returns>
        public Tuple<Int16, Double> getLongitude()
        {
            return this._long;
        }

        /// <summary>
        /// The date/time of this track point, if any. If the value is equal to the GPS Epoch (Jan 6, 1980), 
        /// then the date/time is considered unknown. 
        /// </summary>
        /// <returns></returns>
        public virtual DateTime getDateTime()
        {
            return this._dt;
        }

        /// <summary>
        /// The format of this track point as read from the device. 
        /// </summary>
        /// <returns>0: Position only, 1: Position, date/time, and speed, 2: Like 1 but also with altitude.</returns>
        public virtual UInt16 getTrackFormat()
        {
            return this._format;
        }

        // Turn a six digit integer into a nine-digit string.
        protected String intToNineDigitString(int src)
        {
            // Make sure we get a leading zero for first byte values less than 10
            string fmt = "000000000";
            return src.ToString(fmt);
        }

        // Turn a six digit integer into a six-digit string.
        protected String intToSixDigitString(int src)
        {
            // Make sure we get a leading zero for first byte values less than 10
            string fmt = "000000";
            return src.ToString(fmt);
        }

        //protected int bigEndianArrayToInt32(byte[] arr, int start)
        //{
        //    int size = 4, countUp = 0, countDown = start + size - 1;
        //    byte[] tmpArr = new byte[size];
        //    while (countUp < size)
        //    {
        //        tmpArr[countUp++] = arr[countDown--];
        //    }

        //    return BitConverter.ToInt32(tmpArr, 0);
        //}

        protected int bigEndianArrayToInt32(ArraySegment<byte> arr)
        {
            int size = 4, countUp = 0, countDown = arr.Offset + size - 1;
            byte[] tmpArr = new byte[size];
            while (countUp < size)
            {
                tmpArr[countUp++] = arr.Array[countDown--];
            }

            return BitConverter.ToInt32(tmpArr, 0);
        }
    }
}
