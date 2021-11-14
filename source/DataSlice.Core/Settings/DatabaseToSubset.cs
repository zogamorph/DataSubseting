using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core.Settings
{
    public class DatabaseToSubset
    {
        public string Name { get; set; }

        public string Source { get; set; }

        public string Destination { get; set; }

        public int Order { get; set; }

        public bool Ignore { get; set; }

        public List<string> IgnoreTables { get; set; }
    }
}
