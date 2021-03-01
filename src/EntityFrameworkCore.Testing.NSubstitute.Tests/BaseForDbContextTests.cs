using System;
using System.Collections.Generic;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    public abstract class BaseForDbContextTests<T> : Common.Tests.BaseForDbContextTests<T> where T : DbContext
    {
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