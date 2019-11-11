using System;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.Moq.Extensions;
using EntityFrameworkCore.Testing.Moq.Helpers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    [TestFixture]
    public abstract class DbQueryTestsBase<T> : Common.Tests.DbQueryTestsBase<T> where T : TestEntityBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            MockedDbContext = Create.MockedDbContextFor(dbContextToMock);
        }

        protected TestDbContext MockedDbContext;

        protected override void AddFromSqlResult(IQueryable<T> mockedQueryable, IEnumerable<T> expectedResult)
        {
            mockedQueryable.AddFromSqlResult(expectedResult);
        }

        protected override void AddFromSqlResult(IQueryable<T> mockedQueryable, string sql, IEnumerable<T> expectedResult)
        {
            mockedQueryable.AddFromSqlResult(sql, expectedResult);
        }

        protected override void AddFromSqlResult(IQueryable<T> mockedQueryable, string sql, IEnumerable<object> parameters, IEnumerable<T> expectedResult)
        {
            mockedQueryable.AddFromSqlResult(sql, parameters, expectedResult);
        }

        protected override void AddToReadOnlySource(DbQuery<T> mockedDbQuery, T item)
        {
            mockedDbQuery.AddToReadOnlySource(item);
        }

        protected override void AddRangeToReadOnlySource(DbQuery<T> mockedDbQuery, IEnumerable<T> items)
        {
            mockedDbQuery.AddRangeToReadOnlySource(items);
        }

        protected override void ClearReadOnlySource(DbQuery<T> mockedDbQuery)
        {
            mockedDbQuery.ClearReadOnlySource();
        }
    }
}