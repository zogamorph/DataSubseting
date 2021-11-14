using System.Collections.Generic;

namespace DataSlice.Core.Transfer
{
    public interface ISqlServerTableTransfer
    {
        Dictionary<TableExtract, string> SourceQueries { get; set; }
        void ProcessTransfer(TableExtract tableInfo);
    }
}