using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataSlice.Core.Utils;

namespace DataSlice.Core
{
    public class SourceQueryGenerator
    {
        private readonly DataExtractModel _dataExtractModel;

        private readonly Schema _schema;

        private readonly string _tableAlias = "T";

        private Dictionary<string, string> _computedParameters = new Dictionary<string, string>();


        public SourceQueryGenerator(DataExtractModel model, Schema schema)
        {
            _dataExtractModel = model;

            _schema = schema;
        }


        public string GenerateSourceQuery(string table, string schema, int level)
        {
            var tableInfo =
                _schema.Tables.First(
                    u =>
                        u.Name.Equals(table, StringComparison.OrdinalIgnoreCase) &&
                        u.Schema.Equals(schema, StringComparison.OrdinalIgnoreCase));

            var tableModel =
                _dataExtractModel.Tables.First(
                    u =>
                        u.Schema.Equals(schema, StringComparison.OrdinalIgnoreCase) &&
                        u.TableName.Equals(table, StringComparison.OrdinalIgnoreCase));

            StringBuilder finalQuery = new StringBuilder();

            finalQuery.Append(GenerateSelectClause(tableInfo, tableModel, level));

            bool whereClauseSet = false;

            if (tableModel.Criteria == null || !tableModel.Criteria.StaticCriteria.Any())
            {
                if (!tableModel.ImportAll)
                {
                    finalQuery.AppendLine(" WHERE 1=0;");
                    return finalQuery.ToString();
                }
            }

            if (tableModel.Criteria != null && tableModel.Criteria.StaticCriteria.Any())
            {
                for (int i = 0; i < tableModel.Criteria.StaticCriteria.Count; i++)
                {
                    if (i == 0)
                    {
                        whereClauseSet = true;
                        finalQuery.AppendLine(" WHERE ");
                    }
                    else
                    {
                        if (whereClauseSet == true)
                        {
                            finalQuery.AppendLine(" AND ");
                        }
                    }

                    string clause = ReplaceStaticParameters(tableModel.Criteria.StaticCriteria[i]);

                    finalQuery.Append(String.Format(" {0} ", clause));
                }

            }

            if (tableModel.Limit?.SortColumns != null && tableModel.Limit.SortColumns.Any())
            {
            
                for (int i = 0; i < tableModel.Limit.SortColumns.Count; i++)
                {
                    if (i == 0)
                    {
                        finalQuery.Append(" ORDER BY");
                        finalQuery.AppendFormat(" t.{0} ", tableModel.Limit.SortColumns[i]);
                        if (i != tableModel.Limit.SortColumns.Count - 1)
                        {
                            finalQuery.Append(" , ");
                        }
                    }
                    else
                    {
                        finalQuery.AppendFormat(" t.{0} ", tableModel.Limit.SortColumns[i]);
                        if (i != tableModel.Limit.SortColumns.Count - 1)
                        {
                            finalQuery.Append(" , ");
                        }
                    }
                }
            }


            return finalQuery.ToString();
        }




        private string GenerateSelectClause(Table table, TableExtract tableExtract, int level)
        {
            var select = "SELECT ";

            if (tableExtract.Limit != null && tableExtract.Limit.TopN.HasValue)
            {
                select = select + " TOP " + tableExtract.Limit.TopN.Value.ToString() + " ";
            }

            StringBuilder sb = new StringBuilder(select);

            sb.Append(string.Join(" , ", table.Columns.Select(u => String.Format("{0}.[{1}]",_tableAlias, u.Name))));

            sb.Append(String.Format(" From [{0}].[{1}] {2}", table.Schema, table.Name, _tableAlias));

            return sb.ToString();
        }

