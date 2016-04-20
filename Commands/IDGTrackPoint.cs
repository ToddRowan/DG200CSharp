using System;

namespace kimandtodd.DG200CSharp.commandresults.resultitems
{
    public interface IDGTrackPoint
    {
        long getAltitude();
        float getSpeed();
        bool isWayPoint();
        Tuple<Int16, Double> getLatitude();
        Tuple<Int16, Double> getLongitude();
        DateTime getDateTime();
        UInt16 getTrackFormat();
    }
}
