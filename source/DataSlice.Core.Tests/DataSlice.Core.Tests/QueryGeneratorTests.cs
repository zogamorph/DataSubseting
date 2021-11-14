using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DataSlice.Core.Generation;
using NUnit.Framework;

namespace DataSlice.Core.Tests
{
    [TestFixture]
    public class QueryGeneratorTests
    {
        [Test]
        public void Given_A_Model_With_Table_With_Where_Clause_Then_Correct_Query_Is_Generated()
        {
            //Arrange
            var schema = CreateSchema("TableName", "dbo");

            var model = CreateModel("TableName", "dbo");

            model.GlobalParameters = new List<ExtractParameter>()
            {
                new ExtractParameter()
                {
                    ParameterName = "productVersion",
                    ParameterType = "int",
                    ParameterValue = "1"
                },
                new ExtractParameter()
                {
                    ParameterName = "profileName",
                    ParameterType = "string",
                    ParameterValue = "The Pixel Pusher Profile"
                }
            };

            model.Tables[0].Criteria = new ExtractCriteria()
            {
                StaticCriteria = new List<string>()
                {
                    "t.Column1 = @productVersion"
                }
            };

            IMigrationQueryGenerator generator = new SqlMigrationQueryGenerator();

            var result = generator.GenerateSourceQueries(model, schema);

            var extract = result.Single(u => u.Key.TableName.EndsWith("TableName"));

            string expectedQuery = NormalizeSpacing(@"SELECT T.[Column1] , T.[Column2] From [dbo].[TableName] T WHERE  t.Column1 = 1".Trim());

            var generatedQuery = NormalizeSpacing(extract.Value.Trim());

            //ScriptSwitch database is case sensive and so leaving it as such
            bool areTheSame = generatedQuery.Equals(expectedQuery);

            Assert.IsTrue(areTheSame, "Generate queries are different Expected ={0}  | Actual={1}", extract.Value,
                expectedQuery);
        }

        [Test]
        public void Given_A_Model_With_String_Parameter_Then_Correct_Query_Is_Generated()
        {
            //Arrange
            var schema = CreateSchema("TableName", "dbo");

            var model = CreateModel("TableName", "dbo");

            model.GlobalParameters = new List<ExtractParameter>()
            {
                new ExtractParameter()
                {
                    ParameterName = "productVersion",
                    ParameterType = "int",
                    ParameterValue = "1"
                },
                new ExtractParameter()
                {
                    ParameterName = "profileName",
                    ParameterType = "string",
                    ParameterValue = "The Pixel Pusher Profile"
                }
            };

            model.Tables[0].Criteria = new ExtractCriteria()
            {
                StaticCriteria = new List<string>()
                {
                    "t.Column1 = @profileName"
                }
            };

            //Act
            IMigrationQueryGenerator generator = new SqlMigrationQueryGenerator();

            var result = generator.GenerateSourceQueries(model, schema);

            var extract = result.Single(u => u.Key.TableName.EndsWith("TableName"));

            string expectedQuery = NormalizeSpacing(@"SELECT T.[Column1] , T.[Column2] From [dbo].[TableName] T WHERE  t.Column1 = 'The Pixel Pusher Profile'".Trim());

            var generatedQuery = NormalizeSpacing(extract.Value.Trim());

            //Assert
            //ScriptSwitch database is case sensive and so leaving it as such
            bool areTheSame = generatedQuery.Equals(expectedQuery);

            Assert.IsTrue(areTheSame, "Generate queries are different Expected ={0}  | Actual={1}", extract.Value,
                expectedQuery);
        }


        [Test]
        public void Given_A_Model_With_A_Dual_Parameter_Then_Correct_Query_Is_Generated()
        {
            //Arrange
            var schema = CreateSchema("TableName", "dbo");

            var model = CreateModel("TableName", "dbo");

            model.GlobalParameters = new List<ExtractParameter>()
            {
                new ExtractParameter()
                {
                    ParameterName = "productVersion",
                    ParameterType = "int",
                    ParameterValue = "1,2"
                },
                new ExtractParameter()
                {
                    ParameterName = "profileName",
                    ParameterType = "string",
                    ParameterValue = "The Pixel Pusher Profile"
                }
            };

            model.Tables[0].Criteria = new ExtractCriteria()
            {
                StaticCriteria = new List<string>()
                {
                    "t.Column1 = @productVersion"
                }
            };

            //Act
            IMigrationQueryGenerator generator = new SqlMigrationQueryGenerator();

            var result = generator.GenerateSourceQueries(model, schema);

            var extract = result.Single(u => u.Key.TableName.EndsWith("TableName"));

            string expectedQuery = NormalizeSpacing(@"SELECT T.[Column1] , T.[Column2] From [dbo].[TableName] T WHERE  t.Column1 IN (1,2)".Trim());

            var generatedQuery = NormalizeSpacing(extract.Value.Trim());

            //Assert
            //ScriptSwitch database is case sensive and so leaving it as such
            bool areTheSame = generatedQuery.Equals(expectedQuery);

            Assert.IsTrue(areTheSame, "Generate queries are different Expected ={0}  | Actual={1}", extract.Value,
                expectedQuery);
        }

