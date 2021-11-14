using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core.Generation
{
    public class SqlMigrationQueryGenerator : IMigrationQueryGenerator
    {
        public Dictionary<TableExtract, string> GenerateSourceQueries(DataExtractModel model, Schema schema)
        {
             SourceQueryGenerator generator = new SourceQueryGenerator(model, schema);

            Dictionary<TableExtract, string> generatedQueries = new Dictionary<TableExtract, string>();

            foreach (var table in model.Tables)
            {
                string query = generator.GenerateSourceQuery(table.TableName, table.Schema, 1);

                generatedQueries.Add(table, query);
            }

            return generatedQueries;

        }
    }
}
