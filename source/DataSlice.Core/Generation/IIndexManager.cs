namespace DataSlice.Core.Generation
{
    public interface IIndexManager
    {
        void DisableAllIndexes();
        void DisableAllContraints();
        void EnableAllContraints();
        void EnableAllIndexes();

         DataExtractModel Model { get; set; }

         string DestinationConnectionString { get; set; }
    }
}