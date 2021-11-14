using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataSlice.Core.SchemaGeneration
{
    public class ModelMerge : IModelMerge
    {
        public void Merge(string source, string destination, string newFile)
        {
            string sourceModelText = File.ReadAllText(source);

            DataExtractModel sourceModel = JsonConvert.DeserializeObject<DataExtractModel>(sourceModelText);

            string destModelText = File.ReadAllText(destination);

            DataExtractModel destModel = JsonConvert.DeserializeObject<DataExtractModel>(destModelText);

            foreach (var table in sourceModel.Tables)
            {
                var destTable =
                    destModel.Tables.SingleOrDefault(
                        u =>
                            u.TableName.Equals(table.TableName, StringComparison.OrdinalIgnoreCase) &&
                            u.Schema.Equals(table.Schema, StringComparison.OrdinalIgnoreCase));

                if (destTable != null)
                {
                    if (table.Criteria.StaticCriteria != null && table.Criteria.StaticCriteria.Any())
                    {
                        destTable.Criteria.StaticCriteria = table.Criteria.StaticCriteria;
                    }

                    if (table.Limit != null)
                    {
                        if (table.Limit.TopN.HasValue)
                        {
                            destTable.Limit.TopN = table.Limit.TopN;
                        }

                        if (table.Limit.SortColumns != null && table.Limit.SortColumns.Any())
                        {
                            destTable.Limit.SortColumns = table.Limit.SortColumns;
                        }
                    }

                   destModel.Modified = DateTime.UtcNow;
                }
            }

            string schemaAsJson = JsonConvert.SerializeObject(destModel, Formatting.Indented);

            File.WriteAllText(newFile, schemaAsJson);


        }
    }
}
