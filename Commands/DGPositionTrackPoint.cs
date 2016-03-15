
namespace kimandtodd.DG200CSharp.commandresults.resultitems
{
    public class DGPositionTrackPoint : BaseDGTrackPoint
    {
        public DGPositionTrackPoint(byte[] input) : base(input)
        {
            this._format = BaseDGTrackPoint.FORMAT_POSITION;
        }
    }
}
