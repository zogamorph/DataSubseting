using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSlice.CommandLineParsing;
using DataSlice.Core;
using DataSlice.Core.SchemaGeneration;

namespace DataSlice.Handlers
{
    class GenerateSchemaHandler : ICommandHandler
    {
        private const string GenerateSchemaArg = "generateschema";

        private const string DestinationArg = "dest";
        public bool Handle(CommandLineDictionary commandLineDictionary)
        {
            var isGenerateSchemaCommand = IsGenerateSchemaCommand(commandLineDictionary);

            if (isGenerateSchemaCommand.Item1)
            {
                var databaseName = isGenerateSchemaCommand.Item2;
                var destinationFolder = isGenerateSchemaCommand.Item3;

                var schemaGenerator = AppContainer.Resolve<ISchemaGenerator>();

                schemaGenerator.GenerateSchema(databaseName, destinationFolder);
                
            }

            return isGenerateSchemaCommand.Item1;

        }

        private Tuple<bool, string, string> IsGenerateSchemaCommand(CommandLineDictionary commandLineDictionary)
        {
            bool isGenerateSchemaCommand = commandLineDictionary.ContainsKey(GenerateSchemaArg);

            string databaseName = null;

            string destinationFolder = null;

            if (isGenerateSchemaCommand)
            {
                databaseName = commandLineDictionary[GenerateSchemaArg];

                if (String.IsNullOrEmpty(databaseName))
                {
                    throw new InvalidParameterException("All or DatabaseName expected with generateschema flag.i.e generateschema = All");
                    // Console.WriteLine("All or DatabaseName expected with generateschema flag. i.e generateschema=All");

                }

                bool hasDestinationLocation = commandLineDictionary.ContainsKey(DestinationArg);

                if (hasDestinationLocation)
                {
                    destinationFolder = commandLineDictionary[DestinationArg];

                    if (String.IsNullOrEmpty(destinationFolder))
                    {
                        throw new InvalidParameterException(
                            "destination folder expected with generateschema flag. i.e dest=c:\temp");
                    }

                    if (!Directory.Exists(destinationFolder))
                    {
                        throw new InvalidParameterException("Destination folder does not exist. " + destinationFolder);
                    }
                }
                else
                {
                    throw new InvalidParameterException(
                            "destination folder expected with generateschema flag. i.e dest=c:\temp");
                }
            }

            return new Tuple<bool, string, string>(isGenerateSchemaCommand, databaseName, destinationFolder);
        }
    }
}
