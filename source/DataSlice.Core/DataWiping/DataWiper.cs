using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSlice.Core.Settings;
using DataSlice.Core.Utils;

namespace DataSlice.Core
{
    public class DataWiper : IDataWiper
    {
        private readonly IDatabasesToSubsetSettings _databasesToSubsetSettings;

        private readonly IAppSettings _appSettings;

        private readonly IAppLogger _logger;

        private string[] _commands = new[] { "EXEC sp_MSforeachtable 'DISABLE TRIGGER ALL ON ?';", "EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'", "EXEC sp_MSforeachtable 'SET QUOTED_IDENTIFIER ON; DELETE FROM ?'", "EXEC sp_MSforeachtable 'ALTER TABLE ? CHECK CONSTRAINT ALL'", "EXEC sp_MSforeachtable 'ENABLE TRIGGER ALL ON ?'" };

        public DataWiper(IDatabasesToSubsetSettings databasesToSubsetSetting, IAppSettings appSettings, IAppLogger logger)
        {
            _databasesToSubsetSettings = databasesToSubsetSetting;

            _appSettings = appSettings;

            _logger = logger;
        }

        public void WipeDatabase(string databaseNames)
        {
            List<DatabaseToSubset> databaseInformation = new List<DatabaseToSubset>();

            if (databaseNames.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                databaseInformation = _databasesToSubsetSettings.DatabaseList.Where(u => u.Ignore == false).OrderBy(u => u.Order).ToList();
            }
            else
            {
                var names = databaseNames.Split(',').Select(u => u.Trim()).ToList();

                databaseInformation =
                    _databasesToSubsetSettings.DatabaseList.Where(
                        u => names.Contains(u.Name, StringComparer.OrdinalIgnoreCase)).OrderBy(u=>u.Order).ToList();

            }

            foreach (var database in databaseInformation)
            {
                _logger.Info("Wiping database {0}", database.Name);
                Console.WriteLine("Wiping database {0}", database.Name);

                WipeDatabaseData(database.Destination);
                _logger.Info("Wiping Database {0} completed", database.Name);
                Console.WriteLine("Wiping Database {0} completed", database.Name);
            }
        }

        private void WipeDatabaseData(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var commandText in _commands)
                {
                    using (SqlCommand command = new SqlCommand(commandText, connection))
                    {
                        command.CommandTimeout = _appSettings.CommandTimeOutInSeconds;

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
