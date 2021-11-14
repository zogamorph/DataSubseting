using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core
{
    public class AppSettings : IAppSettings
    {
        //private string _currentDatabase = "SoftwareManagement";
       // public string SourceDatabaseConnectionString { get; private set; }

       // public string TargetDatabaseConnectionString { get; private set; }

        public int CommandTimeOutInSeconds { get; private set; }

        public int BulkInsertBatch { get; private set; }

        public int BulkCopyTimeout { get; private set; }

        public int MaxThreadsPerDatabase { get; private set; }

        public string DatabaseBackupLocation { get; private set; }

        public int BackupCommandTimeoutInSeconds { get; private set; }

        public AppSettings()
        {
            //string[] temp = ConfigurationManager.AppSettings[_currentDatabase + "DatabaseValues"].Split(',');

           // SourceDatabaseConnectionString =
             //   String.Format(ConfigurationManager.ConnectionStrings[temp[0] + "SourceDatabase"].ConnectionString, "SoftwareManagement");
                
            //TargetDatabaseConnectionString = String.Format(ConfigurationManager.ConnectionStrings[temp[0] + "TargetDatabase"].ConnectionString, temp[1]);

            CommandTimeOutInSeconds = Int32.Parse(ConfigurationManager.AppSettings["CommandTimeoutInSeconds"]);

            BulkInsertBatch = Int32.Parse(ConfigurationManager.AppSettings["BulkCopyBatchSize"]);

            BulkCopyTimeout = Int32.Parse(ConfigurationManager.AppSettings["BulkInsertTimeoutInSeconds"]);

            MaxThreadsPerDatabase = Int32.Parse(ConfigurationManager.AppSettings["MaxThreadsPerDatabase"]);

            DatabaseBackupLocation = ConfigurationManager.AppSettings["DatabaseBackupDirectory"];

            BackupCommandTimeoutInSeconds = Int32.Parse(ConfigurationManager.AppSettings["BackupCommandTimeoutInSeconds"]);

        }
    }
}
