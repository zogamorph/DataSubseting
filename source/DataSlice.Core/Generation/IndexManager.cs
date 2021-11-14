using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core.Generation
{
    public class IndexManager : IIndexManager
    {
        private List<string> _indexes = new List<string>();

        //private DataExtractModel _model;

        private const string _indexCommand = "ALTER INDEX ALL ON [{0}].[{1}] {2};";

        private const string _disableConstraintCommand = "ALTER TABLE [{0}].[{1}] NOCHECK CONSTRAINT ALL";

        private const string _enableConstraintCommand = "ALTER TABLE [{0}].[{1}] WITH CHECK CHECK CONSTRAINT ALL";

        private IAppSettings _appSettings;

        public IndexManager(IAppSettings appSettings)
        {
            _appSettings = appSettings;
         
        }

        public DataExtractModel Model { get; set; }

        public string DestinationConnectionString { get; set; }


        public void DisableAllIndexes()
        {
            StringBuilder sb = new StringBuilder();
            //foreach alter disable
            foreach (var table in Model.Tables)
            {
                string statement = String.Format(_indexCommand, table.Schema, table.TableName, " DISABLE; ");
                sb.AppendLine(statement + ";");
                //execute and disable
            }

            using (SqlConnection connection = new SqlConnection(DestinationConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                {
                    command.ExecuteNonQuery();
                }
               
            }          
        }

        public void DisableAllContraints()
        {
            StringBuilder sb = new StringBuilder();
            //foreach alter disable
            foreach (var table in Model.Tables)
            {
                string statement = String.Format(_disableConstraintCommand, table.Schema, table.TableName);
                sb.AppendLine(statement + ";");
                //execute and disable
            }

            using (SqlConnection connection = new SqlConnection(DestinationConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                {
                    command.ExecuteNonQuery();
                }

            }
        }


        public void EnableAllContraints()
        {
            StringBuilder sb = new StringBuilder();
            //foreach alter disable
            foreach (var table in Model.Tables)
            {
                string statement = String.Format(_enableConstraintCommand, table.Schema, table.TableName);
                sb.AppendLine(statement + ";");
                //execute and disable
            }

            using (SqlConnection connection = new SqlConnection( DestinationConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                {
                    command.ExecuteNonQuery();
                }

            }
        }

        public void EnableAllIndexes()
        {
            StringBuilder sb = new StringBuilder();
      
            //foreach alter disable
            foreach (var table in Model.Tables)
            {
                string statement = String.Format(_indexCommand, table.Schema, table.TableName, " REBUILD; ");
                sb.AppendLine(statement + ";");
                
                //exceute and enable
            }

            using (SqlConnection connection = new SqlConnection(DestinationConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sb.ToString(), connection))
                {
                    command.ExecuteNonQuery();
                }

            }
        }
    }
}
