using DataSlice.CommandLineParsing;

namespace DataSlice.Handlers
{
    public interface ICommandHandler
    {
        bool Handle(CommandLineDictionary commandLineDictionary);
    }
}