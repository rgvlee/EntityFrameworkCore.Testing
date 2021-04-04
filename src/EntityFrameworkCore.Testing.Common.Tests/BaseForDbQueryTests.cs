﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public abstract class BaseForDbQueryTests<TEntity> : BaseForMockedQueryableTests<TEntity> where TEntity : BaseTestEntity
    {
        protected DbQuery<TEntity> DbQuery => (DbQuery<TEntity>) Queryable;

        protected override void SeedQueryableSource()
        {
            var itemsToAdd = Fixture.Build<TEntity>().With(p => p.CreatedAt, DateTime.Parse("2019-01-01")).CreateMany().ToList();
            AddRangeToReadOnlySource(DbQuery, itemsToAdd);
            //MockedDbContext.SaveChanges();
            ItemsAddedToQueryableSource = itemsToAdd;
        }

        protected abstract void AddToReadOnlySource(DbQuery<TEntity> mockedDbQuery, TEntity item);

        protected abstract void AddRangeToReadOnlySource(DbQuery<TEntity> mockedDbQuery, IEnumerable<TEntity> items);

        protected abstract void ClearReadOnlySource(DbQuery<TEntity> mockedDbQuery);

        [Test]
        public virtual void AddRangeToReadOnlySource_Items_AddsItemsToReadOnlySource()
        {
            var expectedResult = Fixture.CreateMany<TEntity>().ToList();

            AddRangeToReadOnlySource(DbQuery, expectedResult);

            Assert.That(DbQuery, Is.EquivalentTo(expectedResult));
        }

        [Test]
        public virtual void AddRangeToReadOnlySourceThenAddRangeToReadOnlySource_Items_AddsAllItemsToReadOnlySource()
        {
            var expectedResult = Fixture.CreateMany<TEntity>(4).ToList();

            AddRangeToReadOnlySource(DbQuery, expectedResult.Take(2));
            AddRangeToReadOnlySource(DbQuery, expectedResult.Skip(2));

            Assert.That(DbQuery, Is.EquivalentTo(expectedResult));
        }

        [Test]
        public virtual void AddToReadOnlySource_Item_AddsItemToReadOnlySource()
        {
            var expectedResult = Fixture.Create<TEntity>();

            AddToReadOnlySource(DbQuery, expectedResult);
            var numberOfItemsAdded = DbQuery.ToList().Count;

            Assert.That(numberOfItemsAdded, Is.EqualTo(1));
        }

        [Test]
        public virtual void AddToReadOnlySourceThenAddToReadOnlySource_Items_AddsBothItemsToReadOnlySource()
        {
            var expectedResult = Fixture.CreateMany<TEntity>(2).ToList();

            AddToReadOnlySource(DbQuery, expectedResult.First());
            AddToReadOnlySource(DbQuery, expectedResult.Last());

            Assert.That(DbQuery, Is.EquivalentTo(expectedResult));
        }

        [Test]
        public virtual void AnyThenAddToReadOnlySourceThenAny_ReturnsFalseThenTrue()
        {
            var actualResult1 = DbQuery.Any();
            AddToReadOnlySource(DbQuery, Fixture.Create<TEntity>());
            var actualResult2 = DbQuery.Any();

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
            AddToReadOnlySource(DbQuery, expectedResult);

            var asyncEnumerable = await DbQuery.AsAsyncEnumerable().ToList();

            var actualResults = new List<TEntity>();
            foreach (var item in asyncEnumerable)
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
            AddToReadOnlySource(DbQuery, expectedResult);

            var queryable = DbQuery.AsQueryable();

            Assert.Multiple(() =>
            {
                Assert.That(queryable.Single(), Is.EqualTo(expectedResult));
                Assert.That(queryable.Single(), Is.EqualTo(expectedResult));
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
            var expectedResult = Fixture.CreateMany<TEntity>().ToList();
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
            var expectedResult1 = Fixture.CreateMany<TEntity>().ToList();

            var sql2 = "sp_WithParams";
            var parameters2 = new List<SqlParameter> { new SqlParameter("@SomeParameter1", "Value1"), new SqlParameter("@SomeParameter2", "Value2") };
            var expectedResult2 = Fixture.CreateMany<TEntity>().ToList();

            AddFromSqlResult(DbQuery, sql1, expectedResult1);

            //Change the source, this will force the query provider mock to aggregate
            AddRangeToReadOnlySource(DbQuery, Fixture.CreateMany<TEntity>().ToList());

            AddFromSqlResult(DbQuery, sql2, parameters2, expectedResult2);

            Console.WriteLine("actualResult1");
            var actualResult1 = DbQuery.FromSql("[dbo].[sp_NoParams]").ToList();

            Console.WriteLine("actualResult2");
            var actualResult2 = DbQuery.FromSql("[dbo].[sp_WithParams]", parameters2.ToArray()).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult1, Is.EquivalentTo(expectedResult1));
                Assert.That(actualResult2, Is.EquivalentTo(expectedResult2));
            });
        }
    }
}