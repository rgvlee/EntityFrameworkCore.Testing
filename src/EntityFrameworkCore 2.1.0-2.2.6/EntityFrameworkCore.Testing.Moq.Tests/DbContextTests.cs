﻿using System;
using System.Collections.Generic;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.Moq.Extensions;
using EntityFrameworkCore.Testing.Moq.Helpers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    [TestFixture]
    public class DbContextTests : DbContextTestsBase<TestDbContext>
    {
        [SetUp]
        public override void SetUp()
        {
            var dbContextToMock = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            MockedDbContext = Create.MockedDbContextFor(dbContextToMock);
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
    }
}