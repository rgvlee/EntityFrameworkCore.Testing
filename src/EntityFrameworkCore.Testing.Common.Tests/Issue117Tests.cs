using System;
using AutoFixture;
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
        public void DbContextDispose_DbContextCreatedUsingDbContextFactoryWithinUsingBlock_DoesNotThrowException()
        {
            var dbContextFactory = new TestDbContextFactory(MockedDbContextFactory);
            Invoking(() =>
            {
                using (dbContextFactory.CreateDbContext()) { }
            }).Should().NotThrow();
        }

        [Test]
        public void DbContextDispose_DbContextCreatedUsingDbContextFactory_DoesNotThrowException()
        {
            Invoking(() => MockedDbContextFactory().Dispose()).Should().NotThrow();
        }

        [Test]
        public void DbContextAddRange_DoesNotThrowException()
        {
            Invoking(() => MockedDbContextFactory().AddRange(Fixture.CreateMany<Foo>())).Should().NotThrow();
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