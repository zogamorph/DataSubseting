using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core.Transfer
{
    //Transfer data from source to target based on query
    public class SqlServerTableTransfer : ISqlServerTableTransfer
    {
        private IAppSettings _appSettings;

        public Dictionary<TableExtract,string> SourceQueries { get; set; }

        public SqlServerTableTransfer(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public void ProcessTransfer(TableExtract tableInfo)
        {
            var sourceQuery = SourceQueries[tableInfo];

            using (SqlConnection connection = new SqlConnection(""))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sourceQuery, connection))
                {
                    command.CommandTimeout = _appSettings.CommandTimeOutInSeconds;

                    using (var sqlReader = command.ExecuteReader())
                    {
                        //bulk insert batch
                        using (SqlConnection destinationConnection = new SqlConnection(""))
                        {
                            // open the connection
                            destinationConnection.Open();

                            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection, SqlBulkCopyOptions.KeepNulls| SqlBulkCopyOptions.KeepIdentity|SqlBulkCopyOptions.TableLock, null))
                            {
                                MapColumns(bulkCopy, sqlReader);
                                bulkCopy.DestinationTableName = String.Format("{0}.{1}", tableInfo.Schema, tableInfo.TableName);
                                bulkCopy.BatchSize = _appSettings.BulkInsertBatch;
                                bulkCopy.BulkCopyTimeout = _appSettings.BulkCopyTimeout;
                                bulkCopy.EnableStreaming = true;
                                bulkCopy.NotifyAfter = _appSettings.BulkInsertBatch;
                                bulkCopy.SqlRowsCopied += BulkCopy_SqlRowsCopied;
                                bulkCopy.WriteToServer(sqlReader);
                             
                            }
                        }
                        //map columns
                    }
                }
            }
        }

        private void BulkCopy_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
           Console.WriteLine("Bulk copied : {0} rows", e.RowsCopied);
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
