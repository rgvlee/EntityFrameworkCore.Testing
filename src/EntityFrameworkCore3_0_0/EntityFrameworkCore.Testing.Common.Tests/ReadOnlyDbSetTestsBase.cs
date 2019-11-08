﻿using System;
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
    public abstract class ReadOnlyDbSetTestsBase<TEntity> : MockedDbSetQueryProviderTestsBase<TEntity>
        where TEntity : TestEntityBase
    {
        protected override void SeedQueryableSource()
        {
            var itemsToAdd = Fixture.Build<TEntity>()
                .With(p => p.FixedDateTime, DateTime.Parse("2019-01-01"))
                .CreateMany().ToList();
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
            var parameters2 = new List<SqlParameter> {new SqlParameter("@SomeParameter1", "Value1"), new SqlParameter("@SomeParameter2", "Value2")};
            var expectedResult2 = Fixture.CreateMany<TEntity>().ToList();

            AddFromSqlRawResult(DbSet, sql1, expectedResult1);

            //Change the source, this will force the query provider mock to aggregate
            AddRangeToReadOnlySource(DbSet, Fixture.CreateMany<TEntity>().ToList());

            AddFromSqlRawResult(DbSet, sql2, parameters2, expectedResult2);

            Logger.LogDebug("actualResult1");
            var actualResult1 = DbSet.FromSqlRaw("[dbo].[sp_NoParams]").ToList();
            Logger.LogDebug("actualResult2");
            var actualResult2 = DbSet.FromSqlRaw("sp_NoParams").ToList();

            Logger.LogDebug("actualResult3");
            var actualResult3 = DbSet.FromSqlRaw("[dbo].[sp_WithParams]").ToList();
            Logger.LogDebug("actualResult4");
            var actualResult4 = DbSet.FromSqlRaw("sp_WithParams @SomeParameter1 @SomeParameter2").ToList();

            Logger.LogDebug("actualResult5");
            var actualResult5 = DbSet.FromSqlRaw("[dbo].[sp_WithParams]", new List<SqlParameter> {new SqlParameter("@someparameter2", "value2")}.ToArray()).ToList();
            Logger.LogDebug("actualResult6");
            var actualResult6 = DbSet.FromSqlRaw("sp_WithParams @SomeParameter1 @SomeParameter2", new List<SqlParameter> {new SqlParameter("@someparameter2", "value2")}.ToArray()).ToList();

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