using System;
using System.Collections.Generic;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    public abstract class BaseForDbContextTests<T> : Common.Tests.BaseForDbContextTests<T> where T : DbContext
    {
        public override void AddExecuteSqlCommandResult(T mockedDbContext, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlCommandResult(expectedResult);
        }

        public override void AddExecuteSqlCommandResult(T mockedDbContext, int expectedResult, Action<string, IEnumerable<object>> callback)
        {
            mockedDbContext.AddExecuteSqlCommandResult(expectedResult, callback);
        }

        public override void AddExecuteSqlCommandResult(T mockedDbContext, string sql, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlCommandResult(sql, expectedResult);
        }

        public override void AddExecuteSqlCommandResult(T mockedDbContext, string sql, int expectedResult, Action<string, IEnumerable<object>> callback)
        {
            mockedDbContext.AddExecuteSqlCommandResult(sql, expectedResult, callback);
        }

        public override void AddExecuteSqlCommandResult(T mockedDbContext, string sql, IEnumerable<object> parameters, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlCommandResult(sql, parameters, expectedResult);
        }

        public override void AddExecuteSqlCommandResult(
            T mockedDbContext, string sql, IEnumerable<object> parameters, int expectedResult, Action<string, IEnumerable<object>> callback)
        {
            mockedDbContext.AddExecuteSqlCommandResult(sql, parameters, expectedResult, callback);
        }
    }
}