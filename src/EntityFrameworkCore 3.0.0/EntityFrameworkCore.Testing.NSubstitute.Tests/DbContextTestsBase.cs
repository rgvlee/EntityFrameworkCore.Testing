using System;
using System.Collections.Generic;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    [TestFixture]
    public class DbContextTestsBase<T> : Common.Tests.DbContextTestsBase<T> where T : DbContext
    {
        public override void AddExecuteSqlCommandResult(T mockedDbContext, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlCommandResult(expectedResult);
        }

        public override void AddExecuteSqlCommandResult(T mockedDbContext, int expectedResult, Action callback)
        {
            mockedDbContext.AddExecuteSqlCommandResult(expectedResult, callback);
        }

        public override void AddExecuteSqlCommandResult(T mockedDbContext, string sql, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlCommandResult(sql, expectedResult);
        }

        public override void AddExecuteSqlCommandResult(T mockedDbContext, string sql, int expectedResult, Action callback)
        {
            mockedDbContext.AddExecuteSqlCommandResult(sql, expectedResult, callback);
        }

        public override void AddExecuteSqlCommandResult(T mockedDbContext, string sql, IEnumerable<object> parameters, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlCommandResult(sql, parameters, expectedResult);
        }

        public override void AddExecuteSqlCommandResult(T mockedDbContext, string sql, IEnumerable<object> parameters, int expectedResult, Action callback)
        {
            mockedDbContext.AddExecuteSqlCommandResult(sql, parameters, expectedResult, callback);
        }

        public override void AddExecuteSqlInterpolatedResult(T mockedDbContext, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlInterpolatedResult(expectedResult);
        }

        public override void AddExecuteSqlInterpolatedResult(T mockedDbContext, FormattableString sql, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlInterpolatedResult(sql, expectedResult);
        }

        public override void AddExecuteSqlInterpolatedResult(T mockedDbContext, string sql, IEnumerable<object> parameters, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlInterpolatedResult(sql, parameters, expectedResult);
        }

        public override void AddExecuteSqlRawResult(T mockedDbContext, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlRawResult(expectedResult);
        }

        public override void AddExecuteSqlRawResult(T mockedDbContext, string sql, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlRawResult(sql, expectedResult);
        }

        public override void AddExecuteSqlRawResult(T mockedDbContext, string sql, IEnumerable<object> parameters, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlRawResult(sql, parameters, expectedResult);
        }
    }
}