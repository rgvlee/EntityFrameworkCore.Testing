using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.Moq.Extensions;
using EntityFrameworkCore.Testing.Moq.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        [Test]
        public void ExecuteSqlCommand_CallbackSpecified_InvokesCallback()
        {
            var sql = "sp_NoParams";
            var expectedResult = 1;
            var itemsToCreate = 3;
            var source = Fixture.CreateMany<string>(itemsToCreate).ToList();

            var preSetUpFirst = source.First();
            var preSetUpCount = source.Count;
            
            Logger.LogDebug($"Setting up ExecuteSqlCommand");
            MockedDbContext.AddExecuteSqlCommandResult(sql, expectedResult, () =>
            {
                Logger.LogDebug($"Before callback invoked: {source.Count}");
                source = source.Take(1).ToList();
                Logger.LogDebug($"After callback invoked: {source.Count}");
            });

            var postSetUpCount = source.Count;

            Logger.LogDebug($"Invoking ExecuteSqlCommand");
            var actualResult = MockedDbContext.Database.ExecuteSqlCommand(sql);
            Logger.LogDebug($"ExecuteSqlCommand invoked");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult, Is.EqualTo(expectedResult));
                Assert.That(preSetUpCount, Is.EqualTo(itemsToCreate));
                Assert.That(postSetUpCount, Is.EqualTo(preSetUpCount));
                Assert.That(source.Count, Is.EqualTo(1));
                Assert.That(source.First(), Is.EqualTo(preSetUpFirst));
            });
        }
    }
}