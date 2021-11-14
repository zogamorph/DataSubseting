using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSlice.CommandLineParsing;
using DataSlice.Core;
using DataSlice.Core.DataWiping;

namespace DataSlice.Handlers
{
    class WipeDatabaseHandler : ICommandHandler
    {
        private const string WipeDatabasesCommandLineArg = "wipedatabases";

        public bool Handle(CommandLineDictionary commandLineDictionary)
        {
            bool wipeDatabasesCommand = commandLineDictionary.ContainsKey(WipeDatabasesCommandLineArg);

            if (wipeDatabasesCommand)
            {
                string wipeDatabaseParam = commandLineDictionary[WipeDatabasesCommandLineArg];

                if (String.IsNullOrWhiteSpace(wipeDatabaseParam))
                {
                    throw new InvalidParameterException("wipedatabases command must have an additional argument wipedatabases=all or wipedatabases=softwaremanagement");
                }

                if (wipeDatabasesCommand)
                {
                    var dataWipeManager = AppContainer.Resolve<IDataWipeManager>();

                    dataWipeManager.WipeDatabase(wipeDatabaseParam.Trim());
                }
            }

            


            return wipeDatabasesCommand;
        }
    }
}
