using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core
{
   public  class SchemaExtractionException : Exception
    {
        public SchemaExtractionException(string message) : base(message)
        {

        }

    }
}
