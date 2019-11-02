using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.Moq.Extensions;
using EntityFrameworkCore.Testing.Moq.Helpers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    [TestFixture]
    public abstract class DbSetTestsBase<T> : DbSetTestsBase<TestDbContext, T> where T : TestEntityBase
    {
        protected override TestDbContext CreateMockedDbContext()
        {
            return Create.MockedDbContextFor(new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options));
        }

        protected override void AddFromSqlResult(IQueryable<T> queryable, IEnumerable<T> expectedResult)
        {
            queryable.AddFromSqlResult(expectedResult);
        }

        protected override void AddFromSqlResult(IQueryable<T> queryable, string sql, IEnumerable<T> expectedResult)
        {
            queryable.AddFromSqlResult(sql, expectedResult);
        }

        protected override void AddFromSqlResult(IQueryable<T> queryable, string sql, List<SqlParameter> parameters, IEnumerable<T> expectedResult)
        {
            queryable.AddFromSqlResult(sql, parameters, expectedResult);
        }
    }
}