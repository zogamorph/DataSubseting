using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataSlice.Core.Settings;

namespace DataSlice.Core.Transfer
{
    public interface IAsyncSqlServerTableTransfer
    {
        Task ProcessTransferAsync(DatabaseToSubset databaseToSubSet, TableExtract tableInfo, Dictionary<TableExtract, string> sourceQueries, CancellationTokenSource tokenSource);
    }
}