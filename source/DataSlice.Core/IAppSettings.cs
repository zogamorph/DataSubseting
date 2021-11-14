namespace DataSlice.Core
{
    public interface IAppSettings
    {

        int CommandTimeOutInSeconds { get; }
        int BulkInsertBatch { get; }

        int BulkCopyTimeout { get; }

        int MaxThreadsPerDatabase { get; }

        string DatabaseBackupLocation { get;  }

        int BackupCommandTimeoutInSeconds { get; }
    }
}