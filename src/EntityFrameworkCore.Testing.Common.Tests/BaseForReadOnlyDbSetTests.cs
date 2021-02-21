using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public abstract class BaseForReadOnlyDbSetTests<TEntity> : BaseForMockedDbSetQueryProviderTests<TEntity> where TEntity : BaseTestEntity
    {
        protected override void SeedQueryableSource()
        {
            var itemsToAdd = Fixture.Build<TEntity>().With(p => p.FixedDateTime, DateTime.Parse("2019-01-01")).CreateMany().ToList();
            AddRangeToReadOnlySource(DbSet, itemsToAdd);
            //MockedDbContext.SaveChanges();
            ItemsAddedToQueryableSource = itemsToAdd;
        }

        protected abstract void AddToReadOnlySource(DbSet<TEntity> mockedDbQuery, TEntity item);

        protected abstract void AddRangeToReadOnlySource(DbSet<TEntity> mockedDbQuery, IEnumerable<TEntity> items);

        protected abstract void ClearReadOnlySource(DbSet<TEntity> mockedDbQuery);

        [Test]
        public virtual void AddRangeToReadOnlySource_Items_AddsItemsToReadOnlySource()
        {
            var expectedResult = Fixture.CreateMany<TEntity>().ToList();

            AddRangeToReadOnlySource(DbSet, expectedResult);

            Assert.That(DbSet, Is.EquivalentTo(expectedResult));
        }

        [Test]
        public virtual void AddRangeToReadOnlySourceThenAddRangeToReadOnlySource_Items_AddsAllItemsToReadOnlySource()
        {
            var expectedResult = Fixture.CreateMany<TEntity>(4).ToList();

            AddRangeToReadOnlySource(DbSet, expectedResult.Take(2));
            AddRangeToReadOnlySource(DbSet, expectedResult.Skip(2));

            Assert.That(DbSet, Is.EquivalentTo(expectedResult));
        }

        [Test]
        public virtual void AddToReadOnlySource_Item_AddsItemToReadOnlySource()
        {
            var expectedResult = Fixture.Create<TEntity>();

            AddToReadOnlySource(DbSet, expectedResult);
            var numberOfItemsAdded = DbSet.ToList().Count;

            Assert.That(numberOfItemsAdded, Is.EqualTo(1));
        }

        [Test]
        public virtual void AddToReadOnlySourceThenAddToReadOnlySource_Items_AddsBothItemsToReadOnlySource()
        {
            var expectedResult = Fixture.CreateMany<TEntity>(2).ToList();

            AddToReadOnlySource(DbSet, expectedResult.First());
            AddToReadOnlySource(DbSet, expectedResult.Last());

            Assert.That(DbSet, Is.EquivalentTo(expectedResult));
        }

        [Test]
        public virtual void AnyThenAddToReadOnlySourceThenAny_ReturnsFalseThenTrue()
        {
            var actualResult1 = DbSet.Any();
            AddToReadOnlySource(DbSet, Fixture.Create<TEntity>());
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
            AddToReadOnlySource(DbSet, expectedResult);

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
            AddToReadOnlySource(DbSet, expectedResult);

            var queryable = DbSet.AsQueryable();

            Assert.Multiple(() =>
            {
                Assert.That(queryable.Single(), Is.EqualTo(expectedResult));
                Assert.That(queryable.Single(), Is.EqualTo(expectedResult));
            });
        }

        [Test]
        public virtual void ClearReadOnlySource_WithNoItemsAddedToReadOnlySource_DoesNothing()
        {
            var preActNumberOfItems = DbSet.ToList().Count;

            ClearReadOnlySource(DbSet);

            var postActNumberOfItems = DbSet.ToList().Count;
            Assert.Multiple(() =>
            {
                Assert.That(preActNumberOfItems, Is.EqualTo(0));
                Assert.That(postActNumberOfItems, Is.EqualTo(preActNumberOfItems));
            });
        }

        [Test]
        public virtual void ClearReadOnlySourceWithExistingItems_RemovesAllItemsFromReadOnlySource()
        {
            var expectedResult = Fixture.CreateMany<TEntity>().ToList();
            AddRangeToReadOnlySource(DbSet, expectedResult);
            var numberOfItemsAdded = DbSet.ToList().Count;

            ClearReadOnlySource(DbSet);

            Assert.Multiple(() =>
            {
                Assert.That(numberOfItemsAdded, Is.EqualTo(expectedResult.Count));
                Assert.That(DbSet.Any(), Is.False);
            });
        }

        [Test]
        public override void FromSqlRaw_QueryProviderWithManyFromSqlResults_ReturnsExpectedResults()
        {
            var sql1 = "sp_NoParams";
            var expectedResult1 = Fixture.CreateMany<TEntity>().ToList();

            var sql2 = "sp_WithParams";
            var parameters2 = new List<SqlParameter> { new SqlParameter("@SomeParameter1", "Value1"), new SqlParameter("@SomeParameter2", "Value2") };
            var expectedResult2 = Fixture.CreateMany<TEntity>().ToList();

            AddFromSqlRawResult(DbSet, sql1, expectedResult1);

            //Change the source, this will force the query provider mock to aggregate
            AddRangeToReadOnlySource(DbSet, Fixture.CreateMany<TEntity>().ToList());

            AddFromSqlRawResult(DbSet, sql2, parameters2, expectedResult2);

            Console.WriteLine("actualResult1");
            var actualResult1 = DbSet.FromSqlRaw("[dbo].[sp_NoParams]").ToList();

            Console.WriteLine("actualResult2");
            var actualResult2 = DbSet.FromSqlRaw("[dbo].[sp_WithParams]", parameters2.ToArray()).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EquivalentTo(expectedResult1));
                Assert.That(actualResult2, Is.EquivalentTo(expectedResult2));
            });
        }
    }
}