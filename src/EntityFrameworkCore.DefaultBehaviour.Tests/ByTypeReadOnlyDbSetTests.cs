using System;
using EntityFrameworkCore.Testing.Common.Tests;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.DefaultBehaviour.Tests
{
    public class ByTypeReadOnlyDbSetTests
    {
        protected TestDbContext DbContext;

        protected DbSet<TestReadOnlyEntity> DbSet => DbContext.Set<TestReadOnlyEntity>();

        [SetUp]
        public virtual void SetUp()
        {
            DbContext = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        }

        [Test]
        public virtual void AsAsyncEnumerable_ReturnsAsyncEnumerable()
        {
            var asyncEnumerable = DbSet.AsAsyncEnumerable();

            Assert.That(asyncEnumerable, Is.Not.Null);
        }

        [Test]
        public virtual void AsQueryable_ReturnsQueryable()
        {
            var queryable = DbSet.AsQueryable();

            Assert.That(queryable, Is.Not.Null);
        }
    }
}