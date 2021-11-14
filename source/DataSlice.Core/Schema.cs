using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core
{
    public class Schema
    {
        //public  List<Tuple<string,string>> GlobalParameters { get; set; }

        public Schema()
        {
           

        }

        public List<Table> Tables { get; set; }
    }
}
