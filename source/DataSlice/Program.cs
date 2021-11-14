using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DataSlice.CommandLineParsing;
using DataSlice.Core;
using DataSlice.Core.DataWiping;
using DataSlice.Core.Generation;
using DataSlice.Core.SchemaGeneration;
using DataSlice.Core.Settings;
using DataSlice.Core.Utils;
using DataSlice.Handlers;

namespace DataSlice
{
    class Program
    {
        static int Main(string[] args)
        {
            AppContainer.Initialize();

            var logger = AppContainer.Resolve<IAppLogger>();

            try
            {
                if (args.Length > 0)
                {
                    //not using stop watch
                    var start = DateTime.Now;
                    
                    CommandLineDictionary commandLineDictionary = CommandLineDictionary.FromArguments(args);

                    List<ICommandHandler> handlers = new List<ICommandHandler>()
                    {
                        new WipeDatabaseHandler(),
                        new GenerateSchemaHandler(),
                        new SubsetHandler(),
                        new MergeModelHandler(),
                        new BackupHandler()
                    };

                    bool handled = false;

                    foreach (var handler in handlers)
                    {
                        handled = handler.Handle(commandLineDictionary);

                        if (handled)
                        {
                            break;
                        }
                    }

                    if (!handled)
                    {
                        Console.WriteLine("Invalid parameters. No handler found");
                        return 1;
                    }

                    var end = (DateTime.Now - start).TotalMinutes;

                    logger.Info("Completed in {0} minutes", end);
                    Console.WriteLine("Completed in {0} minutes", end);
                    //Console.ReadLine();
                    return 0;
                }
                else
                {
                    Console.WriteLine("No arguments passed");
                    return 1;
                }
            }
            catch (InvalidParameterException ipe)
            {
                Console.WriteLine(ipe.InvalidParameterMessage);
                logger.Error(ipe);

                throw;

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

        }

        private static void OtherCode()
        {
            //IDatabasesToSubsetSettings settings = new DatabasesToSubsetSettings();

            //foreach (var setting in settings.DatabaseList.OrderBy(u=>u.Order))
            //{
            //    Console.WriteLine("{0} {1} {2}", setting.Name, setting.Order,  setting.Ignore);
            //}
            //Console.ReadLine();

            //return 0;

             CreateTables creator = new CreateTables();

            // creator.Run();

            // SubsetManager manager = new SubsetManager();

            // Stopwatch stopWatch = new Stopwatch();

            //    stopWatch.Start();

            // manager.Process();

            //    stopWatch.Stop();

            //    Console.WriteLine("Completed in " + stopWatch.Elapsed.TotalMinutes + " minutes");

            //    Console.ReadLine();



            //}
        }
    }

   
}
