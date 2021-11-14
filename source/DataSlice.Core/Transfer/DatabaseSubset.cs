using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataSlice.Core.Generation;
using DataSlice.Core.Settings;
using DataSlice.Core.Utils;

namespace DataSlice.Core.Transfer
{
    public class DatabaseSubset
    {
        private readonly IMigrationQueryGenerator _queryGenerator;

        private readonly IAppSettings _appSettings;

        private readonly IIndexManager _indexManager;

        private readonly SemaphoreSlim _semaphoreSlim;

        private readonly IAppLogger _appLogger;

        public DataExtractModel Model { get; set; }

        public Schema Schema { get; set; }

        public DatabaseToSubset DatabaseToSubset { get; set; }


        public  DatabaseSubset(IMigrationQueryGenerator queryGenerator,  IIndexManager indexManager,  IAppSettings appSettings, IAppLogger logger)
        {
            _appSettings = appSettings;

            _queryGenerator = queryGenerator;

            _indexManager = indexManager;

            _appLogger = logger;

            if (appSettings.MaxThreadsPerDatabase > 0)
            {
                _semaphoreSlim = new SemaphoreSlim(_appSettings.MaxThreadsPerDatabase);
            }
            

        }

        public void Subset()
        {
            Info("Starting database {0} subsetting", DatabaseToSubset.Name);

            var model = TopoSortHelper.GetTopologicallySortedModel(Model, Schema);

            var sourceQueries = _queryGenerator.GenerateSourceQueries(model, Schema);


            _indexManager.Model = model;

            _indexManager.DestinationConnectionString = DatabaseToSubset.Destination;

            _indexManager.DisableAllContraints();


            var cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var tasks = new List<Task>();

                WriteQueriesToFile(sourceQueries, DatabaseToSubset.Name);

                foreach (var tableInfo in model.Tables)
                {
                    //await _semaphoreSlim.WaitAsync();

                    // _semaphoreSlim.Wait(cancellationTokenSource.Token);

                    if (cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        break;
                    }
                    _semaphoreSlim.Wait();

                    var task = Task.Factory.StartNew(async () =>
                    {
                        try
                        {
                            AsyncSqlServerTableTransfer transfer = new AsyncSqlServerTableTransfer(_appSettings, _appLogger);

                            

                            await transfer.ProcessTransferAsync(DatabaseToSubset, tableInfo, sourceQueries, cancellationTokenSource);
                        }
                        catch (Exception)
                        {
                            if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
                            {
                                cancellationTokenSource.Cancel();
                            }
                            throw;
                        }
                        finally
                        {
                            int count = _semaphoreSlim.Release();
                            Console.WriteLine("Semaphore release={0}", count);
                        }
                    }, cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                    tasks.Add(task.Unwrap());
                   
                }
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException e)
            {
                Info("\nAggregateException thrown with the following inner exceptions:");
                //Console.WriteLine("\nAggregateException thrown with the following inner exceptions:");
                // Display information about each exception. 
                foreach (var v in e.InnerExceptions)
                {
                    if (v is TaskCanceledException)
                    {
                        Info("TaskCanceledException: Task {0}", ((TaskCanceledException) v).Task.Id);

                        //Console.WriteLine("   TaskCanceledException: Task {0}",
                        //                  ((TaskCanceledException)v).Task.Id);
                    }

                    else
                    {
                        Info("   Exception: {0}", v.GetType().Name);
                        //Console.WriteLine("   Exception: {0}", v.GetType().Name);
                    }
                       
                }

                throw;

            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
            


        }

        private void Info(string message, params object[] parameters)
        {
            _appLogger.Info(message, parameters);
            Console.WriteLine(message, parameters);
        }

        private void WriteQueriesToFile(Dictionary<TableExtract, string> result, string databaseName)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var table in result.Keys)
            {
                sb.AppendLine("--Table = " + table.TableName);
                sb.AppendLine("--*********************");
                sb.AppendLine(result[table]);
                sb.AppendLine("----------------------");
                sb.AppendLine("");
                //  sb.AppendLine("");
            }

            string fileName = String.Format("{0}-generated-query.sql", databaseName);

            string writeLocation = Path.Combine(LocationHelper.EnsureExePathDirectory("Queries"), fileName);

            File.WriteAllText(writeLocation, sb.ToString());
        }
    }
}
