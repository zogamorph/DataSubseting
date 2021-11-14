using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core.Generation
{
    public class MigrationQueryGenerator 
    {

        private readonly DataExtractModel _dataExtractModel;

        private readonly Schema _schema;

        private SourceQueryGenerator _generator;

        public MigrationQueryGenerator(DataExtractModel model, Schema schema)
        {
            _dataExtractModel = model;

            _schema = schema;

            _generator = new SourceQueryGenerator(_dataExtractModel, _schema);
        }
        public Dictionary<TableExtract, string> GenerateSourceQueries()
        {
            Dictionary<TableExtract, string> generatedQueries = new Dictionary<TableExtract, string>();

            foreach (var table in _dataExtractModel.Tables)
            {
               string query = _generator.GenerateSourceQuery(table.TableName, table.Schema, 1);

                generatedQueries.Add(table, query);
            }

            return generatedQueries;
            
        }
    }
}
