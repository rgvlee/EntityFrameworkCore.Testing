using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using EntityFrameworkCore.Testing.Common.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Moq.Tests
{
    public class Issue4Tests : BaseForTests
    {
        [Test]
        public void AsQueryable_Set_ReturnsIQueryableOfTWithMockedQueryProvider()
        {
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();

            var mockedSetAsQueryable = mockedDbContext.TestEntities.AsQueryable();

            var asyncProvider = mockedSetAsQueryable.Provider as IAsyncQueryProvider;
            Assert.That(asyncProvider, Is.Not.Null);
        }

        [Test]
        public async Task AsQueryableThenWhereThenSingleOrDefaultAsync_WhereOperationReturnsFalse_ReturnsDefault()
        {
            var entities = Fixture.CreateMany<TestEntity>().ToList();
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            var mockedSet = mockedDbContext.TestEntities;
            mockedSet.AddRange(entities);
            mockedDbContext.SaveChanges();

            var result = await mockedSet.AsQueryable().Where(x => x.Id.Equals(Guid.NewGuid())).SingleOrDefaultAsync();

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task AsQueryableThenWhereThenSingleOrDefaultAsync_WhereOperationReturnsTrue_ReturnsSingleEntity()
        {
            var entities = Fixture.CreateMany<TestEntity>().ToList();
            var entityToFind = entities.ElementAt(1);
            var mockedDbContext = Create.MockedDbContextFor<TestDbContext>();
            var mockedSet = mockedDbContext.TestEntities;
            mockedSet.AddRange(entities);
            mockedDbContext.SaveChanges();

            var result = await mockedSet.AsQueryable().Where(x => x.Id.Equals(entityToFind.Id)).SingleOrDefaultAsync();

            Assert.That(result, Is.EqualTo(entityToFind));
        }
    }
}