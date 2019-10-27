using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    [TestFixture]
    public abstract class MockQueryableTestsBase<T> : QueryableTestsBase<T>
        where T : TestEntityBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        protected abstract void AddFromSqlResult(IQueryable<T> mockedQueryable, IEnumerable<T> expectedResult);
        protected abstract void AddFromSqlResult(IQueryable<T> mockedQueryable, string sql, IEnumerable<T> expectedResult);
        protected abstract void AddFromSqlResult(IQueryable<T> mockedQueryable, string sql, List<SqlParameter> parameters, IEnumerable<T> expectedResult);

        [Test]
        public virtual void FromSql_AnySql_ReturnsExpectedResult()
        {
            var expectedResult = new Fixture().CreateMany<T>().ToList();
            AddFromSqlResult(Queryable, expectedResult);

            var actualResult1 = Queryable.FromSql("sp_NoParams").ToList();
            var actualResult2 = Queryable.FromSql("sp_NoParams").ToList();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EquivalentTo(expectedResult));
                Assert.That(actualResult2, Is.EquivalentTo(actualResult1));
            });
        }

        [Test]
        public virtual void FromSql_QueryProviderWithManyFromSqlResults_ReturnsExpectedResults()
        {
            var sql1 = "sp_NoParams";
            var expectedResult1 = new Fixture().CreateMany<T>().ToList();

            var sql2 = "sp_WithParams";
            var parameters2 = new List<SqlParameter> {new SqlParameter("@SomeParameter1", "Value1"), new SqlParameter("@SomeParameter2", "Value2")};
            var expectedResult2 = new Fixture().CreateMany<T>().ToList();

            AddFromSqlResult(Queryable, sql1, expectedResult1);
            AddFromSqlResult(Queryable, sql2, parameters2, expectedResult2);

            Logger.LogDebug("actualResult1");
            var actualResult1 = Queryable.FromSql("[dbo].[sp_NoParams]").ToList();
            Logger.LogDebug("actualResult2");
            var actualResult2 = Queryable.FromSql("sp_NoParams").ToList();

            Logger.LogDebug("actualResult3");
            var actualResult3 = Queryable.FromSql("[dbo].[sp_WithParams]").ToList();
            Logger.LogDebug("actualResult4");
            var actualResult4 = Queryable.FromSql("sp_WithParams @SomeParameter1 @SomeParameter2").ToList();

            Logger.LogDebug("actualResult5");
            var actualResult5 = Queryable.FromSql("[dbo].[sp_WithParams]", new List<SqlParameter> {new SqlParameter("@someparameter2", "value2")}).ToList();
            Logger.LogDebug("actualResult6");
            var actualResult6 = Queryable.FromSql("sp_WithParams @SomeParameter1 @SomeParameter2", new List<SqlParameter> {new SqlParameter("@someparameter2", "value2")}).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EquivalentTo(expectedResult1));
                Assert.That(actualResult2, Is.EquivalentTo(actualResult1));

                Assert.That(actualResult3, Is.EquivalentTo(expectedResult2));
                Assert.That(actualResult4, Is.EquivalentTo(actualResult3));

                Assert.That(actualResult5, Is.EquivalentTo(expectedResult2));
                Assert.That(actualResult6, Is.EquivalentTo(actualResult5));
            });
        }

        [Test]
        public virtual void FromSql_SpecifiedSql_ReturnsExpectedResult()
        {
            var sql = "sp_NoParams";
            var expectedResult = new Fixture().CreateMany<T>().ToList();
            AddFromSqlResult(Queryable, sql, expectedResult);

            var actualResult1 = Queryable.FromSql("[dbo].[sp_NoParams]").ToList();
            var actualResult2 = Queryable.FromSql("sp_NoParams").ToList();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EquivalentTo(expectedResult));
                Assert.That(actualResult2, Is.EquivalentTo(actualResult1));
            });
        }

        [Test]
        public virtual void FromSql_SpecifiedSqlThatDoesNotMatchSetUp_ThrowsException()
        {
            var sql = "asdf";
            var expectedResult = new Fixture().CreateMany<T>().ToList();
            AddFromSqlResult(Queryable, sql, expectedResult);

            Assert.Throws<NotSupportedException>(() =>
            {
                var actualResult = Queryable.FromSql("sp_NoParams").ToList();
            });
        }

        [Test]
        public virtual void FromSql_SpecifiedSqlWithParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<SqlParameter> {new SqlParameter("@SomeParameter2", "Value2")};
            var expectedResult = new Fixture().CreateMany<T>().ToList();
            AddFromSqlResult(Queryable, sql, parameters, expectedResult);

            var actualResult1 = Queryable.FromSql("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", parameters.ToArray()).ToList();
            var actualResult2 = Queryable.FromSql("sp_WithParams @SomeParameter1 @SomeParameter2", parameters.ToArray()).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EquivalentTo(expectedResult));
                Assert.That(actualResult2, Is.EquivalentTo(actualResult1));
            });
        }
    }
}