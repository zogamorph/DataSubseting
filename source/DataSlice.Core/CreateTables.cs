using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace DataSlice.Core
{
    public class CreateTables
    {
        private Repository repository;

        public CreateTables()
        {
            repository = new Repository();
        }


        public void Run()
        {
            string databaseName = "SoftwareManagement";

            DataExtractModel dataExtractModel = new DataExtractModel();

            Schema schema = new Schema {Tables = new List<Table>()};

            schema.Tables = repository.GetAllTables(databaseName);

            string ignoreList = "dbo.temp_ProductPref,dbo.temp_RecommendationIdMap,dbo.temp_RecommendationIdRedeploymentTracking,dbo.DmdPacks_Feb15,dbo.ProductMappings_071116".ToLowerInvariant();

            List<string> ignore = ignoreList.Split(',').ToList();

            List<Table> tablesToRemove = new List<Table>();

            foreach (var table  in schema.Tables)
            {
                string fullName = String.Format("{0}.{1}", table.Schema, table.Name).ToLowerInvariant();

                if (ignore.Contains(fullName))
                {
                    tablesToRemove.Add(table);
                }
            }

            foreach (var table in tablesToRemove)
            {
                schema.Tables.Remove(table);
            }

            foreach (var table in schema.Tables)
            {
                var columns = repository.GetColumns(table, databaseName);

                table.Columns = columns;

                table.Dependencies = repository.GetReferencedTables(table);

                dataExtractModel.Tables.Add(new TableExtract()
                {
                    Schema = table.Schema,
                    TableName = table.Name
                });
            }

            Serialize(schema, databaseName);

            Serialize(dataExtractModel, databaseName);
        }

        public void Serialize(Schema schema, string databaseName)
        {
            string text = JsonConvert.SerializeObject(schema, Formatting.Indented);

            string fileName = String.Format("{0}-schema.json", databaseName);

            File.WriteAllText(@"c:\temp\" + fileName, text);
        }

        public void Serialize(DataExtractModel model,string databaseName)
        {
            string text = JsonConvert.SerializeObject(model, Formatting.Indented);

            string fileName = String.Format("{0}-model.json", databaseName);

            File.WriteAllText(@"c:\temp\" + fileName, text);
        }



    }
}
