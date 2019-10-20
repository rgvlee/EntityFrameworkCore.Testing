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
    public abstract class DbQueryTestsBase : DbQueryTestsBase<TestDbContext, TestQuery1>
    {
        protected override TestDbContext CreateMockedDbContext()
        {
            return Create.MockedDbContextFor(new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options));
        }

        protected override void AddFromSqlResult(IQueryable<TestQuery1> mockedQueryable, IEnumerable<TestQuery1> expectedResult)
        {
            mockedQueryable.AddFromSqlResult(expectedResult);
        }

        protected override void AddFromSqlResult(IQueryable<TestQuery1> mockedQueryable, string sql, IEnumerable<TestQuery1> expectedResult)
        {
            mockedQueryable.AddFromSqlResult(sql, expectedResult);
        }

        protected override void AddFromSqlResult(IQueryable<TestQuery1> mockedQueryable, string sql, List<SqlParameter> parameters, IEnumerable<TestQuery1> expectedResult)
        {
            mockedQueryable.AddFromSqlResult(sql, parameters, expectedResult);
        }

        protected override void Add(DbQuery<TestQuery1> mockedDbQuery, TestQuery1 item)
        {
            mockedDbQuery.Add(item);
        }

        protected override void AddRange(DbQuery<TestQuery1> mockedDbQuery, IEnumerable<TestQuery1> sequence)
        {
            mockedDbQuery.AddRange(sequence);
        }

        protected override void Clear(DbQuery<TestQuery1> mockedDbQuery)
        {
            mockedDbQuery.Clear();
        }
    }
}