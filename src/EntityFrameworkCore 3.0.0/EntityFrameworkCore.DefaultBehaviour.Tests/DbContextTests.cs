using System;
using EntityFrameworkCore.Testing.Common.Tests;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.DefaultBehaviour.Tests
{
    [TestFixture]
    public class DbContextTests : BaseForTests
    {
        [SetUp]
        public override void SetUp()
        {
            DbContext = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            base.SetUp();
        }

        protected TestDbContext DbContext;

        [Test]
        public virtual void ExecuteSqlCommand_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult = DbContext.Database.ExecuteSqlCommand("sp_NoParams");
            });
        }

        [Test]
        public virtual void ExecuteSqlInterpolated_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult = DbContext.Database.ExecuteSqlInterpolated($"sp_NoParams");
            });
        }

        [Test]
        public virtual void ExecuteSqlRaw_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var actualResult = DbContext.Database.ExecuteSqlRaw("sp_NoParams");
            });
        }
    }
}