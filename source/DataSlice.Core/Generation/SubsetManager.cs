//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using DataSlice.Core.Transfer;
//using DataSlice.Core.Utils;
//using Newtonsoft.Json;

//namespace DataSlice.Core.Generation
//{
//    public class SubsetManager
//    {
//        public void Process()
//        {
//            string modelText = File.ReadAllText(@"C:\DevExamp\DataSlice\Models\SoftwareManagement-model.json");

//            DataExtractModel model = JsonConvert.DeserializeObject<DataExtractModel>(modelText);

//            string schemaText = File.ReadAllText(@"C:\DevExamp\DataSlice\Models\SoftwareManagement-schema.json");

//            Schema schema = JsonConvert.DeserializeObject<Schema>(schemaText);

//            Func<Table, List<Table>> dependencyAction = u =>
//            {
//                List<Table> dep = new List<Table>();

//                foreach (var depen in u.Dependencies)
//                {
//                    var table =
//                        schema.Tables.First(
//                            x =>
//                                x.Schema.Equals(depen.ToTableSchema, StringComparison.OrdinalIgnoreCase) &&
//                                x.Name.Equals(depen.ToTableName, StringComparison.OrdinalIgnoreCase));

//                    if (!dep.Contains(table))
//                    {
//                        dep.Add(table);
//                    }
//                }
                
//                return dep;
//            };

//            var sortData = TopologicalSort.SortTopologically(schema.Tables, dependencyAction, true);

//            DataExtractModel model2 = new DataExtractModel();
//            model2.GlobalParameters = model.GlobalParameters;
//            model2.Modified = model.Modified;
//            model2.Tables = new List<TableExtract>();

//            foreach (var sortedTable in sortData)
//            {
//                var table =
//                        model.Tables.Single(
//                            x =>
//                                x.Schema.Equals(sortedTable.Schema, StringComparison.OrdinalIgnoreCase) &&
//                                x.TableName.Equals(sortedTable.Name, StringComparison.OrdinalIgnoreCase));

//                model2.Tables.Add(table);
//            }

            
//            MigrationQueryGenerator generator = new MigrationQueryGenerator(model2, schema);

//            var result = generator.GenerateSourceQueries();

//            WriteQueriesToFile(result);

//            //return;

//            AppSettings appSettings = new AppSettings();

//            SqlServerTableTransfer transfer = new SqlServerTableTransfer(appSettings);

//            transfer.SourceQueries = result;

//            //var tableToTransfer = model2.Tables.Single(u => u.TableName.Equals("BitMaskTable", StringComparison.OrdinalIgnoreCase));

//            //relax constaints

//            IndexManager manager = new IndexManager(appSettings, model2);

//            manager.DisableAllContraints();

//          //  manager.DisableAllIndexes();



//            //transfer.ProcessTransfer(tableToTransfer);

//            foreach (var table in model2.Tables)
//            {
//                Console.WriteLine("Starting table " + table.TableName);
//                transfer.ProcessTransfer(table);
//                Console.WriteLine("Processed table " + table.TableName);
//            }

//           // manager.EnableAllIndexes();

//            manager.EnableAllContraints();

//        }

//        private void WriteQueriesToFile(Dictionary<TableExtract, string> result)
//        {
//            StringBuilder sb = new StringBuilder();

//            foreach (var table in result.Keys)
//            {
//                sb.AppendLine("--Table = " + table.TableName);
//                sb.AppendLine("--*********************");
//                sb.AppendLine(result[table]);
//                sb.AppendLine("----------------------");
//                sb.AppendLine("");
//                //  sb.AppendLine("");
//            }

//            File.WriteAllText(@"C:\temp\softwaremanagment-gen-query.txt", sb.ToString());
//        }
//    }
//}
