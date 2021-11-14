using System.Collections.Generic;

namespace DataSlice.Core.Settings
{
    public interface IDatabasesToSubsetSettings
    {
        List<DatabaseToSubset> DatabaseList { get; }
    }
}