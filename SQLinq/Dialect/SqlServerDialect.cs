//Copyright (c) Chris Pietschmann 2015 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: https://github.com/crpietschmann/SQLinq/blob/master/LICENSE

using System;
using System.Text;

namespace SQLinq
{
    public class SqlServerDialect : AbsSqlDialect, ISqlDialect
    {
        //const string _Space = " ";

        public override object ConvertParameterValue(object value)
        {
            return value;
        }

        public override void AssertSkip<T>(SQLinq<T> sqLinq)
        {
            if (sqLinq.OrderByExpressions.Count == 0)
            {
                throw new NotSupportedException("SQLinq: Skip can only be performed if OrderBy is specified.");
            }
        }

        
        public  virtual string ParameterPrefix => _parameterPrefix;

        public  override string ParseTableName(string tableName)
        {
            if (!tableName.StartsWith("[", StringComparison.InvariantCultureIgnoreCase))
            {
                return $"[{tableName}]";
            }

            return tableName;
        }

        public  override string ParseColumnName(string columnName)
        {
            if (!columnName.StartsWith("[", StringComparison.InvariantCultureIgnoreCase) && !columnName.Contains("."))
            {
                return $"[{columnName}]";
            }

            return columnName;  
        }

        public  override string ToQuery(SQLinqSelectResult selectResult)
        {
            var orderby = DialectProvider.ConcatFieldArray(selectResult.OrderBy);

            var groupby = DialectProvider.ConcatFieldArray(selectResult.GroupBy);


            var sb = new StringBuilder();

            if (selectResult.Distinct == true)
            {
                sb.Append("DISTINCT ");
            }

            // SELECT
            sb.Append(DialectProvider.ConcatFieldArray(selectResult.Select));


            if (selectResult.Skip != null)
            {
                if (sb.Length > 0)
                {
                    sb.Append(",");
                }
                sb.Append($" ROW_NUMBER() OVER (ORDER BY {@orderby}) AS [SQLinq_row_number]");
            }

            sb.Append(" FROM ");

            if (selectResult.Distinct == true && selectResult.Skip != null && selectResult.Take != null)
            {
                sb.Append("(SELECT DISTINCT ");
                sb.Append(DialectProvider.ConcatFieldArray(selectResult.Select));
                sb.Append(" FROM ");
                sb.Append(selectResult.Table);
                sb.Append(") AS d");
            }
            else
            {
                sb.Append(selectResult.Table);
            }

            if (selectResult.Join != null)
            {
                foreach (var j in selectResult.Join)
                {
                    sb.Append(_Space);
                    sb.Append(j);
                }
            }

            if (!string.IsNullOrEmpty(selectResult.Where))
            {
                sb.Append(" WHERE ");
                sb.Append(selectResult.Where);
            }

            if (!string.IsNullOrEmpty(groupby))
            {
                sb.Append(" GROUP BY ");
                sb.Append(groupby);
            }

            if (!string.IsNullOrEmpty(selectResult.Having))
            {
                sb.Append(" HAVING ");
                sb.Append(selectResult.Having);
            }


            var sqlOrderBy = string.Empty;
            if (orderby.Length > 0)
            {
                sqlOrderBy = " ORDER BY " + orderby;
            }

            if (selectResult.Skip != null)
            {
                // paging support
                var start = (selectResult.Skip + 1).ToString();
                var end = (selectResult.Skip + (selectResult.Take ?? 0)).ToString();
                if (selectResult.Take == null)
                {
                    if (selectResult.Distinct == true)
                    {
                        return
                            $@"WITH SQLinq_data_set AS (SELECT {sb}) SELECT * FROM SQLinq_data_set WHERE [SQLinq_row_number] >= {start} ORDER BY [SQLinq_row_number]";
                    }
                    else
                    {
                        return
                            $@"WITH SQLinq_data_set AS (SELECT {sb}) SELECT * FROM SQLinq_data_set WHERE [SQLinq_row_number] >= {start}";
                    }
                }

                return
                    $@"WITH SQLinq_data_set AS (SELECT {sb}) SELECT * FROM SQLinq_data_set WHERE [SQLinq_row_number] BETWEEN {start} AND {end}";
            }
            else if (selectResult.Take != null)
            {
                var sbQuery = sb.ToString();
                if (sbQuery.ToLowerInvariant().StartsWith("distinct ", StringComparison.InvariantCultureIgnoreCase))
                {
                    return "SELECT DISTINCT TOP " + selectResult.Take.ToString() + _Space + sbQuery.Substring(9) + sqlOrderBy;
                }
                else
                {
                    return "SELECT TOP " + selectResult.Take.ToString() + _Space + sbQuery + sqlOrderBy;
                }
            }

            return "SELECT " + sb.ToString() + sqlOrderBy;
        }
    }
}
