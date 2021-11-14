using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core
{
    public class Table
    {
        public string Name { get; set; }

        public string Schema { get; set; }

        public List<Column> Columns { get; set; }

        //public List<string> Dependencies { get; set; }

        public List<Dependency> Dependencies { get; set; }

        //public string Filter { get; set; }
    }
}
