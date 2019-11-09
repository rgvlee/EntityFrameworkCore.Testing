using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    [TestFixture]
    public abstract class ExecuteSqlInterpolatedTestsBase<TDbContext> : TestBase
        where TDbContext : DbContext
    {
        protected TDbContext MockedDbContext;

        public abstract void AddExecuteSqlInterpolatedResult(TDbContext mockedDbContext, int expectedResult);
        public abstract void AddExecuteSqlInterpolatedResult(TDbContext mockedDbContext, string sql, int expectedResult);
        public abstract void AddExecuteSqlInterpolatedResult(TDbContext mockedDbContext, FormattableString sql, int expectedResult);

        [Test]
        public void ExecuteSqlInterpolated_AnySql_ReturnsExpectedResult()
        {
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlInterpolated($"sp_NoParams");
            var actualResult2 = MockedDbContext.Database.ExecuteSqlInterpolated($"sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlInterpolated_SpecifiedSql_ReturnsExpectedResult()
        {
            var sql = "sp_NoParams";
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlInterpolated($"sp_NoParams");
            var actualResult2 = MockedDbContext.Database.ExecuteSqlInterpolated($"sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlInterpolated_SpecifiedSqlThatDoesNotMatchSetUp_ThrowsException()
        {
            var sql = "asdf";
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, expectedResult);

            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult = MockedDbContext.Database.ExecuteSqlInterpolated($"sp_NoParams");
            });
        }

        [Test]
        public void ExecuteSqlInterpolated_SpecifiedSqlWithSqlParameterParameters_ReturnsExpectedResult()
        {
            var parameters = new List<SqlParameter> { new SqlParameter("@SomeParameter2", "Value2") };
            var sql = (FormattableString)$"[sp_WithParams] {parameters[0]}";
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlInterpolated($"[dbo].[sp_WithParams] {parameters[0]}");
            var actualResult2 = MockedDbContext.Database.ExecuteSqlInterpolated($"[dbo].[sp_WithParams] {parameters[0]}");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlInterpolated_SpecifiedSqlWithStringParameterParameters_ReturnsExpectedResult()
        {
            var parameters = new List<string> { "Value2" };
            var sql = (FormattableString)$"[sp_WithParams] {parameters[0]}";
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, expectedResult);

            var actualResult1 = MockedDbContext.Database.ExecuteSqlInterpolated($"[dbo].[sp_WithParams] {parameters[0]}");
            var actualResult2 = MockedDbContext.Database.ExecuteSqlInterpolated($"[dbo].[sp_WithParams] {parameters[0]}");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlInterpolated_SpecifiedSqlWithParametersThatDoNotMatchSetUp_ThrowsException()
        {
            var setUpParameters = new List<SqlParameter> { new SqlParameter("@SomeParameter3", "Value3") };
            var invocationParameters = new List<SqlParameter> { new SqlParameter("@SomeParameter1", "Value1"), new SqlParameter("@SomeParameter2", "Value2") };
            var sql = (FormattableString)$"[sp_WithParams] {setUpParameters[0]}";
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, expectedResult);

            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult1 = MockedDbContext.Database.ExecuteSqlInterpolated($"[dbo].[sp_WithParams] {invocationParameters[0]}");
            });

            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult2 = MockedDbContext.Database.ExecuteSqlInterpolated($"[dbo].[sp_WithParams] {invocationParameters[0]}");
            });
        }

        [Test]
        public void ExecuteSqlInterpolated_WithNoMatchesAdded_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult = MockedDbContext.Database.ExecuteSqlInterpolated($"sp_NoParams");
            });
        }

        [Test]
        public async Task ExecuteSqlInterpolatedAsync_AnySql_ReturnsExpectedResult()
        {
            var sql = string.Empty;
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, expectedResult);

            var actualResult1 = await MockedDbContext.Database.ExecuteSqlInterpolatedAsync($"sp_NoParams");
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlInterpolatedAsync($"sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public async Task ExecuteSqlInterpolatedAsync_SpecifiedSql_ReturnsExpectedResult()
        {
            var sql = "sp_NoParams";
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, expectedResult);

            var actualResult1 = await MockedDbContext.Database.ExecuteSqlInterpolatedAsync($"sp_NoParams");
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlInterpolatedAsync($"sp_NoParams");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public void ExecuteSqlInterpolatedAsync_SpecifiedSqlThatDoesNotMatchSetUp_ThrowsException()
        {
            var sql = "asdf";
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, expectedResult);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                var actualResult = await MockedDbContext.Database.ExecuteSqlInterpolatedAsync($"sp_NoParams");
            });
        }

        [Test]
        public async Task ExecuteSqlInterpolatedAsync_SpecifiedSqlWithSqlParameterParameters_ReturnsExpectedResult()
        {
            var parameters = new List<SqlParameter> { new SqlParameter("@SomeParameter2", "Value2") };
            var sql = (FormattableString)$"[sp_WithParams] {parameters[0]}";
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, expectedResult);

            var actualResult1 = await MockedDbContext.Database.ExecuteSqlInterpolatedAsync($"[dbo].[sp_WithParams] {parameters[0]}");
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlInterpolatedAsync($"[dbo].[sp_WithParams] {parameters[0]}");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }

        [Test]
        public async Task ExecuteSqlInterpolatedAsync_SpecifiedSqlWithStringParameterParameters_ReturnsExpectedResult()
        {
            var parameters = new List<string> { "Value2" };
            var sql = (FormattableString)$"[sp_WithParams] {parameters[0]}";
            var expectedResult = 1;
            AddExecuteSqlInterpolatedResult(MockedDbContext, sql, expectedResult);

            var actualResult1 = await MockedDbContext.Database.ExecuteSqlInterpolatedAsync($"[dbo].[sp_WithParams] {parameters[0]}");
            var actualResult2 = await MockedDbContext.Database.ExecuteSqlInterpolatedAsync($"[dbo].[sp_WithParams] {parameters[0]}");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EqualTo(expectedResult));
                Assert.That(actualResult2, Is.EqualTo(actualResult1));
            });
        }
    }
}