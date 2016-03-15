
namespace kimandtodd.DG200CSharp.commandresults
{
    public interface ITrackHeaderResult : ICommandResult
    {
        int getNextTrackHeaderId();
        bool requestAdditionalSession();
    }
}
