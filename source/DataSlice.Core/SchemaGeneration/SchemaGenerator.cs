using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DataSlice.Core.Settings;
using Newtonsoft.Json;

namespace DataSlice.Core.SchemaGeneration
{
    public class SchemaGenerator : ISchemaGenerator
    {
        private readonly IDatabasesToSubsetSettings _databasesToSubsetSettings;

        private readonly IAppSettings _appSettings;

        private readonly ISchemaRepository _schemaRepository;

        public SchemaGenerator(IDatabasesToSubsetSettings databasesToSubsetSettings, IAppSettings appSettings, ISchemaRepository schemaRepository)
        {
            _databasesToSubsetSettings = databasesToSubsetSettings;

            _appSettings = appSettings;

            _schemaRepository = schemaRepository;
        }

        public void GenerateSchema(string database, string destinationFolder)
        {
            var databaseToSubsets = new List<DatabaseToSubset>();

            var currentDatabaseList = _databasesToSubsetSettings.DatabaseList;

            if (database.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                databaseToSubsets = currentDatabaseList;
            }
            else
            {
                databaseToSubsets =
                    currentDatabaseList.Where(u => u.Name.Equals(database.Trim(), StringComparison.OrdinalIgnoreCase))
                        .ToList();
            }

            if (!databaseToSubsets.Any())
            {
                throw new SchemaExtractionException("No databases found to generate schema. Check spelling and config");
            }

            foreach (var databaseInfo in databaseToSubsets)
            {
                Console.WriteLine("Generating model for {0}", databaseInfo.Name);
                var schemaInfo = GenerateSchema(databaseInfo);

                var destinationSchemaFile = Path.Combine(destinationFolder, String.Format("{0}.schema.json", databaseInfo.Name));

                var destinationModelFile = Path.Combine(destinationFolder, String.Format("{0}_generated.model.json", databaseInfo.Name));

                string schemaAsJson = JsonConvert.SerializeObject(schemaInfo.Item1, Formatting.Indented);

                File.WriteAllText(destinationSchemaFile, schemaAsJson);

                string modelAsJson = JsonConvert.SerializeObject(schemaInfo.Item2, Formatting.Indented);

                File.WriteAllText(destinationModelFile, modelAsJson);
                Console.WriteLine("Completed generating for {0}", databaseInfo.Name);
            }

        }

        private Tuple<Schema, DataExtractModel> GenerateSchema(DatabaseToSubset databaseInfo)
        {
            DataExtractModel dataExtractModel = new DataExtractModel();

            Schema schema = new Schema { Tables = new List<Table>() };

            schema.Tables = _schemaRepository.GetAllTables(databaseInfo.Name, databaseInfo.Source);

            List<Table> tablesToRemove = new List<Table>();

            foreach (var table in schema.Tables)
            {
                string fullName = String.Format("{0}.{1}", table.Schema, table.Name).ToLowerInvariant();

                if (databaseInfo.IgnoreTables.Contains(fullName, StringComparer.OrdinalIgnoreCase))
                {
                    tablesToRemove.Add(table);
                }
            }

            //ignore stuff
     
            foreach (var item in databaseInfo.IgnoreTables)
            {
                if (item.StartsWith("pattern", StringComparison.OrdinalIgnoreCase))
                {
                    var pattern = item.Trim().Replace("pattern", String.Empty).Replace(":", String.Empty).Trim();

                    if (pattern.StartsWith("schema", StringComparison.OrdinalIgnoreCase))
                    {
                        var schemaToIgnore = pattern.Replace("schema", String.Empty).Replace("=", String.Empty);

                        foreach (var table in schema.Tables)
                        {
                            if (table.Schema.Equals(schemaToIgnore, StringComparison.OrdinalIgnoreCase))
                            {
                                tablesToRemove.Add(table);
                            }
                        }
                    }
                    else if (pattern.StartsWith("TableNameStartsWith", StringComparison.OrdinalIgnoreCase))
                    {
                        var patternToApply = pattern.Replace("TableNameStartsWith", String.Empty).Replace("=", String.Empty);

                        foreach (var table in schema.Tables)
                        {
                            if (table.Name.StartsWith(patternToApply, StringComparison.OrdinalIgnoreCase))
                            {
                                tablesToRemove.Add(table);
                            }
                        }
                    }
                }
            }

            foreach (var table in tablesToRemove)
            {
                schema.Tables.Remove(table);
            }

            foreach (var table in schema.Tables)
            {
                var columns = _schemaRepository.GetColumns(table, databaseInfo.Name, databaseInfo.Source);

                table.Columns = columns;

                table.Dependencies = _schemaRepository.GetReferencedTables(table, databaseInfo.Source);

                dataExtractModel.Tables.Add(new TableExtract()
                {
                    Schema = table.Schema,
                    TableName = table.Name
                });
            }

            return new Tuple<Schema, DataExtractModel>(schema, dataExtractModel);
        }
    }
}
