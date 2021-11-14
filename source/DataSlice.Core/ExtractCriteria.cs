using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core
{
    public class ExtractCriteria
    {
        public List<string> StaticCriteria { get; set; }

        public List<string> DynamicCriteria { get; set; }

        public ExtractCriteria()
        {
            StaticCriteria = new List<string>();

            DynamicCriteria = new List<string>();
        }
    }
}
