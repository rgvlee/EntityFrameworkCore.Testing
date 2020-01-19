using System;
using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkCore.Testing.Common.Tests;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.DefaultBehaviour.Tests
{
    [TestFixture]
    public class ByTypeDbQueryTests
    {
        [SetUp]
        public void SetUp()
        {
            DbContext = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        }

        protected TestDbContext DbContext;
        protected DbQuery<TestQuery> DbQuery => DbContext.Query<TestQuery>();

        [Test]
        public virtual async Task AsAsyncEnumerable_ReturnsAsyncEnumerable()
        {
            var asyncEnumerable = DbQuery.AsAsyncEnumerable();

            Assert.That(asyncEnumerable, Is.Not.Null);
        }

        [Test]
        public virtual void AsQueryable_ReturnsQueryable()
        {
            var queryable = DbQuery.AsQueryable();

            Assert.That(queryable, Is.Not.Null);
        }
    }
}