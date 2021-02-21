using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.Moq.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    public abstract class BaseForDbQueryTests<T> : Common.Tests.BaseForDbQueryTests<T> where T : BaseTestEntity
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            MockedDbContext = Create.MockedDbContextFor<TestDbContext>();
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