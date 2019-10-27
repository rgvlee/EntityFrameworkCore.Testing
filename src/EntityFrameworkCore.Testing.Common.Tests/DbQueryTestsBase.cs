using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        [Test]
        public override void FromSql_QueryProviderWithManyFromSqlResults_ReturnsExpectedResults()
        {
            var sql1 = "sp_NoParams";
            var expectedResult1 = new Fixture().CreateMany<TQuery>().ToList();

            var sql2 = "sp_WithParams";
            var parameters2 = new List<SqlParameter> {new SqlParameter("@SomeParameter1", "Value1"), new SqlParameter("@SomeParameter2", "Value2")};
            var expectedResult2 = new Fixture().CreateMany<TQuery>().ToList();

            AddFromSqlResult(Queryable, sql1, expectedResult1);

            //Change the source, this will force the query provider mock to aggregate
            AddRange(DbQuery, new Fixture().CreateMany<TQuery>().ToList());

            AddFromSqlResult(Queryable, sql2, parameters2, expectedResult2);

            Logger.LogDebug("actualResult1");
            var actualResult1 = Queryable.FromSql("[dbo].[sp_NoParams]").ToList();
            Logger.LogDebug("actualResult2");
            var actualResult2 = Queryable.FromSql("sp_NoParams").ToList();

            Logger.LogDebug("actualResult3");
            var actualResult3 = Queryable.FromSql("[dbo].[sp_WithParams]").ToList();
            Logger.LogDebug("actualResult4");
            var actualResult4 = Queryable.FromSql("sp_WithParams @SomeParameter1 @SomeParameter2").ToList();

            Logger.LogDebug("actualResult5");
            var actualResult5 = Queryable.FromSql("[dbo].[sp_WithParams]", new List<SqlParameter> {new SqlParameter("@someparameter2", "value2")}).ToList();
            Logger.LogDebug("actualResult6");
            var actualResult6 = Queryable.FromSql("sp_WithParams @SomeParameter1 @SomeParameter2", new List<SqlParameter> {new SqlParameter("@someparameter2", "value2")}).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EquivalentTo(expectedResult1));
                Assert.That(actualResult2, Is.EquivalentTo(actualResult1));

                Assert.That(actualResult3, Is.EquivalentTo(expectedResult2));
                Assert.That(actualResult4, Is.EquivalentTo(actualResult3));

                Assert.That(actualResult5, Is.EquivalentTo(expectedResult2));
                Assert.That(actualResult6, Is.EquivalentTo(actualResult5));
            });
        }
    }
}