using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    [TestFixture]
    public abstract class ExecuteSqlRawTestsBase<TDbContext> : TestBase
        where TDbContext : DbContext
    {
        protected TDbContext MockedDbContext;

        public abstract void AddExecuteSqlRawResult(TDbContext mockedDbContext, int expectedResult);
        public abstract void AddExecuteSqlRawResult(TDbContext mockedDbContext, string sql, int expectedResult);
        public abstract void AddExecuteSqlRawResult(TDbContext mockedDbContext, string sql, List<SqlParameter> parameters, int expectedResult);

        [Test]
        public void ExecuteSqlRaw_AnySql_ReturnsExpectedResult()
        {
            var expectedResult = 1;
            AddExecuteSqlRawResult(MockedDbContext, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlRaw("sp_NoParams");
            var actualResult2 = MockedDbContext.Database.ExecuteSqlRaw("sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlRaw_SpecifiedSql_ReturnsExpectedResult()
        {
            var sql = "sp_NoParams";
            var expectedResult = 1;
            AddExecuteSqlRawResult(MockedDbContext, sql, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlRaw("sp_NoParams");
            var actualResult2 = MockedDbContext.Database.ExecuteSqlRaw("sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlRaw_SpecifiedSqlThatDoesNotMatchSetUp_ThrowsException()
        {
            var sql = "asdf";
            var expectedResult = 1;
            AddExecuteSqlRawResult(MockedDbContext, sql, expectedResult);

            Assert.Throws<NullReferenceException>(() =>
            {
                var actualResult = MockedDbContext.Database.ExecuteSqlRaw("sp_NoParams");
            });
        }

        [Test]
        public void ExecuteSqlRaw_SpecifiedSqlWithParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<SqlParameter> {new SqlParameter("@SomeParameter2", "Value2")};
            var expectedResult = 1;
            AddExecuteSqlRawResult(MockedDbContext, sql, parameters, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlRaw("[dbo.[sp_WithParams] @SomeParameter1 @SomeParameter2", parameters);
            var actualResult2 = MockedDbContext.Database.ExecuteSqlRaw("[dbo.[sp_WithParams] @SomeParameter1 @SomeParameter2", parameters);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlRaw_WithNoMatchesAdded_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult = MockedDbContext.Database.ExecuteSqlRaw("sp_NoParams");
            });
        }

        [Test]
        public async Task ExecuteSqlRawAsync_AnySql_ReturnsExpectedResult()
        {
            var sql = "";
            var expectedResult = 1;
            AddExecuteSqlRawResult(MockedDbContext, sql, expectedResult);

            var actualResult1 = await MockedDbContext.Database.ExecuteSqlRawAsync("sp_NoParams");
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlRawAsync("sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public async Task ExecuteSqlRawAsync_SpecifiedSql_ReturnsExpectedResult()
        {
            var sql = "sp_NoParams";
            var expectedResult = 1;
            AddExecuteSqlRawResult(MockedDbContext, sql, expectedResult);

            var actualResult1 = await MockedDbContext.Database.ExecuteSqlRawAsync("sp_NoParams");
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlRawAsync("sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlRawAsync_SpecifiedSqlThatDoesNotMatchSetUp_ThrowsException()
        {
            var sql = "asdf";
            var expectedResult = 1;
            AddExecuteSqlRawResult(MockedDbContext, sql, expectedResult);

            Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                var actualResult = await MockedDbContext.Database.ExecuteSqlRawAsync("sp_NoParams");
            });
        }

        [Test]
        public async Task ExecuteSqlRawAsync_SpecifiedSqlWithParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<SqlParameter> {new SqlParameter("@SomeParameter2", "Value2")};
            var expectedResult = 1;
            AddExecuteSqlRawResult(MockedDbContext, sql, parameters, expectedResult);

            var actualResult1 = await MockedDbContext.Database.ExecuteSqlRawAsync("[dbo.[sp_WithParams] @SomeParameter2", parameters);
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlRawAsync("[dbo.[sp_WithParams] @SomeParameter2", parameters);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }
    }
}