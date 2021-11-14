using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSlice.Core.Factory;
using DataSlice.Core.Settings;
using DataSlice.Core.Transfer;
using DataSlice.Core.Utils;

namespace DataSlice.Core.Databackup
{
    public class DatabaseBackupService : IDatabaseBackupService
    {
        private readonly IDatabasesToSubsetSettings _databasesToSubsetSettings;

        private readonly IAppSettings _appSettings;

        private readonly IAppLogger _appLogger;
        public DatabaseBackupService(IDatabasesToSubsetSettings databasesToSubsetSettings, IAppSettings appSettings, IAppLogger appLogger)
        {
            _databasesToSubsetSettings = databasesToSubsetSettings;

            _appSettings = appSettings;

            _appLogger = appLogger;
        }

        public void BackupDatabases(string databaseNames)
        {
            List<DatabaseToSubset> databaseInformation = new List<DatabaseToSubset>();

            if (databaseNames.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                databaseInformation = _databasesToSubsetSettings.DatabaseList.Where(u => u.Ignore == false).ToList();
            }
            else
            {
                var names = databaseNames.Split(',').Select(u => u.Trim()).ToList();

                databaseInformation =
                    _databasesToSubsetSettings.DatabaseList.Where(
                        u => names.Contains(u.Name, StringComparer.OrdinalIgnoreCase)).ToList();

            }

            if (!databaseInformation.Any())
            {
                Info("No databases found to backup from parameters given. Parameter = {0}", databaseNames);
                return;
            }


            try
            {
                foreach (var databaseInfo in databaseInformation)
                {
                    BackupDatabase(databaseInfo);
                    Info("Completed backing up database {0} ", databaseInfo.Name);
                }
            }
            catch (Exception ex)
            {
                _appLogger.Error("Error backing up database. " +  ex.Message, ex);
            }
        }


        private void BackupDatabase(DatabaseToSubset dataBaseInformation)
        {
            Info("Backing up {0} ", dataBaseInformation.Name);
            const string backUpCommand = @"BACKUP DATABASE [{0}] TO  DISK = N'{1}' 
WITH
NOFORMAT,
NOINIT,
NAME = N'[{0}]-Full Database Backup', 
SKIP, 
NOREWIND, 
NOUNLOAD,  
STATS = 10
";
            string databaseName = dataBaseInformation.Name.Trim();
            string location = Path.Combine(_appSettings.DatabaseBackupLocation, databaseName+".bak");

            string query = String.Format(backUpCommand, dataBaseInformation.Name, location);

            using (SqlConnection connection = new SqlConnection(dataBaseInformation.Destination))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.CommandTimeout = _appSettings.BackupCommandTimeoutInSeconds;

                     connection.Open();

                     command.ExecuteNonQuery();
                }
            }
        }

        private void Info(string message, params object[] parameters)
        {
            _appLogger.Info(message, parameters);
            Console.WriteLine(message, parameters);
        }
    }
}
