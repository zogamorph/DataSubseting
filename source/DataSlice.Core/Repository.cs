using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DataSlice.Core
{
    public class Repository
    {
        private string _connectionString;

        public Repository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;
        }


        public List<Column> GetColumns(Table table, string databaseName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var columns = connection.Query<Column>(@"
		SELECT  COLUMN_NAME as Name,  DATA_TYPE as DataType
FROM INFORMATION_SCHEMA.COLUMNS
where Table_Name=@tableName and Table_Schema=@tableSchema
and TABLE_CATALOG=@databaseName
and COLUMNPROPERTY(OBJECT_ID(TABLE_SCHEMA+'.'+TABLE_NAME),COLUMN_NAME,'IsComputed') =0
order by Ordinal_Position", new { DatabaseName = databaseName, TableName = table.Name, TableSchema = table.Schema }).ToList();

                return columns;


            }
        }

        public List<Table> GetAllTables(string databaseName)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var tables = connection.Query<Table>(@"
		SELECT s.TABLE_NAME as Name, TABLE_SCHEMA as [Schema]
FROM INFORMATION_SCHEMA.TABLES s
WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG=@DatabaseName order by s.Table_Name", new { DatabaseName = databaseName }).ToList();


                tables.RemoveAll(u => u.Schema.Equals("sys", StringComparison.OrdinalIgnoreCase));
                tables.RemoveAll(u => u.Name.Equals("sysdiagrams", StringComparison.OrdinalIgnoreCase));
                return tables;

            }
        }

        public List<string> GetDependencies(Table table)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var dep = connection.Query<string>(@"
		select

SO_R.name as [referenced table]


from sys.foreign_key_columns FKC
inner join sys.objects SO_P on SO_P.object_id = FKC.parent_object_id
inner join sys.columns SC_P on (SC_P.object_id = FKC.parent_object_id) AND (SC_P.column_id = FKC.parent_column_id)
inner join sys.objects SO_R on SO_R.object_id = FKC.referenced_object_id
inner join sys.columns SC_R on (SC_R.object_id = FKC.referenced_object_id) AND (SC_R.column_id = FKC.referenced_column_id)
where
    ((SO_P.name = @tableName) AND (SO_P.type = 'U'))
    OR
    ((SO_R.name = @tableName) AND (SO_R.type = 'U'))", new { tableName = table.Name }).ToList();

                return dep.Distinct().ToList();

            }
        }

        public List<Dependency> GetReferencedTables(Table table)
        {
            List<Dependency> dependencies = new List<Dependency>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var dep = connection.Query<DependencyInfo>(@"
			select 
    t.name as TableWithForeignKey, 
	c.name as ForeignKeyColumn ,
	t1.name as ParentTable,
	p.name as ParentColumn,
	 fk.constraint_column_id as FK_PartNo
from 
    sys.foreign_key_columns as fk
inner join 
    sys.tables as t on fk.parent_object_id = t.object_id
inner join 
    sys.columns as c on fk.parent_object_id = c.object_id and fk.parent_column_id = c.column_id
inner join sys.tables t1 on fk.referenced_object_id =  t1.object_id
inner join sys.columns as p on fk.referenced_object_id = p.object_id and fk.referenced_column_id = p.column_id
where 
    (fk.referenced_object_id = (select object_id 
                               from sys.tables 
                               where name = @tableName))
or (fk.parent_object_id = (select object_id 
                               from sys.tables 
                               where name = @tableName))
order by 
    TableWithForeignKey, FK_PartNo", new { tableName = table.Name }).ToList();


                dependencies.AddRange(from dependencyInfo in dep
                    where dependencyInfo.TableWithForeignKey.Equals(table.Name, StringComparison.OrdinalIgnoreCase)
                    select new Dependency()
                    {
                        FromTableName = table.Name, FromTableSchema = table.Schema, ToTableName = dependencyInfo.ParentTable, ToColumnName = dependencyInfo.ParentColumn, ToTableSchema = table.Schema, FromColumn = dependencyInfo.ForeignKeyColumn
                    });

                return dependencies;

            }
        }

    }
}
