namespace DataSlice.Core.Generation
{
    public interface ISubsetService
    {
        void GenerateSubSet(string databases, string modelFolderLocation);
    }
}