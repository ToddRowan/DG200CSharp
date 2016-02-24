
namespace kimandtodd.DG200CSharp
{
    interface ICommandData
    {
        byte[] getCommandData();
        int getCorrectedExpectedBytes();
        int getExpectedSessionCount();
    }
}
