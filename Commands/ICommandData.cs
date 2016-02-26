using kimandtodd.DG200CSharp.commandresults;

namespace kimandtodd.DG200CSharp.commands
{
    interface ICommandData
    {
        byte[] getCommandData();
        void addCommandResultData(byte[] bytes, int byteCount);
        int getExpectedSessionCount();
        BaseCommandResult getLastResult();
    }
}
