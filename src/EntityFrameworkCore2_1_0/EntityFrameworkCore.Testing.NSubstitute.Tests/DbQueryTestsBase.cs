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
    public abstract class DbQueryTestsBase : DbQueryTestsBase<TestQuery>
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            MockedDbContext = Create.SubstituteFor(dbContextToMock);
        }

        protected TestDbContext MockedDbContext;

        protected override void AddFromSqlResult(IQueryable<TestQuery> mockedQueryable, IEnumerable<TestQuery> expectedResult)
        {
            mockedQueryable.AddFromSqlResult(expectedResult);
        }

        protected override void AddFromSqlResult(IQueryable<TestQuery> mockedQueryable, string sql, IEnumerable<TestQuery> expectedResult)
        {
            mockedQueryable.AddFromSqlResult(sql, expectedResult);
        }

        protected override void AddFromSqlResult(IQueryable<TestQuery> mockedQueryable, string sql, List<SqlParameter> parameters, IEnumerable<TestQuery> expectedResult)
        {
            mockedQueryable.AddFromSqlResult(sql, parameters, expectedResult);
        }

        protected override void AddToReadOnlySource(DbQuery<TestQuery> mockedDbQuery, TestQuery item)
        {
            mockedDbQuery.AddToReadOnlySource(item);
        }

        protected override void AddRangeToReadOnlySource(DbQuery<TestQuery> mockedDbQuery, IEnumerable<TestQuery> enumerable)
        {
            mockedDbQuery.AddRangeToReadOnlySource(enumerable);
        }

        protected override void ClearReadOnlySource(DbQuery<TestQuery> mockedDbQuery)
        {
            mockedDbQuery.ClearReadOnlySource();
        }
    }
}