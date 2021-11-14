using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataSlice.Core.Settings;
using DataSlice.Core.Utils;

namespace DataSlice.Core.Transfer
{
    public class AsyncSqlServerTableTransfer : IAsyncSqlServerTableTransfer
    {
        private readonly IAppSettings _appSettings;
         
        private readonly IAppLogger _logger;

       // public Dictionary<TableExtract, string> SourceQueries { get; set; }

        public AsyncSqlServerTableTransfer(IAppSettings appSettings, IAppLogger logger)
        {
            _appSettings = appSettings;

            _logger = logger;
        }

        public async Task ProcessTransferAsync(DatabaseToSubset databaseToSubSet, TableExtract tableInfo, Dictionary<TableExtract, string> sourceQueries, CancellationTokenSource tokenSource)
        {
            Info("Starting table = {0}.{1} {2}", tableInfo.Schema, tableInfo.TableName, DateTime.Now.ToLongTimeString());

            var sourceQuery = sourceQueries[tableInfo];

            if (tokenSource.IsCancellationRequested)
            {
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(databaseToSubSet.Source))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(sourceQuery, connection))
                    {
                        command.CommandTimeout = _appSettings.CommandTimeOutInSeconds;

                        using (var sqlReader = await command.ExecuteReaderAsync(tokenSource.Token))
                        {
                            //bulk insert batch
                            using (SqlConnection destinationConnection = new SqlConnection(databaseToSubSet.Destination))
                            {
                                // open the connection
                                await destinationConnection.OpenAsync();

                                if (tokenSource.IsCancellationRequested)
                                {
                                    return;
                                }

                                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection, SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.TableLock, null))
                                {
                                    Info("Processing table = {0}.{1}", tableInfo.Schema, tableInfo.TableName);
                                    MapColumns(bulkCopy, sqlReader);
                                    bulkCopy.DestinationTableName = String.Format("[{0}].[{1}]", tableInfo.Schema, tableInfo.TableName);
                                    bulkCopy.BatchSize = _appSettings.BulkInsertBatch;
                                    bulkCopy.BulkCopyTimeout = _appSettings.BulkCopyTimeout;
                                    bulkCopy.EnableStreaming = true;
                                    bulkCopy.NotifyAfter = _appSettings.BulkInsertBatch;
                                    bulkCopy.SqlRowsCopied += BulkCopy_SqlRowsCopied;

                                    await bulkCopy.WriteToServerAsync(sqlReader, tokenSource.Token);

                                }

                                Info("Completed table = {0}.{1} {2}", tableInfo.Schema, tableInfo.TableName, DateTime.Now.ToLongTimeString());
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);

                if (!tokenSource.IsCancellationRequested)
                {
                     tokenSource.Cancel();
                 }
                throw;
            }
        }

        private void BulkCopy_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            //Console.WriteLine("Bulk copied : {0} rows", e.RowsCopied);
        }

        private void Info(string message, params object[] parameters)
        {
            _logger.Info(message, parameters);
            Console.WriteLine(message, parameters);
        }

        private void MapColumns(SqlBulkCopy copy, SqlDataReader reader)
        {
            var columns = new List<string>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns.Add(reader.GetName(i));
            }

            foreach (var col in columns)
            {
                if (!col.Equals("DmdIdString", StringComparison.OrdinalIgnoreCase))
                {
                    copy.ColumnMappings.Add(col, col);
                }
            }
        }
    }
}
