namespace DataSlice.Core.SchemaGeneration
{
    public interface IModelMerge
    {
        void Merge(string source, string destination, string newFile);
    }
}