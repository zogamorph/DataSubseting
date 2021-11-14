namespace DataSlice.Core.Databackup
{
    public interface IDatabaseBackupService
    {
        void BackupDatabases(string databaseNames);
    }
}