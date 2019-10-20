using System;
using System.Linq;
using AutoFixture;
using EntityFrameworkCore.Testing.Common.Tests;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.DefaultBehaviour.Tests
{
    [TestFixture]
    public class ByTypeDbSetTests : QueryableTestsBase<TestEntity1>
    {
        [SetUp]
        public override void SetUp()
        {
            DbContext = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            base.SetUp();
        }

        protected TestDbContext DbContext;
        protected override IQueryable<TestEntity1> Queryable => DbContext.Set<TestEntity1>();

        protected override void SeedQueryableSource()
        {
            var itemsToAdd = Fixture.CreateMany<TestEntity1>().ToList();
            DbContext.Set<TestEntity1>().AddRange(itemsToAdd);
            DbContext.SaveChanges();
            ItemsAddedToQueryableSource = itemsToAdd;
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