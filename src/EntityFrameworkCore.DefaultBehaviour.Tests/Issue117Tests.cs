using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.DefaultBehaviour.Tests
{
    public class Issue117Tests : Testing.Common.Tests.Issue117Tests
    {
        protected override TestDbContext MockedDbContextFactory()
        {
            return new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        }
    }
}