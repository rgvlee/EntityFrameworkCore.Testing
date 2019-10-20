using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    [TestFixture]
    public abstract class DbQueryTestsBase<TDbContext, TQuery> : MockQueryableTestsBase<TQuery>
        where TDbContext : DbContext
        where TQuery : TestEntityBase
    {
        [SetUp]
        public override void SetUp()
        {
            MockedDbContext = CreateMockedDbContext();
            base.SetUp();
        }

        protected override void SeedQueryableSource()
        {
            var itemsToAdd = Fixture.CreateMany<TQuery>().ToList();
            AddRange(DbQuery, itemsToAdd);
            //MockedDbContext.SaveChanges();
            ItemsAddedToQueryableSource = itemsToAdd;
        }

        protected TDbContext MockedDbContext;
        protected DbQuery<TQuery> DbQuery => (DbQuery<TQuery>) Queryable;

        protected abstract TDbContext CreateMockedDbContext();

        protected abstract void Add(DbQuery<TQuery> mockedDbQuery, TQuery item);
        protected abstract void AddRange(DbQuery<TQuery> mockedDbQuery, IEnumerable<TQuery> sequence);
        protected abstract void Clear(DbQuery<TQuery> mockedDbQuery);

        [Test]
        public virtual void Add_Item_AddsItem()
        {
            var expectedResult = new Fixture().Create<TQuery>();

            Add(DbQuery, expectedResult);
            var numberOfItemsAdded = DbQuery.ToList().Count;

            Assert.That(numberOfItemsAdded, Is.EqualTo(1));
        }

        [Test]
        public virtual void AddRange_Items_AddsItems()
        {
            var expectedResult = new Fixture().CreateMany<TQuery>().ToList();

            AddRange(DbQuery, expectedResult);

            Assert.That(DbQuery, Is.EquivalentTo(expectedResult));
        }

        [Test]
        public virtual void AddRangeThenAddRange_Items_AddsAllItems()
        {
            var expectedResult = new Fixture().CreateMany<TQuery>(4).ToList();

            AddRange(DbQuery, expectedResult.Take(2));
            AddRange(DbQuery, expectedResult.Skip(2));

            Assert.That(DbQuery, Is.EquivalentTo(expectedResult));
        }

        [Test]
        public virtual void AddRangeThenClear_RemovesAllItems()
        {
            var expectedResult = new Fixture().CreateMany<TQuery>().ToList();
            AddRange(DbQuery, expectedResult);
            var numberOfItemsAdded = DbQuery.ToList().Count;

            Clear(DbQuery);

            Assert.Multiple(() =>
            {
                Assert.That(numberOfItemsAdded, Is.EqualTo(expectedResult.Count));
                Assert.That(DbQuery.Any(), Is.False);
            });
        }

        [Test]
        public virtual void AddThenAdd_Items_AddsBothItems()
        {
            var expectedResult = new Fixture().CreateMany<TQuery>(2).ToList();

            Add(DbQuery, expectedResult.First());
            Add(DbQuery, expectedResult.Last());

            Assert.That(DbQuery, Is.EquivalentTo(expectedResult));
        }

        [Test]
        public virtual void AnyThenAddAndPersistThenAny_ReturnsFalseThenTrue()
        {
            var actualResult1 = DbQuery.Any();
            Add(DbQuery, Fixture.Create<TQuery>());
            var actualResult2 = DbQuery.Any();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.False);
                Assert.That(actualResult2, Is.True);
            });
        }

        [Test]
        public virtual void Clear_WithNoItemsAdded_DoesNothing()
        {
            var preActNumberOfItems = DbQuery.ToList().Count;

            Clear(DbQuery);

            var postActNumberOfItems = DbQuery.ToList().Count;
            Assert.Multiple(() =>
            {
                Assert.That(preActNumberOfItems, Is.EqualTo(0));
                Assert.That(postActNumberOfItems, Is.EqualTo(preActNumberOfItems));
            });
        }
    }
}