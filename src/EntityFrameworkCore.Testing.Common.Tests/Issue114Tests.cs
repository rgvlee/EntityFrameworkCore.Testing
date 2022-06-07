using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public abstract class Issue114Tests : BaseForTests
    {
        protected abstract TestDbContext MockedDbContextFactory();

        [Test]
        public void ReadOnlyEntityToList_ReadOnlyEntityHasNoDbContextProperty_EmptyList()
        {
            var mockedContext = MockedDbContextFactory();

            mockedContext.Set<Foo>().ToList().Should().BeEmpty();
            mockedContext.Query<Foo>().ToList().Should().BeEmpty();
        }

        public class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Foo>().HasNoKey();
            }
        }

        public class Foo
        {
            public string Bar { get; set; }
        }
    }
}