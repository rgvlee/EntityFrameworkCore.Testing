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
    public abstract class DbQueryTestsBase<T> : ReadOnlyDbSetTestsBase<T> where T : TestEntityBase
    {
        [SetUp]
        public override void SetUp()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            MockedDbContext = Create.MockedDbContextFor(dbContextToMock);
            base.SetUp();
        }

        protected TestDbContext MockedDbContext;

        protected override void AddFromSqlRawResult(IQueryable<T> mockedQueryable, IEnumerable<T> expectedResult)
        {
            mockedQueryable.AddFromSqlRawResult(expectedResult);
        }

        protected override void AddFromSqlRawResult(IQueryable<T> mockedQueryable, string sql, IEnumerable<T> expectedResult)
        {
            mockedQueryable.AddFromSqlRawResult(sql, expectedResult);
        }

        protected override void AddFromSqlRawResult(IQueryable<T> mockedQueryable, string sql, IEnumerable<object> parameters, IEnumerable<T> expectedResult)
        {
            mockedQueryable.AddFromSqlRawResult(sql, parameters, expectedResult);
        }

        protected override void AddToReadOnlySource(DbSet<T> mockedDbQuery, T item)
        {
            mockedDbQuery.AddToReadOnlySource(item);
        }

        protected override void AddRangeToReadOnlySource(DbSet<T> mockedDbQuery, IEnumerable<T> items)
        {
            mockedDbQuery.AddRangeToReadOnlySource(items);
        }

        protected override void ClearReadOnlySource(DbSet<T> mockedDbQuery)
        {
            mockedDbQuery.ClearReadOnlySource();
        }
    }
}