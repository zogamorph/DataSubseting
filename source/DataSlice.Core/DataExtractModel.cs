using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core
{
    public class DataExtractModel
    {
        public List<ExtractParameter> GlobalParameters { get; set; }

        public List<TableExtract> Tables { get; set; }

        public DateTime Modified { get; set; }

        public DataExtractModel()
        {
            Tables = new List<TableExtract>();

            GlobalParameters = new List<ExtractParameter>();
        }
    }
}
