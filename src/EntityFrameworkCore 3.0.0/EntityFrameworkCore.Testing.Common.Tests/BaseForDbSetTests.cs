using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    [TestFixture]
    public abstract class BaseForDbSetTests<TDbContext, TEntity> : BaseForMockedDbSetQueryProviderTests<TEntity> where TDbContext : DbContext
        where TEntity : BaseTestEntity
    {
        [SetUp]
        public override void SetUp()
        {
            MockedDbContext = CreateMockedDbContext();
            base.SetUp();
        }

        protected override void SeedQueryableSource()
        {
            var itemsToAdd = Fixture.Build<TEntity>().With(p => p.FixedDateTime, DateTime.Parse("2019-01-01")).CreateMany().ToList();
            DbSet.AddRange(itemsToAdd);
            MockedDbContext.SaveChanges();
            ItemsAddedToQueryableSource = itemsToAdd;
        }

        protected TDbContext MockedDbContext;

        protected abstract TDbContext CreateMockedDbContext();

        [Test]
        public virtual void AddAndPersist_Item_AddsAndPersistsItem()
        {
            var expectedResult = Fixture.Create<TEntity>();

            DbSet.Add(expectedResult);
            MockedDbContext.SaveChanges();

            Assert.Multiple(() =>
            {
                Assert.That(DbSet.Single(), Is.EqualTo(expectedResult));
                Assert.That(DbSet.Single(), Is.EqualTo(expectedResult));
            });
        }

        [Test]
        public virtual void AddAndPersist_Items_AddsAndPersistsItems()
        {
            var expectedResult = Fixture.CreateMany<TEntity>().ToList();

            DbSet.AddRange(expectedResult);
            MockedDbContext.SaveChanges();

            var actualResult = DbSet.ToList();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult, Is.EquivalentTo(expectedResult));
                Assert.That(DbSet.ToList(), Is.EquivalentTo(actualResult));
            });
        }

        [Test]
        public virtual async Task AddAndPersistAsync_Item_AddsAndPersistsItem()
        {
            var expectedResult = Fixture.Create<TEntity>();

            await DbSet.AddAsync(expectedResult);
            await MockedDbContext.SaveChangesAsync();

            Assert.Multiple(() =>
            {
                Assert.That(DbSet.Single(), Is.EqualTo(expectedResult));
                Assert.That(DbSet.Single(), Is.EqualTo(expectedResult));
            });
        }

        [Test]
        public virtual async Task AddAndPersistAsync_Items_AddsAndPersistsItems()
        {
            var expectedResult = Fixture.CreateMany<TEntity>().ToList();

            await DbSet.AddRangeAsync(expectedResult);
            await MockedDbContext.SaveChangesAsync();

            var actualResult = DbSet.ToList();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult, Is.EquivalentTo(expectedResult));
                Assert.That(DbSet.ToList(), Is.EquivalentTo(actualResult));
            });
        }

        [Test]
        public virtual void AddThenSingleThenAddRangeThenToListThenWhereThenSelect_ReturnsExpectedResults()
        {
            var items = Fixture.CreateMany<TEntity>().ToList();
            DbSet.Add(items[0]);
            MockedDbContext.SaveChanges();

            var singleResult = DbSet.Single();

            DbSet.AddRange(items.Skip(1));
            MockedDbContext.SaveChanges();

            var toListResult = DbSet.ToList();

            var selectedItem = items.Last();
            var whereResult = DbSet.Where(x => x.Equals(selectedItem)).ToList();

            var selectResult = DbSet.Select(x => new { Item = x }).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(singleResult, Is.EqualTo(items[0]));
                Assert.That(toListResult, Is.EquivalentTo(items));
                Assert.That(whereResult, Is.EquivalentTo(new List<TEntity> { selectedItem }));
                for (var i = 0; i < items.Count; i++)
                {
                    Assert.That(selectResult[i].Item, Is.EqualTo(items[i]));
                }
            });
        }

        [Test]
        public virtual void AnyThenAddThenPersistThenAny_ReturnsFalseThenTrue()
        {
            var actualResult1 = DbSet.Any();
            DbSet.Add(Fixture.Create<TEntity>());
            MockedDbContext.SaveChanges();
            var actualResult2 = DbSet.Any();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.False);
                Assert.That(actualResult2, Is.True);
            });
        }

        [Test]
        public virtual async Task AsAsyncEnumerable_ReturnsAsyncEnumerable()
        {
            var expectedResult = Fixture.Create<TEntity>();
            DbSet.Add(expectedResult);
            MockedDbContext.SaveChanges();

            var asyncEnumerable = DbSet.AsAsyncEnumerable();

            var actualResults = new List<TEntity>();
            await foreach (var item in asyncEnumerable)
            {
                actualResults.Add(item);
            }

            Assert.Multiple(() =>
            {
                Assert.That(actualResults.Single(), Is.EqualTo(expectedResult));
                Assert.That(actualResults.Single(), Is.EqualTo(expectedResult));
            });
        }

        [Test]
        public virtual void AsQueryable_ReturnsQueryable()
        {
            var expectedResult = Fixture.Create<TEntity>();
            DbSet.Add(expectedResult);
            MockedDbContext.SaveChanges();

            var queryable = DbSet.AsQueryable();

            Assert.Multiple(() =>
            {
                Assert.That(queryable.Single(), Is.EqualTo(expectedResult));
                Assert.That(queryable.Single(), Is.EqualTo(expectedResult));
            });
        }
    }
}