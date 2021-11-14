using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSlice.CommandLineParsing;
using DataSlice.Core;
using DataSlice.Core.Generation;

namespace DataSlice.Handlers
{
    class SubsetHandler : ICommandHandler
    {
        private const string SubsetArg = "subset";

        private const string ModelFolderArg = "modelFolder";

        public bool Handle(CommandLineDictionary commandLineDictionary)
        {
            var extractedCommands = ExtractSubsetCommand(commandLineDictionary);

            bool canHandle = extractedCommands.Item1;

            if (canHandle)
            {
                var subsetService = AppContainer.Resolve<ISubsetService>();

                subsetService.GenerateSubSet(extractedCommands.Item2, extractedCommands.Item3);
            }

            return canHandle;
        }

        private Tuple<bool, string, string> ExtractSubsetCommand(CommandLineDictionary commandLineDictionary)
        {
            bool isSubSetCommand = commandLineDictionary.ContainsKey(SubsetArg);

            string databaseNames = null;

            string folder = null;

            if (isSubSetCommand)
            {
                databaseNames = commandLineDictionary[SubsetArg];

                if (String.IsNullOrWhiteSpace(databaseNames))
                {
                    throw new InvalidParameterException("All or DatabaseName list expected with subset flag.i.e subset = All");
                }

                folder = commandLineDictionary[ModelFolderArg];

                if (String.IsNullOrWhiteSpace(folder))
                {
                    throw new InvalidParameterException("Model folder location expected with subset flag.i.e modelFolder=c:temp");
                }
            }

            return new Tuple<bool, string, string>(isSubSetCommand, databaseNames, folder);

        }
    }
}
