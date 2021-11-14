using System.Collections.Generic;

namespace DataSlice.Core.Generation
{
    public interface IMigrationQueryGenerator
    {
        Dictionary<TableExtract, string> GenerateSourceQueries(DataExtractModel model, Schema schema);
    }
}