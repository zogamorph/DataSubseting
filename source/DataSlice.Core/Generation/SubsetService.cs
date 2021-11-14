using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSlice.Core.Factory;
using DataSlice.Core.Settings;
using DataSlice.Core.Transfer;
using DataSlice.Core.Utils;
using Newtonsoft.Json;

namespace DataSlice.Core.Generation
{
    public class SubsetService : ISubsetService
    {
        private readonly IDatabasesToSubsetSettings _databasesToSubsetSettings;

        private readonly IAppSettings _appSettings;

        private readonly IAppLogger _appLogger;

        private readonly IServiceLocator _serviceLocator;

        public SubsetService(IDatabasesToSubsetSettings databasesToSubsetSettings, IAppSettings appSettings, IAppLogger appLogger, IServiceLocator serviceLocator)
        {
            _databasesToSubsetSettings = databasesToSubsetSettings;

            _appSettings = appSettings;

            _appLogger = appLogger;

            _serviceLocator = serviceLocator;
        }

        public void GenerateSubSet(string databases, string modelFolderLocation)
        {
            List<DatabaseToSubset> databasesToSubset = new List<DatabaseToSubset>();

            if (databases.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                databasesToSubset =
                    _databasesToSubsetSettings.DatabaseList.Where(u => u.Ignore == false).OrderBy(u => u.Order).ToList();
            }
            else
            {
                var tempList = databases.Split(',').Select(u=>u.Trim()).ToList();

                databasesToSubset =
                    _databasesToSubsetSettings.DatabaseList.Where(u => tempList.Contains(u.Name, StringComparer.OrdinalIgnoreCase)).OrderBy(u => u.Order).ToList();
            }

            if (!databasesToSubset.Any())
            {
                throw new InvalidParameterException("No databases found to subset. Please check database names in config. Parameter = " + databases);
            }

            foreach (var database in databasesToSubset)
            {

                DatabaseSubset databaseSubSet = new DatabaseSubset(_serviceLocator.Resolve<IMigrationQueryGenerator>(),
                    _serviceLocator.Resolve<IIndexManager>(), _appSettings, _appLogger);

                databaseSubSet.DatabaseToSubset = database;

                databaseSubSet.Model = GetModel(database, modelFolderLocation);

                databaseSubSet.Schema = GetSchema(database, modelFolderLocation);

                databaseSubSet.Subset();

            }
        }

        private DataExtractModel GetModel(DatabaseToSubset databaseToSubset, string modelFolderLocation)
        {
            var fileName = String.Format("{0}.model.json", databaseToSubset.Name);

            var fileLocation = Path.Combine(modelFolderLocation, fileName);

            string modelText = File.ReadAllText(fileLocation);

             DataExtractModel model = JsonConvert.DeserializeObject<DataExtractModel>(modelText);

            return model;
        }

        private Schema GetSchema(DatabaseToSubset databaseToSubset, string modelFolderLocation)
        {
            var fileName = String.Format("{0}.schema.json", databaseToSubset.Name);

            var fileLocation = Path.Combine(modelFolderLocation, fileName);

            string modelText = File.ReadAllText(fileLocation);

            Schema model = JsonConvert.DeserializeObject<Schema>(modelText);

            return model;
        }
    }
}
