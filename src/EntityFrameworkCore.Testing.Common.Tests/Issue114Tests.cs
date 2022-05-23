using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public abstract class Issue114Tests : BaseForTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        protected abstract TestDbContext MockedDbContextFactory();


        [Test]
        public void ToList_DbSetWithNoKey_EmptyList()
        {
            var mockedContext = MockedDbContextFactory();

            mockedContext.Set<Foo>().ToList().Should().BeEquivalentTo(new List<Foo>());
        }

        public class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Foo>().HasNoKey();
            }
        }

        private class Foo
        {
            public string Bar { get; set; }
        }
    }
}