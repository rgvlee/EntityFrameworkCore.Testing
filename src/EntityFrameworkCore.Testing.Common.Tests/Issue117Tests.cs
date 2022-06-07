using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static FluentAssertions.FluentActions;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public abstract class Issue117Tests : BaseForTests
    {
        protected abstract TestDbContext MockedDbContextFactory();

        [Test]
        public void DbSetToList_DbContextCreatedUsingDbContextFactoryWithinUsingBlock_EmptyList()
        {
            var dbContextFactory = new TestDbContextFactory(MockedDbContextFactory);
            List<Foo> actualResults;
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                actualResults = dbContext.Set<Foo>().ToList();
            }

            actualResults.Should().BeEmpty();
        }

        [Test]
        public void DbContextDispose_DoesNotThrowException()
        {
            Invoking(() => MockedDbContextFactory().Dispose()).Should().NotThrow();
        }

        public class TestDbContextFactory : IDbContextFactory<TestDbContext>
        {
            private readonly Func<TestDbContext> _mockedDbContextFactory;

            public TestDbContextFactory(Func<TestDbContext> mockedDbContextFactory)
            {
                _mockedDbContextFactory = mockedDbContextFactory;
            }

            public TestDbContext CreateDbContext()
            {
                return _mockedDbContextFactory();
            }
        }

        public class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

            public virtual DbSet<Foo> MutableEntities { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Foo>().HasKey(c => c.Id);
            }
        }

        public class Foo
        {
            public string Id { get; set; }

            public string Bar { get; set; }
        }
    }
}