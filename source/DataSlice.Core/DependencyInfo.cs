using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core
{
    public class DependencyInfo
    {
        public string TableWithForeignKey { get; set; }

        public string ForeignKeyColumn { get; set; }

        public string ParentTable { get; set; }

        public string ParentColumn { get; set; }
    }
}
