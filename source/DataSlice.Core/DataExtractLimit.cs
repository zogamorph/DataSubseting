using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core
{
    public class DataExtractLimit
    {
        public int? TopN { get; set; }

        public List<string> SortColumns { get; set; }
    }
}
