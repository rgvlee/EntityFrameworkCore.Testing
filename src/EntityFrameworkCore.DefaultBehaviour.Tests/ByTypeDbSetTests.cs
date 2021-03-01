using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using EntityFrameworkCore.Testing.Common.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using NUnit.Framework;

namespace EntityFrameworkCore.DefaultBehaviour.Tests
{
    public class ByTypeDbSetTests : BaseForQueryableTests<TestEntity>
    {
        protected TestDbContext DbContext;

        protected DbSet<TestEntity> DbSet => DbContext.Set<TestEntity>();

        protected override IQueryable<TestEntity> Queryable => DbSet;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            DbContext = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        }

        protected override void SeedQueryableSource()
        {
            var itemsToAdd = Fixture.Build<TestEntity>().With(p => p.CreatedAt, DateTime.Parse("2019-01-01")).CreateMany().ToList();
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

            var asyncEnumerable = await DbSet.AsAsyncEnumerable().ToList();

            var actualResults = new List<TestEntity>();
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
        public virtual void FromSql_ThrowsException()
        {
            Assert.Throws<NotSupportedException>(() =>
            {
                var actualResult = DbSet.FromSql("sp_NoParams").ToList();
            });
        }
    }
}