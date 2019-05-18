using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLinq;

namespace SQLinqTest
{
    [TestClass]
    public class SQLinqColumnTest
    {
        [TestMethod]
        public void UseWithSQLinqSubQuery_001()
        {
            var query = new SQLinq<MyTable>();

            var mysql = new SQLinq<MySqlTable>(DialectProvider.MySql).ToSQL().ToQuery();


            var result = query.ToSQL();
            var actual = result.ToQuery();

            var expected = "SELECT [Identifier] AS [ID], [FullName] AS [Name] FROM (SELECT [Identifier], [FullName] FROM [tblSomeTable]) AS [MyTable]";
            var expectedMysql = "SELECT `Identifier` AS `ID`, `FullName` AS `Name` FROM (SELECT `Identifier`, `FullName` FROM `tblSomeTable`) AS `MySqlTable`";

            
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expectedMysql, mysql);

        }

        [TestMethod]
        public void UseWithSQLinqSubQuery_Oracle_001()
        {
            var dialect = new OracleDialect();
            var query = new SQLinq<MyTable>(dialect);

            var result = query.ToSQL();
            var actual = result.ToQuery();

            var expected = "SELECT Identifier AS ID, FullName AS Name FROM (SELECT [Identifier], [FullName] FROM [tblSomeTable]) AS MyTable";

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UseWithSQLinqSubQuery_002()
        {
            var query = from item in new SQLinq<MyTable>()
                        where item.Name == "Chris"
                        select item;

            var result = query.ToSQL();
            var actual = result.ToQuery();

            var expected = "SELECT [Identifier] AS [ID], [FullName] AS [Name] FROM (SELECT [Identifier], [FullName] FROM [tblSomeTable]) AS [MyTable] WHERE [FullName] = @sqlinq_1";

            var mySqlQuery = new SQLinq<MySqlTable>(DialectProvider.MySql).Where(x => x.Name == "Chris");
            var resultMySql = mySqlQuery.ToSQL();
            var actualMysql = resultMySql.ToQuery();

            var expectedMysql = "SELECT `Identifier` AS `ID`, `FullName` AS `Name` FROM (SELECT `Identifier`, `FullName` FROM `tblSomeTable`) AS `MySqlTable` WHERE `FullName` = @sqlinq_1";

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expectedMysql, actualMysql);
        }

        [TestMethod]
        public void UseWithSQLinqSubQuery_Oracle_002()
        {
            var dialect = new OracleDialect();
            var query = from item in new SQLinq<MyTable>(dialect)
                        where item.Name == "Chris"
                        select item;

            var result = query.ToSQL();
            var actual = result.ToQuery();

            var expected = "SELECT Identifier AS ID, FullName AS Name FROM (SELECT [Identifier], [FullName] FROM [tblSomeTable]) AS MyTable WHERE FullName = :sqlinq_1";

            Assert.AreEqual(expected, actual);
        }

        [SQLinqSubQuery(SQL = @"SELECT [Identifier], [FullName] FROM [tblSomeTable]")]
        private class MyTable
        {
            [SQLinqColumn("Identifier")]
            public int ID { get; set; }

            [SQLinqColumn("FullName")]
            public string Name { get; set; }
        }

        [SQLinqSubQuery(SQL = @"SELECT `Identifier`, `FullName` FROM `tblSomeTable`")]
        private class MySqlTable
        {
            [SQLinqColumn("Identifier")]
            public int ID { get; set; }

            [SQLinqColumn("FullName")]
            public string Name { get; set; }
        }
    }
}
