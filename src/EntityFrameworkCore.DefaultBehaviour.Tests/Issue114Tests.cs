using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.DefaultBehaviour.Tests
{
    public class Issue114Tests : Testing.Common.Tests.Issue114Tests
    {
        protected override TestDbContext MockedDbContextFactory()
        {
            return new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        }
    }
}