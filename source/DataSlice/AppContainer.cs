using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSlice.Core;
using DataSlice.Core.Databackup;
using DataSlice.Core.DataWiping;
using DataSlice.Core.Factory;
using DataSlice.Core.Generation;
using DataSlice.Core.SchemaGeneration;
using DataSlice.Core.Settings;
using DataSlice.Core.Utils;
using StructureMap;

namespace DataSlice
{
    public class AppContainer
    {
        private static Container _container;

        public static void Initialize()
        {
            ServiceLocator serviceLocator = new ServiceLocator();
            

            _container = new Container(x =>
            {
     
                x.For<IAppSettings>().Use<AppSettings>();

                x.For<IDatabasesToSubsetSettings>().Use<DatabasesToSubsetSettings>();

                x.For<IDataWiper>().Use<DataWiper>();

                x.For<IDataWipeManager>().Use<DataWipeManager>();

                x.For<IAppLogger>().Use<AppLogger>();

                x.For<ISchemaGenerator>().Use<SchemaGenerator>();

                 x.For<ISchemaRepository>().Use<SchemaRepository>();

                x.For<IServiceLocator>().Use(serviceLocator);

                x.For<IMigrationQueryGenerator>().Use<SqlMigrationQueryGenerator>();

                x.For<ISubsetService>().Use<SubsetService>();

                x.For<ISubsetService>().Use<SubsetService>();

                x.For<ISubsetService>().Use<SubsetService>();

                x.For<ISubsetService>().Use<SubsetService>();

                x.For<IModelMerge>().Use<ModelMerge>();

                x.For<IIndexManager>().Use<IndexManager>();

                x.For<IDatabaseBackupService>().Use<DatabaseBackupService>();

            });

            serviceLocator.RegisterContainer(new Resolvable(_container));




        }

        public static T Resolve<T>()
        {
            return _container.GetInstance<T>();
        }
    }
}
