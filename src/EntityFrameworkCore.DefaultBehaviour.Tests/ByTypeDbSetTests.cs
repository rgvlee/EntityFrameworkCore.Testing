using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using EntityFrameworkCore.Testing.Common.Tests;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.DefaultBehaviour.Tests
{
    public class ByTypeDbSetTests : BaseForQueryableTests<TestEntity>
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            DbContext = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        }

        protected TestDbContext DbContext;

        protected DbSet<TestEntity> DbSet => DbContext.Set<TestEntity>();

        protected override IQueryable<TestEntity> Queryable => DbSet;

        protected override void SeedQueryableSource()
        {
            var itemsToAdd = Fixture.Build<TestEntity>().With(p => p.FixedDateTime, DateTime.Parse("2019-01-01")).CreateMany().ToList();
            DbContext.Set<TestEntity>().AddRange(itemsToAdd);
            DbContext.SaveChanges();
            ItemsAddedToQueryableSource = itemsToAdd;
        }

        [Test]
        public virtual async Task AsAsyncEnumerable_ReturnsAsyncEnumerable()
        {
            var expectedResult = Fixture.Create<TestEntity>();
            DbSet.Add(expectedResult);
            DbContext.SaveChanges();

            var asyncEnumerable = DbSet.AsAsyncEnumerable();

            var actualResults = new List<TestEntity>();
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
            var expectedResult = Fixture.Create<TestEntity>();
            DbSet.Add(expectedResult);
            DbContext.SaveChanges();

            var queryable = DbSet.AsQueryable();

            Assert.Multiple(() =>
            {
                Assert.That(queryable.Single(), Is.EqualTo(expectedResult));
                Assert.That(queryable.Single(), Is.EqualTo(expectedResult));
            });
        }

        [Test]
        [Ignore("This is not supported by the in memory database provider.")]
        public override void ElementAt_ReturnsElementAtSpecifiedIndex() { }

        [Test]
        [Ignore("This is not supported by the in memory database provider.")]
        public override void ElementAtOrDefault_ReturnsElementAtSpecifiedIndex() { }

        [Test]
        [Ignore("This is not supported by the in memory database provider.")]
        public override void ElementAtOrDefault_WithNoItemsAdded_ReturnsDefault() { }

        [Test]
        public virtual void FromSqlInterpolated_ThrowsException()
        {
            Assert.Throws<NotImplementedException>(() =>
            {
                var actualResult = DbSet.FromSqlInterpolated($"sp_NoParams").ToList();
            });
        }

        [Test]
        public virtual void FromSqlRaw_ThrowsException()
        {
            Assert.Throws<NotImplementedException>(() =>
            {
                var actualResult = DbSet.FromSqlRaw("sp_NoParams").ToList();
            });
        }

        [Test]
        [Ignore("This is not supported by the in memory database provider.")]
        public override void IndexedSelectThenWhereThenAny_TrueCondition_ReturnsTrue() { }

        [Test]
        [Ignore("This is not supported by the in memory database provider.")]
        public override void Select_WithIndex_ReturnsIndexedSequence() { }

        [Test]
        [Ignore("This is not supported by the in memory database provider.")]
        public override void SkipWhile_SkipFirstItem_ReturnsSequenceThatDoesNotIncludeFirstItem() { }

        [Test]
        [Ignore("This is not supported by the in memory database provider.")]
        public override void TakeWhile_TakeFirstItem_ReturnsFirstItem() { }

        [Test]
        [Ignore("This is not supported by the in memory database provider.")]
        public override void TakeWhile_TakeFirstItemUsingIndex_ReturnsFirstItem() { }
    }
}