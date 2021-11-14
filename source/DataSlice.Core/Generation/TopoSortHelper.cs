using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSlice.Core.Utils;

namespace DataSlice.Core.Generation
{
    public class TopoSortHelper
    {
        public static DataExtractModel GetTopologicallySortedModel(DataExtractModel model, Schema schema)
        {
            Func<Table, List<Table>> dependencyAction = u =>
            {
                List<Table> dep = new List<Table>();

                foreach (var depen in u.Dependencies)
                {
                  
                    var table =
                        schema.Tables.First(
                            x =>
                                x.Schema.Equals(depen.ToTableSchema, StringComparison.OrdinalIgnoreCase) &&
                                x.Name.Equals(depen.ToTableName, StringComparison.OrdinalIgnoreCase));

                    if (!dep.Contains(table))
                    {
                        dep.Add(table);
                    }
                }

                return dep;
            };

            var sortData = TopologicalSort.SortTopologically(schema.Tables, dependencyAction, false);

            DataExtractModel model2 = new DataExtractModel();
            model2.GlobalParameters = model.GlobalParameters;
            model2.Modified = model.Modified;
            model2.Tables = new List<TableExtract>();

            foreach (var sortedTable in sortData)
            {
                string name = sortedTable.Name;
                string schemaName = sortedTable.Schema;

                try
                {
                   // Console.WriteLine("{0}.{1}", sortedTable.Schema, sortedTable.Name);
                    var table =
                        model.Tables.Single(
                            x =>
                                x.Schema.Equals(sortedTable.Schema, StringComparison.OrdinalIgnoreCase) &&
                                x.TableName.Equals(sortedTable.Name, StringComparison.OrdinalIgnoreCase));

                    model2.Tables.Add(table);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Schema / Model descrepency. Schema={0}, Table={1}. Exception={2}, StackTrace={3}  ", name, schemaName, ex.Message, ex.StackTrace);
                    throw;
                }
               
            }

            return model2;
        }
    }
}
