using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core.Utils
{
    // Pardon the name and french
    public static class StringyTypeHelper
    {
        private static readonly string[] ShouldEnclose = {
            "image", "text", "uniqueidentifier", "date", "time", "datetime2", "datetimeoffset", "smalldatetime",
            "datetime", "ntext", "hierarchyid", "geometry", "geography", "varbinary", "varchar", "binary", "char",
            "timestamp", "nvarchar", "nchar", "xml"
        };

        private static readonly string[] NotEnclose =  { "tinyint", "smallint", "int", "real", "money", "float", "bit", "decimal", "numeric", "smallmoney", "bigint" , "number"};

        public static bool IsStringy(string data)
        {
            data = data.ToLower().Trim();

            if (ShouldEnclose.Contains(data, StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }

            if (NotEnclose.Contains(data, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            throw new DataException("Unknown Data Type : " + data);
        }

    }
}