        public string ReplaceStaticParameters(string query)
        {

            string newQuery = Regex.Replace(query, "\r\n ?|\n", " "); //check

            //find all parameters starting with @
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            var matches = Regex.Matches(newQuery, @"\@\w+");

            foreach (Match match in matches)
            {
                if (!dictionary.ContainsKey(match.Value.ToLower()))
                {
                    dictionary.Add(match.Value.ToLowerInvariant(), null);
                }
            }

            if (matches.Count == 0)
            {
                return newQuery;
            }

            //now lookup global parameter replace
            foreach (var key in dictionary.Keys)
            {
                var parameterValueInfo = CreateParameterValue(key);

                if (parameterValueInfo.Item2)
                {
                    newQuery = StringUtils.ReplaceString(newQuery, "=" + key, " IN " + parameterValueInfo.Item1, StringComparison.OrdinalIgnoreCase);
                    newQuery = StringUtils.ReplaceString(newQuery, "= " + key, " IN " + parameterValueInfo.Item1, StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    newQuery = StringUtils.ReplaceString(newQuery, key, parameterValueInfo.Item1, StringComparison.OrdinalIgnoreCase);
                }



            }

            return newQuery;
        }

        private Tuple<string, bool> CreateParameterValue(string parameterName)
        {
            parameterName = parameterName.Replace("@", "");

            var parameter =
                _dataExtractModel.GlobalParameters.FirstOrDefault(
                    u => u.ParameterName.Equals(parameterName, StringComparison.OrdinalIgnoreCase));

            if (parameter == null)
            {
                throw new InvalidDataException("Global Predefined Parameter not found for @" + parameterName);
            }

            List<string> paramValues = new List<string>();

            if (parameter.ParameterValue.Trim().StartsWith("$"))
            {
                var extractedParameterValue = ExtractParameterValueFromFormula(parameter.ParameterValue);

                return new Tuple<string, bool>(extractedParameterValue, false);
            }

            if (parameter.ParameterValue.Contains(","))
            {
                paramValues = parameter.ParameterValue.Split(',').ToList();
            }
            else
            {
                paramValues.Add(parameter.ParameterValue);
            }

            bool isStringy = false;

            switch (parameter.ParameterType.ToLower())
            {
                case "string":
                case "varchar":
                case "guid":
                case "uniqueidentifier":
                case "date":
                case "datetime":
                case "datetime2":
                    isStringy = true;
                    break;
                case "number":
                case "int":
                case "bigint":
                case "tinyint":
                case "smallint":
                case "decimal":
                case "float":
                case "money":
                    isStringy = false;
                    break;
                default:
                    throw new DataException("Invalid global parameter type : " + parameter.ParameterType);

            }

            if (isStringy)
            {
                if (paramValues.Count == 1)
                {
                    return new Tuple<string, bool>(String.Format("'{0}'", paramValues[0].Trim()), false);
                }

                string[] temp = paramValues.Select(u => String.Format("'{0}'", u.Trim())).ToArray();

                string temp2 = String.Join(",", temp);

                return new Tuple<string, bool>(String.Format("({0})", temp2), true);
            }

            if (paramValues.Count == 1)
            {
                return new Tuple<string, bool>(paramValues[0], false);
            }
            else
            {
                string[] temp = paramValues.Select(u => String.Format("{0}", u.Trim())).ToArray();

                string temp2 = String.Join(",", temp);

                return new Tuple<string, bool>(String.Format("({0})", temp2), true);
            }


        }

        public string ExtractParameterValueFromFormula(string parameterValueExpression)
        {
            int? result = null;

            if (_computedParameters.ContainsKey(parameterValueExpression))
            {
                return _computedParameters[parameterValueExpression];
            }

            if (parameterValueExpression.StartsWith("$min(", StringComparison.OrdinalIgnoreCase) || parameterValueExpression.StartsWith("$max(", StringComparison.OrdinalIgnoreCase))
            {
                var temp = parameterValueExpression.Replace("$min(", String.Empty).Replace(")", String.Empty);
                temp = temp.Replace("$max(", String.Empty);
                temp = temp.Replace("@", String.Empty);

                var parameter =
                _dataExtractModel.GlobalParameters.FirstOrDefault(
                    u => u.ParameterName.Equals(temp, StringComparison.OrdinalIgnoreCase));

                if (parameter == null)
                {
                    throw new InvalidDataException("Global Predefined Parameter not found for @" + temp);
                }

             

                List<int> paramValues = new List<int>();

                if (parameter.ParameterValue.Contains(","))
                {
                    paramValues = parameter.ParameterValue.Split(',').Select(u => Convert.ToInt32(u)).ToList();

                    if (parameterValueExpression.StartsWith("$min(", StringComparison.OrdinalIgnoreCase))
                    {
                        result = paramValues.Min();
                    }
                    else
                    {
                        result = paramValues.Max();
                    }


                }
                else
                {
                    result = Convert.ToInt32(parameter.ParameterValue.Trim());
                }

            }

            if (result == null)
            {
                throw new DataException("Computed parameter " + parameterValueExpression + " could not be evaluated");
            }
            _computedParameters[parameterValueExpression] = result.ToString();
            return result.Value.ToString();
        }

      
    }
}
