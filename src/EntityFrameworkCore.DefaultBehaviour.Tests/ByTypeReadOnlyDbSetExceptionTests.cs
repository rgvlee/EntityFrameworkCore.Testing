using System;
using EntityFrameworkCore.Testing.Common.Tests;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.DefaultBehaviour.Tests
{
    [TestFixture]
    public class ReadOnlyDbSetExceptionTests : ReadOnlyDbSetExceptionTests<TestReadOnlyEntity>
    {
        [SetUp]
        public override void SetUp()
        {
            DbContext = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            base.SetUp();
        }

        protected TestDbContext DbContext;
        protected override DbSet<TestReadOnlyEntity> DbSet => DbContext.Set<TestReadOnlyEntity>();
    }
}