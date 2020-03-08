using System;
using System.Collections.Generic;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    [TestFixture]
    public class BaseForDbContextTests<T> : Common.Tests.BaseForDbContextTests<T> where T : DbContext
    {
        public override void AddExecuteSqlCommandResult(T mockedDbContext, int expectedResult)
        {
#pragma warning disable 618
            mockedDbContext.AddExecuteSqlCommandResult(expectedResult);
#pragma warning restore 618
        }

        public override void AddExecuteSqlCommandResult(T mockedDbContext, int expectedResult, Action<string, IEnumerable<object>> callback)
        {
#pragma warning disable 618
            mockedDbContext.AddExecuteSqlCommandResult(expectedResult, callback);
#pragma warning restore 618
        }

        public override void AddExecuteSqlCommandResult(T mockedDbContext, string sql, int expectedResult)
        {
#pragma warning disable 618
            mockedDbContext.AddExecuteSqlCommandResult(sql, expectedResult);
#pragma warning restore 618
        }

        public override void AddExecuteSqlCommandResult(T mockedDbContext, string sql, int expectedResult, Action<string, IEnumerable<object>> callback)
        {
#pragma warning disable 618
            mockedDbContext.AddExecuteSqlCommandResult(sql, expectedResult, callback);
#pragma warning restore 618
        }

        public override void AddExecuteSqlCommandResult(T mockedDbContext, string sql, IEnumerable<object> parameters, int expectedResult)
        {
#pragma warning disable 618
            mockedDbContext.AddExecuteSqlCommandResult(sql, parameters, expectedResult);
#pragma warning restore 618
        }

        public override void AddExecuteSqlCommandResult(
            T mockedDbContext, string sql, IEnumerable<object> parameters, int expectedResult, Action<string, IEnumerable<object>> callback)
        {
#pragma warning disable 618
            mockedDbContext.AddExecuteSqlCommandResult(sql, parameters, expectedResult, callback);
#pragma warning restore 618
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