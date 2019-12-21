using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
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
        protected abstract void AddFromSqlResult(IQueryable<T> mockedQueryable, IEnumerable<T> expectedResult);
        protected abstract void AddFromSqlResult(IQueryable<T> mockedQueryable, string sql, IEnumerable<T> expectedResult);
        protected abstract void AddFromSqlResult(IQueryable<T> mockedQueryable, string sql, IEnumerable<object> parameters, IEnumerable<T> expectedResult);

        [Test]
        public virtual void FromSql_AnySql_ReturnsExpectedResult()
        {
            var expectedResult = Fixture.CreateMany<T>().ToList();
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
            var expectedResult1 = Fixture.CreateMany<T>().ToList();

            var sql2 = "sp_WithParams";
            var parameters2 = new List<SqlParameter> {new SqlParameter("@SomeParameter1", "Value1"), new SqlParameter("@SomeParameter2", "Value2")};
            var expectedResult2 = Fixture.CreateMany<T>().ToList();

            AddFromSqlResult(Queryable, sql1, expectedResult1);
            AddFromSqlResult(Queryable, sql2, parameters2, expectedResult2);

            Logger.LogDebug("actualResult1");
            var actualResult1 = Queryable.FromSql("[dbo].[sp_NoParams]").ToList();

            Logger.LogDebug("actualResult2");
            var actualResult2 = Queryable.FromSql("[dbo].[sp_WithParams]", parameters2.ToArray()).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EquivalentTo(expectedResult1));
                Assert.That(actualResult2, Is.EquivalentTo(expectedResult2));
            });
        }

        [Test]
        public virtual void FromSql_SpecifiedSql_ReturnsExpectedResult()
        {
            var sql = "sp_NoParams";
            var expectedResult = Fixture.CreateMany<T>().ToList();
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
            var expectedResult = Fixture.CreateMany<T>().ToList();
            AddFromSqlResult(Queryable, sql, expectedResult);

            Assert.Throws<NotSupportedException>(() =>
            {
                var actualResult = Queryable.FromSql("sp_NoParams").ToList();
            });
        }

        [Test]
        public virtual void FromSql_SpecifiedSqlWithSqlParameterParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<SqlParameter> {new SqlParameter("@SomeParameter2", "Value2")};
            var expectedResult = Fixture.CreateMany<T>().ToList();
            AddFromSqlResult(Queryable, sql, parameters, expectedResult);

            var actualResult1 = Queryable.FromSql("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", parameters.ToArray()).ToList();
            var actualResult2 = Queryable.FromSql("sp_WithParams @SomeParameter1 @SomeParameter2", parameters.ToArray()).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EquivalentTo(expectedResult));
                Assert.That(actualResult2, Is.EquivalentTo(actualResult1));
            });
        }

        [Test]
        public virtual void FromSql_SpecifiedSqlWithSqlParameterParametersThatDoNotMatchSetUp_ThrowsException()
        {
            var sql = "sp_WithParams";
            var setUpParameters = new List<SqlParameter> {new SqlParameter("@SomeParameter3", "Value3")};
            var invocationParameters = new List<SqlParameter> {new SqlParameter("@SomeParameter1", "Value1"), new SqlParameter("@SomeParameter2", "Value2")};
            var expectedResult = Fixture.CreateMany<T>().ToList();
            AddFromSqlResult(Queryable, sql, setUpParameters, expectedResult);

            Assert.Throws<NotSupportedException>(() =>
            {
                var actualResult1 = Queryable.FromSql("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", invocationParameters.ToArray()).ToList();
            });

            Assert.Throws<NotSupportedException>(() =>
            {
                var actualResult2 = Queryable.FromSql("sp_WithParams @SomeParameter1 @SomeParameter2", invocationParameters.ToArray()).ToList();
            });
        }

        [Test]
        public virtual void FromSql_SpecifiedSqlWithStringParameterParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<string> {"Value2"};
            var expectedResult = Fixture.CreateMany<T>().ToList();
            AddFromSqlResult(Queryable, sql, parameters, expectedResult);

            var actualResult1 = Queryable.FromSql("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", parameters.ToArray()).ToList();
            var actualResult2 = Queryable.FromSql("sp_WithParams @SomeParameter1 @SomeParameter2", parameters.ToArray()).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EquivalentTo(expectedResult));
                Assert.That(actualResult2, Is.EquivalentTo(actualResult1));
            });
        }

        [Test]
        public virtual void FromSql_ThrowsException()
        {
            Assert.Throws<NotSupportedException>(() =>
            {
                var actualResult = Queryable.FromSql("sp_NoParams").ToList();
            });
        }

        [Test]
        public virtual async Task FromSqlThenFirstOrDefaultAsync_ReturnsFirstElement()
        {
            var sql = "sp_NoParams";
            var expectedResult = Fixture.CreateMany<T>().ToList();
            AddFromSqlResult(Queryable, sql, expectedResult);

            var actualResult1 = await Queryable.FromSql("[dbo].[sp_NoParams]").FirstOrDefaultAsync();
            var actualResult2 = await Queryable.FromSql("[dbo].[sp_NoParams]").FirstOrDefaultAsync();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult.First()));
                Assert.That(actualResult2, Is.EqualTo(expectedResult.First()));
            });
        }
    }
}