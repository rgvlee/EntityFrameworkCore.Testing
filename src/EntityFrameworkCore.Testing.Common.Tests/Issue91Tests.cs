using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public abstract class Issue91Tests : BaseForTests
    {
        protected readonly Expression<Func<Foo, Qux>> Expression = foo => new Qux
        {
            TotalWeight = foo.Bars.Sum(y => y.Weight),
            Heights = foo.Bazs.Select(y => y.Weight)
        };

        protected Func<Issue91DbContext> DbContextFactory;

        protected DbContextOptions<Issue91DbContext> DbContextOptions;

        protected List<Qux> ExpectedQuxs;

        protected List<Foo> Foos;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            DbContextOptions = new DbContextOptionsBuilder<Issue91DbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            Foos = Fixture.CreateMany<Foo>().ToList();

            ExpectedQuxs = Foos.AsQueryable().Select(foo => new Qux
            {
                TotalWeight = foo.Bars.Sum(y => y.Weight),
                Heights = foo.Bazs.Select(y => y.Weight)
            }).ToList();
        }

        [Test]
        public virtual void SelectWithExpressionFunc_ReturnsSequence()
        {
            var dbContext = DbContextFactory();
            dbContext.Set<Foo>().AddRange(Foos);
            dbContext.SaveChanges();

            var actualQuxs = dbContext.Set<Foo>().Select(Expression).ToList();

            Assert.Multiple(() =>
            {
                actualQuxs.Should().NotBeEmpty();
                actualQuxs.Should().BeEquivalentTo(ExpectedQuxs);
            });
        }

        [Test]
        public virtual void SelectWithAnonymousExpressionFunc_ReturnsSequence()
        {
            var dbContext = DbContextFactory();
            dbContext.Set<Foo>().AddRange(Foos);
            dbContext.SaveChanges();

            var actualQuxs = dbContext.Set<Foo>().Select(foo => new Qux
            {
                TotalWeight = foo.Bars.Sum(y => y.Weight),
                Heights = foo.Bazs.Select(y => y.Weight)
            }).ToList();

            Assert.Multiple(() =>
            {
                actualQuxs.Should().NotBeEmpty();
                actualQuxs.Should().BeEquivalentTo(ExpectedQuxs);
            });
        }
    }

    public class Issue91DbContext : DbContext
    {
        public Issue91DbContext(DbContextOptions<Issue91DbContext> options) : base(options) { }

        public virtual DbSet<Foo> Foos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Foo>().HasKey(c => c.Id);

            modelBuilder.Entity<Bar>().HasKey(c => c.Id);

            modelBuilder.Entity<Baz>().HasKey(c => c.Id);
        }
    }

    public class Foo : BaseTestEntity
    {
        public List<Bar> Bars { get; set; }

        public List<Baz> Bazs { get; set; }
    }

    public class Bar : BaseTestEntity { }

    public class Baz : BaseTestEntity { }

    public class Qux
    {
        public decimal TotalWeight { get; set; }

        public IEnumerable<decimal> Heights { get; set; }
    }
}