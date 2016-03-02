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

        // Headers have an index value that corresponds to a track index.
        private int _fileIndex;

        // A first block indicates a session start (any time a track is started without the previous one filling up)
        private bool _isFirstBlock;

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

        // Getters for the values.
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

        public int getFileIndex()
        {
            return this._fileIndex;
        }

        public bool getIsFirstBlock()
        {
            return this._isFirstBlock;
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
            int size = 4, countUp = 0, countDown = 3;
            byte[] tmpArr = new byte[size];
            while (countUp < size)
            {
                tmpArr[countUp++] = arr[countDown--];
            }

            return BitConverter.ToInt32(tmpArr, 0);
        }
    }
}
