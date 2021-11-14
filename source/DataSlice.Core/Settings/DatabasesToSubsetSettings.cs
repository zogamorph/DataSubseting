using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core.Settings
{
    public class DatabasesToSubsetSettings : IDatabasesToSubsetSettings
    {
        public List<DatabaseToSubset> DatabaseList { get; private set; }

        public DatabasesToSubsetSettings()
        {

            DatabaseList = new List<DatabaseToSubset>();

            DatabasesToSubsetConfiguration config =
                (DatabasesToSubsetConfiguration)
                    System.Configuration.ConfigurationManager.GetSection("database-subset/databases");

            foreach (var item in config.DatabasesToSubSet)
            {
                var data = item as DatabaseToSubsetElement;

                DatabaseList.Add(new DatabaseToSubset()
                {
                    Name = data.Name,
                    Source = data.Source,
                    Destination = data.Destination,
                    Order = data.Order,
                    Ignore = data.Ignore,
                    IgnoreTables = ConvertToList(data.IgnoreTable)
                   
                });
            }
        }

        private static List<string> ConvertToList(string data)
        {
            List<string> returnVal = new List<string>();

            if (!String.IsNullOrWhiteSpace(data))
            {
                string[] tables = data.Split(',');

                returnVal.AddRange(tables.Select(u => u.Replace("[", String.Empty).Replace("]", String.Empty).Trim()));
            }

            return returnVal;
        }
    }
}
