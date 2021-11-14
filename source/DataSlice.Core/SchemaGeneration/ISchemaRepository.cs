using System.Collections.Generic;

namespace DataSlice.Core.SchemaGeneration
{
    public interface ISchemaRepository
    {
        List<Column> GetColumns(Table table, string databaseName, string connectionString);
        List<Table> GetAllTables(string databaseName, string connectionString);
        List<string> GetDependencies(Table table, string connectionString);
        List<Dependency> GetReferencedTables(Table table, string connectionString);
    }
}