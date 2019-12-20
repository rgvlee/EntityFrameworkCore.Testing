using System;
using System.Collections.Generic;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    [TestFixture]
    public class DbContextTestsBase<T> : Common.Tests.DbContextTestsBase<TestDbContext> where T : DbContext
    {
        public override void AddExecuteSqlCommandResult(TestDbContext mockedDbContext, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlCommandResult(expectedResult);
        }

        public override void AddExecuteSqlCommandResult(TestDbContext mockedDbContext, int expectedResult, Action callback)
        {
            mockedDbContext.AddExecuteSqlCommandResult(expectedResult, callback);
        }

        public override void AddExecuteSqlCommandResult(TestDbContext mockedDbContext, string sql, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlCommandResult(sql, expectedResult);
        }

        public override void AddExecuteSqlCommandResult(TestDbContext mockedDbContext, string sql, int expectedResult, Action callback)
        {
            mockedDbContext.AddExecuteSqlCommandResult(sql, expectedResult, callback);
        }

        public override void AddExecuteSqlCommandResult(TestDbContext mockedDbContext, string sql, IEnumerable<object> parameters, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlCommandResult(sql, parameters, expectedResult);
        }

        public override void AddExecuteSqlCommandResult(TestDbContext mockedDbContext, string sql, IEnumerable<object> parameters, int expectedResult, Action callback)
        {
            mockedDbContext.AddExecuteSqlCommandResult(sql, parameters, expectedResult, callback);
        }
    }
}