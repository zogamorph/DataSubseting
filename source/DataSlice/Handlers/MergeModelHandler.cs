using System;
using DataSlice.CommandLineParsing;
using DataSlice.Core;
using DataSlice.Core.SchemaGeneration;


namespace DataSlice.Handlers
{
    public class MergeModelHandler : ICommandHandler
    {
        private const string MergeModelArg = "mergemodel";

        private const string SourceArg = "source";

        private const string DestinationArg = "dest";

        private const string NewFileArg = "newfile";

        public bool Handle(CommandLineDictionary commandLineDictionary)
        {
            var extractedParameters = ExtractParams(commandLineDictionary);

            bool canHandle = extractedParameters.Item1;

            if (canHandle)
            {
                var mergeModel = AppContainer.Resolve<IModelMerge>();

                mergeModel.Merge(extractedParameters.Item2, extractedParameters.Item3, extractedParameters.Item4);
            }

            return canHandle;
        }

        private Tuple<bool, string, string, string> ExtractParams(CommandLineDictionary commandLineDictionary)
        {
            bool isMergeModelCommand = commandLineDictionary.ContainsKey(MergeModelArg);

            string source = null;

            string destination = null;

            string newFile = null;

            if (isMergeModelCommand)
            {
                source = commandLineDictionary[SourceArg];

                if (String.IsNullOrWhiteSpace(source))
                {
                    throw new InvalidParameterException("Source file is expected with merge model flag. ex.source=xxx");
                }

                destination = commandLineDictionary[DestinationArg];

                if (String.IsNullOrWhiteSpace(destination))
                {
                    throw new InvalidParameterException("Destination file is expected with merge model flag. ex. dest=xxx");
                }

                newFile = commandLineDictionary[NewFileArg];

                if (String.IsNullOrWhiteSpace(newFile))
                {
                    throw new InvalidParameterException("New file is expected with merge model flag. ex. NewFile=xxx");
                }
            }

            return new Tuple<bool, string, string, string>(isMergeModelCommand, source, destination,newFile);
        }

        
    }
}
