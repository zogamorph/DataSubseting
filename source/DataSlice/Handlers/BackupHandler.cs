using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSlice.CommandLineParsing;
using DataSlice.Core;
using DataSlice.Core.Databackup;
using DataSlice.Core.SchemaGeneration;

namespace DataSlice.Handlers
{
    public class BackupHandler : ICommandHandler
    {
        private const string BackupArg = "backup";

        public bool Handle(CommandLineDictionary commandLineDictionary)
        {
            var isBackupCommand = IsBackupHandlerCommand(commandLineDictionary);

            if (isBackupCommand.Item1)
            {
                var databaseName = isBackupCommand.Item2;

                var backupService = AppContainer.Resolve<IDatabaseBackupService>();

                backupService.BackupDatabases(databaseName);
            }

            return isBackupCommand.Item1;

        }

        private Tuple<bool, string> IsBackupHandlerCommand(CommandLineDictionary commandLineDictionary)
        {
            bool isBackupCommand = commandLineDictionary.ContainsKey(BackupArg);

            string databaseName = null;

            if (isBackupCommand)
            {
                databaseName = commandLineDictionary[BackupArg];

                if (String.IsNullOrEmpty(databaseName))
                {
                    throw new InvalidParameterException("All or DatabaseName expected with backup flag.i.e /backup=All ");
                }
            }

            return new Tuple<bool, string>(isBackupCommand, databaseName);
        }
    }
}
