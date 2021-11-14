using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core.DataWiping
{
    public class DataWipeManager : IDataWipeManager
    {
        private readonly IDataWiper _dataWiper;

        public DataWipeManager(IDataWiper dataWiper)
        {
            _dataWiper = dataWiper;
        }

        public void WipeDatabase(string databaseNames)
        {
            _dataWiper.WipeDatabase(databaseNames);
        }
    }
}
