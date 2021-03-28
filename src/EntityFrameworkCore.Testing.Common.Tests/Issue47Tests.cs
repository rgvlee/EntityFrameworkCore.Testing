using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ScaffoldingTester.Models;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public abstract class Issue47Tests<TDbContext, TEntity> : BaseForTests 
        where TDbContext : DbContext
        where TEntity : BaseTestEntity
    {
        protected TDbContext MockedDbContext;

        protected abstract void AddExecuteSqlRawResult(string sql, IEnumerable<object> parameters, int expectedResult);

        protected abstract void AddFromSqlRawResult(string sql, IEnumerable<object> parameters, IEnumerable<TEntity> expectedResult);

        [Test]
        public virtual void GetCommandTimeout_ReturnsZero()
        {
            var actualResult = MockedDbContext.Database.GetCommandTimeout();

            Assert.That(actualResult, Is.EqualTo(0));
        }

        [Test]
        public virtual async Task SqlQueryAsync_InvokesExecuteSqlRawAsyncWithSpecifiedSqlWithSqlParameterParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new object[] { new SqlParameter("@SomeParameter2", "Value2") };
            var expectedResult = 1;
            AddExecuteSqlRawResult(sql, parameters, expectedResult);

            var actualResult1 = await MockedDbContext.SqlQueryAsync<object>("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", parameters);
            var actualResult2 = await MockedDbContext.SqlQueryAsync<object>("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", parameters);

            Assert.Multiple(() =>
            {
                actualResult1.Should().BeEquivalentTo(default);
                actualResult2.Should().BeEquivalentTo(default);
            });
        }

        [Test]
        public virtual async Task SqlQueryAsync_InvokesFromSqlRawWithSpecifiedSqlWithStringParameters_ReturnsExpectedResult()
        {
            var sql = "sp_WithParams";
            var parameters = new List<string> { "Value2" };
            var expectedResult = Fixture.CreateMany<TEntity>().ToList();
            AddFromSqlRawResult(sql, parameters, expectedResult);

            var actualResult1 = await MockedDbContext.SqlQueryAsync<TEntity>("[dbo].[sp_WithParams] @SomeParameter1 @SomeParameter2", parameters.ToArray());
            var actualResult2 = await MockedDbContext.SqlQueryAsync<TEntity>("sp_WithParams @SomeParameter1 @SomeParameter2", parameters.ToArray());

            Assert.Multiple(() =>
            {
                actualResult1.Should().BeEquivalentTo(expectedResult);
                actualResult2.Should().BeEquivalentTo(expectedResult);
            });
        }
    }
}