using System;

namespace kimandtodd.DG200CSharp.commandresults.resultitems
{
    /// <summary>
    /// Represents and individual track header. GPS data is stored in tracks. New tracks are created anytime
    /// the device is turned on, it regains gps signal, or a track reaches the size limit. 
    /// </summary>
    public class DGTrackHeader
    {
        // Headers have date and time data.
        private int _year;
        private int _month;
        private int _day;
        private int _hour;
        private int _minute;
        private int _second;
        
        private int _fileIndex;
        
        private bool _isFirstBlock;

        private bool _isInvalid;

        // The raw track byte stream.
        private byte[] _data;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="headerData">The header byte array. 12 bytes long.</param>
        public DGTrackHeader(byte[] headerData)
        {
            this._data = headerData;

            this.init();
        }

        // Getters for the date/time values.
        public int getYear()
        {
            return this._year;
        }

        public int getMonth()
        {
            return this._month;
        }

        public int getDay()
        {
            return this._day;
        }

        public int getHour()
        {
            return this._hour;
        }

        public int getMinute()
        {
            return this._minute;
        }

        public int getSeconds()
        {
            return this._second;
        }

        /// <summary>
        /// Headers have an index value that corresponds to the gps data file. 
        /// </summary>
        /// <returns>The file index for this header.</returns>
        public int getFileIndex()
        {
            return this._fileIndex;
        }

        /// <summary>
        /// A first block indicates a session start (any time the device initializes the gps connection).
        /// First blocks occur when you turn the device on or regain signal (leave a building, for example).
        /// </summary>
        /// <returns>True if the first block, false otherwise.</returns>
        public bool getIsFirstBlock()
        {
            return this._isFirstBlock;
        }

        /// <summary>
        /// If a header contains waypoints but was never able to make contact with the satellites
        /// the data isn't really worth anything, but the device still creates a header and a file. 
        /// So we mark that here to tell readers to ignore or discard this record.
        /// </summary>
        /// <returns>True if valid, false otherwise.</returns>
        public bool getIsValid()
        {
            return this._isInvalid;
        }

        // Break up the header data.
        private void init()
        {
            this.setTime();
            this.setDate();
            this.setFileIndex();
        }

        // Break the four-byte time value into a block header and H-M-S.
        private void setTime()
        {
            // Convert the time bytes into an integer.
            int time = this.bigEndianArrayToInt32(this._data,0);
            // Look for the first bit that sets the header as the start of a recording session.
            this._isFirstBlock = ((time & 0x80000000) > 0);

            // Get the raw time, without the start bit and convert to a string.
            String timeString = this.intToSixDigitString(time & 0x7fffffff);

            // Break it into its constituent parts.
            this._hour = Int32.Parse(timeString.Substring(0,2));
            this._minute = Int32.Parse(timeString.Substring(2, 2));
            this._second = Int32.Parse(timeString.Substring(4, 2));
        }

        // Break the four-byte date value into D-M-Y.
        private void setDate()
        {
            // Convert the date bytes into a parseable string.
            String dateString = this.intToSixDigitString(this.bigEndianArrayToInt32(this._data, 4));

            // Break it into its constituent parts.
            this._day = Int32.Parse(dateString.Substring(0, 2));
            this._month = Int32.Parse(dateString.Substring(2, 2));
            this._year = Int32.Parse(dateString.Substring(4, 2));
            // Invalid headers have a date/time of midnight Jan 6, 1980 (the gps epoch start). 
            if (this._year == 80)
            {
                this._isInvalid = true;
            }
        }

        // Extract the file index value. 
        private void setFileIndex()
        {
            this._fileIndex = this.bigEndianArrayToInt32(this._data, 8);
        }

        // Turn a six digit integer into a six-digit string.
        private String intToSixDigitString(int src)
        {
            // Make sure we get a leading zero for first byte values less than 10
            string fmt = "000000";
            return src.ToString(fmt);
        }

        private int bigEndianArrayToInt32(byte[] arr, int start)
        {
            int size = 4, countUp = 0, countDown = start + size - 1;
            byte[] tmpArr = new byte[size];
            while (countUp < size)
            {
                tmpArr[countUp++] = arr[countDown--];
            }

            return BitConverter.ToInt32(tmpArr, 0);
        }

        /// <summary>
        /// Output a nice ISO date/time string. If the value ends with a star, it's the start of a block. 
        /// </summary>
        /// <returns>The UTC date/time in ISO format</returns>
        public override string ToString()
        {
            string twoZeroFmt = "00";
            string fourZeroFmt = "0000";
            string sep = "-";
            string colon = ":";

            int year = this._year + (this._year == 80 ? 1900 : 2000);

            return year.ToString(fourZeroFmt) + sep + this._month.ToString(twoZeroFmt) + this._day.ToString(twoZeroFmt) + "T" +
                        this._hour.ToString(twoZeroFmt) +  colon + this._minute.ToString(twoZeroFmt) + colon + this._second.ToString(twoZeroFmt) + "Z"
                        + (this._isFirstBlock?"*":"");
        }
    }
}
