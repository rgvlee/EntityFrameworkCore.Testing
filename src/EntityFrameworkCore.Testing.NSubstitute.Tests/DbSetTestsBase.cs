using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using EntityFrameworkCore.Testing.NSubstitute.Helpers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    [TestFixture]
    public abstract class DbSetTestsBase : DbSetTestsBase<TestDbContext, TestEntity1>
    {
        protected override TestDbContext CreateMockedDbContext()
        {
            return Create.MockedDbContextFor(new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options));
        }

        protected override void AddFromSqlResult(IQueryable<TestEntity1> queryable, IEnumerable<TestEntity1> expectedResult)
        {
            queryable.AddFromSqlResult(expectedResult);
        }

        protected override void AddFromSqlResult(IQueryable<TestEntity1> queryable, string sql, IEnumerable<TestEntity1> expectedResult)
        {
            queryable.AddFromSqlResult(sql, expectedResult);
        }

        protected override void AddFromSqlResult(IQueryable<TestEntity1> queryable, string sql, List<SqlParameter> parameters, IEnumerable<TestEntity1> expectedResult)
        {
            queryable.AddFromSqlResult(sql, parameters, expectedResult);
        }
    }
}