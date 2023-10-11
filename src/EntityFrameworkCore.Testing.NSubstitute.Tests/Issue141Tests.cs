using System;
using System.Linq;
using AutoFixture;
using EntityFrameworkCore.Testing.Common.Tests;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.NSubstitute.Tests;

public class Issue141Tests : BaseForTests
{
    [Test]
    public void JoinThenProject_TablesToDto_DoesNotThrow()
    {
        var fixture = new Fixture();
        var context = Create.MockedDbContextFor<Issue141Context>();

        var tableAEntities = fixture.CreateMany<TableA>().ToList();
        var tableBEntities = fixture.Build<TableB>().With(x => x.TableAId, tableAEntities.First().Id).CreateMany().ToList();
        context.TableAEntities.AddRange(tableAEntities);
        context.TableBEntities.AddRange(tableBEntities);
        context.SaveChanges();

        var result = context.TableAEntities
            .Join(
                context.TableBEntities,
                l => l.Id,
                r => r.TableAId,
                (l, r) => new Aggregate { LeftId = l.Id, LeftName = l.Name, RightId = r.Id, RightName = r.Name })
            .ToList();

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Select(x => x.LeftId).Should().AllBeEquivalentTo(tableAEntities.First().Id);
            result.Select(x => x.LeftName).Should().AllBeEquivalentTo(tableAEntities.First().Name);
            result.Select(x => x.RightId).Should().BeEquivalentTo(tableBEntities.Select(x => x.Id));
            result.Select(x => x.RightName).Should().BeEquivalentTo(tableBEntities.Select(x => x.Name));
        }
    }

    [Test]
    public void JoinThenProject_TableAndViewToDto_DoesNotThrow()
    {
        var fixture = new Fixture();
        var context = Create.MockedDbContextFor<Issue141Context>();

        var tableAEntities = fixture.CreateMany<TableA>().ToList();
        var viewCEntities = fixture.Build<ViewC>().With(x => x.TableAId, tableAEntities.First().Id).CreateMany().ToList();
        context.TableAEntities.AddRange(tableAEntities);
        context.ViewCEntities.AddRangeToReadOnlySource(viewCEntities);
        context.SaveChanges();

        var result = context.TableAEntities
            .Join(
                context.ViewCEntities,
                l => l.Id,
                r => r.TableAId,
                (l, r) => new Aggregate { LeftId = l.Id, LeftName = l.Name, RightId = r.Id, RightName = r.Name })
            .ToList();

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Select(x => x.LeftId).Should().AllBeEquivalentTo(tableAEntities.First().Id);
            result.Select(x => x.LeftName).Should().AllBeEquivalentTo(tableAEntities.First().Name);
            result.Select(x => x.RightId).Should().BeEquivalentTo(viewCEntities.Select(x => x.Id));
            result.Select(x => x.RightName).Should().BeEquivalentTo(viewCEntities.Select(x => x.Name));
        }
    }

    public class Issue141Context : DbContext
    {
        public Issue141Context(DbContextOptions<Issue141Context> options) : base(options) { }

        public virtual DbSet<TableA> TableAEntities { get; set; }

        public virtual DbSet<TableB> TableBEntities { get; set; }

        public virtual DbSet<ViewC> ViewCEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TableA>().HasKey(x => x.Id);
            modelBuilder.Entity<TableB>().HasKey(x => x.Id);
            modelBuilder.Entity<ViewC>().HasNoKey().ToView("ViewC");
        }
    }

    public class TableA
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class TableB
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid TableAId { get; set; }
    }

    public class ViewC
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid TableAId { get; set; }
    }

    public class Aggregate
    {
        public Guid LeftId { get; set; }

        public string LeftName { get; set; }

        public Guid RightId { get; set; }

        public string RightName { get; set; }
    }
}