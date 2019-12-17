using System;
using System.Collections.Generic;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.Moq.Extensions;
using EntityFrameworkCore.Testing.Moq.Helpers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    [TestFixture]
    public class DbContextTestsUsingFactory : DbContextTestsBase<TestDbContext>
    {
        [SetUp]
        public override void SetUp()
        {
            TestDbContext Factory()
            {
                return new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            }

            MockedDbContext = Create.MockedDbContextFor(Factory);
            base.SetUp();
        }

        public override void AddExecuteSqlCommandResult(TestDbContext mockedDbContext, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlCommandResult(expectedResult);
        }

        public override void AddExecuteSqlCommandResult(TestDbContext mockedDbContext, string sql, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlCommandResult(sql, expectedResult);
        }

        public override void AddExecuteSqlCommandResult(TestDbContext mockedDbContext, string sql, IEnumerable<object> parameters, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlCommandResult(sql, parameters, expectedResult);
        }

        public override void AddExecuteSqlInterpolatedResult(TestDbContext mockedDbContext, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlInterpolatedResult(expectedResult);
        }

        public override void AddExecuteSqlInterpolatedResult(TestDbContext mockedDbContext, FormattableString sql, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlInterpolatedResult(sql, expectedResult);
        }

        public override void AddExecuteSqlInterpolatedResult(TestDbContext mockedDbContext, string sql, IEnumerable<object> parameters, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlInterpolatedResult(sql, parameters, expectedResult);
        }

        public override void AddExecuteSqlRawResult(TestDbContext mockedDbContext, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlRawResult(expectedResult);
        }

        public override void AddExecuteSqlRawResult(TestDbContext mockedDbContext, string sql, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlRawResult(sql, expectedResult);
        }

        public override void AddExecuteSqlRawResult(TestDbContext mockedDbContext, string sql, IEnumerable<object> parameters, int expectedResult)
        {
            mockedDbContext.AddExecuteSqlRawResult(sql, parameters, expectedResult);
        }
    }
}