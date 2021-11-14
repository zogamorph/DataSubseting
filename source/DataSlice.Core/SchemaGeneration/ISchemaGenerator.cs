namespace DataSlice.Core.SchemaGeneration
{
    public interface ISchemaGenerator
    {
        void GenerateSchema(string database, string destinationFolder);
    }
}