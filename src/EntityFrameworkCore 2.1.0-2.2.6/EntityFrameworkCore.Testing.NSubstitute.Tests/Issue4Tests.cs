using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using EntityFrameworkCore.Testing.Common.Helpers;
using EntityFrameworkCore.Testing.Common.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests
{
    public class Issue4Tests
    {
        [SetUp]
        public virtual void SetUp()
        {
            LoggerHelper.LoggerFactory.AddConsole(LogLevel.Debug);
        }

        [Test]
        public void AsQueryable_Set_ReturnsIQueryableOfTWithMockedQueryProvider()
        {
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

            var mockedSetAsQueryable = mockedDbContext.TestEntities.AsQueryable();

            var asyncProvider = mockedSetAsQueryable.Provider as IAsyncQueryProvider;
            Assert.That(asyncProvider, Is.Not.Null);
        }

        [Test]
        public async Task AsQueryableThenWhereThenSingleOrDefaultAsync_WhereOperationReturnsTrue_ReturnsSingleEntity()
        {
            var fixture = new Fixture();
            var entities = fixture.CreateMany<TestEntity>().ToList();
            var entityToFind = entities.ElementAt(1);
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            var mockedSet = mockedDbContext.TestEntities;
            mockedSet.AddRange(entities);
            mockedDbContext.SaveChanges();

            var result = await mockedSet.AsQueryable().Where(x => x.Guid.Equals(entityToFind.Guid)).SingleOrDefaultAsync();

            Assert.That(result, Is.EqualTo(entityToFind));
        }

        [Test]
        public async Task AsQueryableThenWhereThenSingleOrDefaultAsync_WhereOperationReturnsFalse_ReturnsDefault()
        {
            var fixture = new Fixture();
            var entities = fixture.CreateMany<TestEntity>().ToList();
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            var mockedSet = mockedDbContext.TestEntities;
            mockedSet.AddRange(entities);
            mockedDbContext.SaveChanges();

            var result = await mockedSet.AsQueryable().Where(x => x.Guid.Equals(Guid.NewGuid())).SingleOrDefaultAsync();

            Assert.That(result, Is.Null);
        }
    }
}