        [Test]
        public void Given_A_Model_With_A_Limit_Parameter_Then_Correct_Query_Is_Generated()
        {
            //Arrange
            var schema = CreateSchema("TableName", "dbo");

            var model = CreateModel("TableName", "dbo");

            model.GlobalParameters = new List<ExtractParameter>()
            {
                new ExtractParameter()
                {
                    ParameterName = "productVersion",
                    ParameterType = "int",
                    ParameterValue = "1,2"
                },
                new ExtractParameter()
                {
                    ParameterName = "profileName",
                    ParameterType = "string",
                    ParameterValue = "The Pixel Pusher Profile"
                }
            };

            model.Tables[0].Criteria = new ExtractCriteria()
            {
                StaticCriteria = new List<string>()
                {
                    "t.Column1 = @productVersion"
                }
            };

            model.Tables[0].Limit = new DataExtractLimit()
            {
                SortColumns = new List<string>() { "Column1 asc", "Column2 desc" },
                TopN = 1000
            };

            //Act
            IMigrationQueryGenerator generator = new SqlMigrationQueryGenerator();

            var result = generator.GenerateSourceQueries(model, schema);

            var extract = result.Single(u => u.Key.TableName.EndsWith("TableName"));

            string expectedQuery = NormalizeSpacing(@"SELECT TOP 1000 T.[Column1] , T.[Column2] From [dbo].[TableName] T WHERE t.Column1 IN (1,2) ORDER BY t.Column1 asc , t.Column2 desc".Trim());

            var generatedQuery = NormalizeSpacing(extract.Value.Trim());

            //Assert
            //ScriptSwitch database is case sensive and so leaving it as such
            bool areTheSame = generatedQuery.Equals(expectedQuery);

            Assert.IsTrue(areTheSame, "Generate queries are different Expected ={0}  | Actual={1}", extract.Value,
                expectedQuery);
        }

        [Test]
        public void Given_A_Model_With_ImportAll_Set_Then_Correct_Query_Is_Generated()
        {
            //Arrange
            var schema = CreateSchema("TableName", "dbo");

            var model = CreateModel("TableName", "dbo");

            model.GlobalParameters = new List<ExtractParameter>()
            {
                new ExtractParameter()
                {
                    ParameterName = "productVersion",
                    ParameterType = "int",
                    ParameterValue = "1,2"
                },
                new ExtractParameter()
                {
                    ParameterName = "profileName",
                    ParameterType = "string",
                    ParameterValue = "The Pixel Pusher Profile"
                }
            };

            model.Tables[0].Criteria = new ExtractCriteria()
            {
                StaticCriteria = new List<string>()


            };

            model.Tables[0].ImportAll = true;

            //Act
            IMigrationQueryGenerator generator = new SqlMigrationQueryGenerator();

            var result = generator.GenerateSourceQueries(model, schema);

            var extract = result.Single(u => u.Key.TableName.EndsWith("TableName"));

            string expectedQuery = NormalizeSpacing(@"SELECT T.[Column1] , T.[Column2] From [dbo].[TableName] T".Trim());

            var generatedQuery = NormalizeSpacing(extract.Value.Trim());

            //Assert
            //ScriptSwitch database is case sensive and so leaving it as such
            bool areTheSame = generatedQuery.Equals(expectedQuery);

            Assert.IsTrue(areTheSame, "Generate queries are different Expected ={0}  | Actual={1}", extract.Value,
                expectedQuery);
        }

        [Test]
        public void Given_SchemaMismatch__Then_Exception_Is_Thrown()
        {
            //Arrange
            var schema = CreateSchema("TableName", "dbo");

            var model = CreateModel("TableName", "otherschema");

            model.GlobalParameters = new List<ExtractParameter>()
            {
                new ExtractParameter()
                {
                    ParameterName = "productVersion",
                    ParameterType = "int",
                    ParameterValue = "1,2"
                },
                new ExtractParameter()
                {
                    ParameterName = "profileName",
                    ParameterType = "string",
                    ParameterValue = "The Pixel Pusher Profile"
                }
            };

            model.Tables[0].Criteria = new ExtractCriteria()
            {
                StaticCriteria = new List<string>()


            };

            model.Tables[0].ImportAll = true;

            //Act
            IMigrationQueryGenerator generator = new SqlMigrationQueryGenerator();

            //Assert
            Assert.Throws<System.InvalidOperationException>(() => generator.GenerateSourceQueries(model, schema));
        }

        private string NormalizeSpacing(string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }

        private DataExtractModel CreateModel(string tableName, string tableSchema)
        {
            DataExtractModel model = new DataExtractModel()
            {
                GlobalParameters = new List<ExtractParameter>(),
                Tables = new List<TableExtract>()
                {
                    new TableExtract()
                    {
                        TableName = tableName,
                        Schema = tableSchema,
                        Criteria = new ExtractCriteria(),
                        ImportAll = false
                    }
                }
            };

            return model;
        }

        private Schema CreateSchema(string tableName, string databaseSchema)
        {
            var schema = new Schema()
            {
                Tables = new List<Table>()
                {
                    new Table()
                    {
                        Name = tableName,
                        Schema = databaseSchema,
                        Dependencies = new List<Dependency>(),
                        Columns = new List<Column>()
                        {
                            new Column()
                            {
                                Name = "Column1",
                                DataType = "int",
                                DefaultValue = null
                            },
                            new Column()
                            {
                                Name = "Column2",
                                DataType = "string",
                                DefaultValue = null
                            }
                        }
                    }
                }
            };

            return schema;

        }
    }
}
