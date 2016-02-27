using System;

namespace kimandtodd.DG200CSharp.commandresults.resultitems
{
    public class DGTrackHeader
    {
        private int _year;
        private int _month;
        private int _day;
        private int _hour;
        private int _minute;
        private int _second;

        private int _fileIndex;

        private bool _isFirstBlock;

        private byte[] _data;

        public DGTrackHeader(byte[] headerData)
        {
            this._data = headerData;
        }
    }
}
