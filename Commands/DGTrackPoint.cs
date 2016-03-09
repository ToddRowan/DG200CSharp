using System;
using System.Collections.Generic;

namespace kimandtodd.DG200CSharp.commandresults.resultitems
{    
    class DGTrackPoint
    {
        private DateTime _dt;
        private Tuple<Int16, Double> _long;
        private Tuple<Int16, Double> _lat;
        private UInt32 _altitude;
        private UInt32 _speed;
        private UInt16 _format;

        private static UInt32 SPEED_MULTIPLIER = 100;
        private static UInt32 ALTITUDE_MULTIPLIER = 10000;
            
        private static UInt16 TRACK_LENGTH_NO_ALTITUDE = 20;
        private static UInt16 TRACK_LENGTH_WITH_ALTITUDE = 32;

        public static UInt16 FORMAT_POSITION = 0;
        public static UInt16 FORMAT_POSITION_DATE_SPEED = 1;
        public static UInt16 FORMAT_POSITION_DATE_SPEED_ALTITUDE = 2;
        

        public DGTrackPoint(byte[] input)
        {

        }
    }
}
