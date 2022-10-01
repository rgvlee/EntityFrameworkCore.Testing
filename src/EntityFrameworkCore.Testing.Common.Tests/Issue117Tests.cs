using System.Linq;
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
        public void DbContextDispose_InvokedViaUsingBlock_DoesNotThrowException()
        {
            Invoking(() =>
            {
                using (MockedDbContextFactory()) { }
            }).Should().NotThrow();
        }

        [Test]
        public void DbContextDispose_DoesNotThrowException()
        {
            Invoking(() => MockedDbContextFactory().Dispose()).Should().NotThrow();
        }

        [Test]
        public void DbContextAddRange_DoesNotThrowException()
        {
            Invoking(() => MockedDbContextFactory().AddRange(Fixture.CreateMany<Foo>())).Should().NotThrow();
        }

        [Test]
        public void DbContextAddRangeThenSaveChanges_WithinUsingBlock_PersistsMutableEntities()
        {
            using (var dbContext = MockedDbContextFactory())
            {
                var entities = Fixture.CreateMany<Foo>();

                dbContext.AddRange(entities);
                dbContext.SaveChanges();

                dbContext.Set<Foo>().ToList().Should().BeEquivalentTo(entities);
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