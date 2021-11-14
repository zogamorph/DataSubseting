using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core
{
    public class Dependency
    {
        public string FromTableName { get; set; }

        public string FromColumn { get; set; }

        public string FromTableSchema { get; set; }

        public string ToTableName { get; set; }

        public string ToTableSchema { get; set; }

        public string ToColumnName { get; set; }
    }
}
