using System;
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
    public abstract class DbQueryTestsBase<TQuery> : MockQueryableTestsBase<TQuery>
        where TQuery : TestEntityBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        protected override void SeedQueryableSource()
        {
            var itemsToAdd = Fixture.Build<TQuery>()
                .With(p => p.FixedDateTime, DateTime.Parse("2019-01-01"))
                .CreateMany().ToList();
            AddRangeToReadOnlySource(DbQuery, itemsToAdd);
            //MockedDbContext.SaveChanges();
            ItemsAddedToQueryableSource = itemsToAdd;
        }

        protected DbQuery<TQuery> DbQuery => (DbQuery<TQuery>) Queryable;

        protected abstract void AddToReadOnlySource(DbQuery<TQuery> mockedDbQuery, TQuery item);
        protected abstract void AddRangeToReadOnlySource(DbQuery<TQuery> mockedDbQuery, IEnumerable<TQuery> items);
        protected abstract void ClearReadOnlySource(DbQuery<TQuery> mockedDbQuery);

        [Test]
        public virtual void AddRangeToReadOnlySource_Items_AddsItemsToReadOnlySource()
        {
            var expectedResult = Fixture.CreateMany<TQuery>().ToList();

            AddRangeToReadOnlySource(DbQuery, expectedResult);

            Assert.That(DbQuery, Is.EquivalentTo(expectedResult));
        }

        [Test]
        public virtual void AddRangeToReadOnlySourceThenAddRangeToReadOnlySource_Items_AddsAllItemsToReadOnlySource()
        {
            var expectedResult = Fixture.CreateMany<TQuery>(4).ToList();

            AddRangeToReadOnlySource(DbQuery, expectedResult.Take(2));
            AddRangeToReadOnlySource(DbQuery, expectedResult.Skip(2));

            Assert.That(DbQuery, Is.EquivalentTo(expectedResult));
        }

        [Test]
        public virtual void AddToReadOnlySource_Item_AddsItemToReadOnlySource()
        {
            var expectedResult = Fixture.Create<TQuery>();

            AddToReadOnlySource(DbQuery, expectedResult);
            var numberOfItemsAdded = DbQuery.ToList().Count;

            Assert.That(numberOfItemsAdded, Is.EqualTo(1));
        }

        [Test]
        public virtual void AddToReadOnlySourceThenAddToReadOnlySource_Items_AddsBothItemsToReadOnlySource()
        {
            var expectedResult = Fixture.CreateMany<TQuery>(2).ToList();

            AddToReadOnlySource(DbQuery, expectedResult.First());
            AddToReadOnlySource(DbQuery, expectedResult.Last());

            Assert.That(DbQuery, Is.EquivalentTo(expectedResult));
        }

        [Test]
        public virtual void AnyThenAddToReadOnlySourceThenAny_ReturnsFalseThenTrue()
        {
            var actualResult1 = DbQuery.Any();
            AddToReadOnlySource(DbQuery, Fixture.Create<TQuery>());
            var actualResult2 = DbQuery.Any();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.False);
                Assert.That(actualResult2, Is.True);
            });
        }

        [Test]
        public virtual void ClearReadOnlySource_WithNoItemsAddedToReadOnlySource_DoesNothing()
        {
            var preActNumberOfItems = DbQuery.ToList().Count;

            ClearReadOnlySource(DbQuery);

            var postActNumberOfItems = DbQuery.ToList().Count;
            Assert.Multiple(() =>
            {
                Assert.That(preActNumberOfItems, Is.EqualTo(0));
                Assert.That(postActNumberOfItems, Is.EqualTo(preActNumberOfItems));
            });
        }

        [Test]
        public virtual void ClearReadOnlySourceWithExistingItems_RemovesAllItemsFromReadOnlySource()
        {
            var expectedResult = Fixture.CreateMany<TQuery>().ToList();
            AddRangeToReadOnlySource(DbQuery, expectedResult);
            var numberOfItemsAdded = DbQuery.ToList().Count;

            ClearReadOnlySource(DbQuery);

            Assert.Multiple(() =>
            {
                Assert.That(numberOfItemsAdded, Is.EqualTo(expectedResult.Count));
                Assert.That(DbQuery.Any(), Is.False);
            });
        }

        [Test]
        public override void FromSql_QueryProviderWithManyFromSqlResults_ReturnsExpectedResults()
        {
            var sql1 = "sp_NoParams";
            var expectedResult1 = Fixture.CreateMany<TQuery>().ToList();

            var sql2 = "sp_WithParams";
            var parameters2 = new List<SqlParameter> {new SqlParameter("@SomeParameter1", "Value1"), new SqlParameter("@SomeParameter2", "Value2")};
            var expectedResult2 = Fixture.CreateMany<TQuery>().ToList();

            AddFromSqlResult(DbQuery, sql1, expectedResult1);

            //Change the source, this will force the query provider mock to aggregate
            AddRangeToReadOnlySource(DbQuery, Fixture.CreateMany<TQuery>().ToList());

            AddFromSqlResult(DbQuery, sql2, parameters2, expectedResult2);

            Logger.LogDebug("actualResult1");
            var actualResult1 = DbQuery.FromSql("[dbo].[sp_NoParams]").ToList();
            Logger.LogDebug("actualResult2");
            var actualResult2 = DbQuery.FromSql("sp_NoParams").ToList();

            Logger.LogDebug("actualResult3");
            var actualResult3 = DbQuery.FromSql("[dbo].[sp_WithParams]").ToList();
            Logger.LogDebug("actualResult4");
            var actualResult4 = DbQuery.FromSql("sp_WithParams @SomeParameter1 @SomeParameter2").ToList();

            Logger.LogDebug("actualResult5");
            var actualResult5 = DbQuery.FromSql("[dbo].[sp_WithParams]", new List<SqlParameter> {new SqlParameter("@someparameter2", "value2")}).ToList();
            Logger.LogDebug("actualResult6");
            var actualResult6 = DbQuery.FromSql("sp_WithParams @SomeParameter1 @SomeParameter2", new List<SqlParameter> {new SqlParameter("@someparameter2", "value2")}).ToList();

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