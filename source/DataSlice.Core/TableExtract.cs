using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core
{
    public class TableExtract
    {
        public string TableName { get; set; }

        public string Schema { get; set; }

        public ExtractCriteria Criteria { get; set; }

        public DataExtractLimit Limit { get; set; }

        public bool ImportAll { get; set; }

        public TableExtract()
        {
            Criteria = new ExtractCriteria();

            Limit = new DataExtractLimit();



        }
    }
}
