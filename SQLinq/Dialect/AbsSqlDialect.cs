namespace SQLinq
{
    public abstract class AbsSqlDialect 
    {
        protected string _Space = " ";
        protected string _parameterPrefix = "@";

        public abstract object ConvertParameterValue(object value);
        


        public abstract string ParseTableName(string tableName);


        public abstract string ParseColumnName(string columnName);

        public abstract void AssertSkip<T>(SQLinq<T> sqLinq);

        public abstract string ToQuery(SQLinqSelectResult selectResult);
    }
}