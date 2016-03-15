using kimandtodd.DG200CSharp.commands;

namespace kimandtodd.DG200CSharp.commandresults
{
    public interface ICommandResult
    {
        void addResultBuffer(CommandBuffer c);
    }
}
