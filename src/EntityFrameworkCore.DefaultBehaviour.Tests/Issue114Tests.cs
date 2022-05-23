using System;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.DefaultBehaviour.Tests
{
    public class Issue114Tests : Testing.Common.Tests.Issue114Tests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        protected override TestDbContext MockedDbContextFactory()
        {
            return new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        }
    }
}