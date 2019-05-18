//Copyright (c) Chris Pietschmann 2015 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: https://github.com/crpietschmann/SQLinq/blob/master/LICENSE

using System;
using System.Text;
using SQLinq.Dialect;

namespace SQLinq
{
    public static class DialectProvider
    {
        public static Type DefaultProviderType = typeof(SqlServerDialect);
        public static ISqlDialect Create()
        {
            return (ISqlDialect)Activator.CreateInstance(DefaultProviderType);
        }

        /// <summary>Gets the default sql dialect (Sql Server).</summary>
        /// <value>The default ISqlDialect.</value>
        public static ISqlDialect Default => SqlServer;

        /// <summary>Gets the SQL server dialect.</summary>
        /// <value>The SQL server ISqlDialect.</value>
        public static ISqlDialect SqlServer => new SqlServerDialect();

        /// <summary>Gets the oracle dialect.</summary>
        /// <value>The oracle ISqlDialect.</value>
        public static ISqlDialect Oracle => new OracleDialect();

        /// <summary>Gets MySQL dialect.</summary>
        /// <value>The MySQL dialect ISqlDialect</value>
        public static ISqlDialect MySql => new MySqlDialect(); 

        

        public static ISqlDialect Create<T>()
            where T : ISqlDialect, new()
        {
            return new T();
        }

        internal static string ConcatFieldArray(string[] fields)
        {
            if (fields == null)
            {
                return string.Empty;
            }

            if (fields.Length == 0)
            {
                return string.Empty;
            }

            return string.Join(", ", fields);

        }
    }
}
