using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using EntityFrameworkCore.Testing.Common.Tests;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using static FluentAssertions.FluentActions;

namespace EntityFrameworkCore.DefaultBehaviour.Tests;

public class ByTypeDbSetTests : BaseForQueryableTests<TestEntity>
{
    protected TestDbContext DbContext;

    protected DbSet<TestEntity> DbSet => DbContext.Set<TestEntity>();

    protected override IQueryable<TestEntity> Queryable => DbSet;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        DbContext = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
    }

    protected override void SeedQueryableSource()
    {
        var itemsToAdd = Fixture.Build<TestEntity>().With(p => p.CreatedAt, DateTime.Today).With(p => p.LastModifiedAt, DateTime.Today).CreateMany().ToList();
        DbContext.Set<TestEntity>().AddRange(itemsToAdd);
        DbContext.SaveChanges();
        ItemsAddedToQueryableSource = itemsToAdd;
    }

    [Test]
    public virtual async Task AsAsyncEnumerable_ReturnsAsyncEnumerable()
    {
        var expectedResult = Fixture.Create<TestEntity>();
        DbSet.Add(expectedResult);
        DbContext.SaveChanges();

        var asyncEnumerable = DbSet.AsAsyncEnumerable();

        var actualResults = new List<TestEntity>();
        await foreach (var item in asyncEnumerable) actualResults.Add(item);

        Assert.Multiple(() =>
        {
            Assert.That(actualResults.Single(), Is.EqualTo(expectedResult));
            Assert.That(actualResults.Single(), Is.EqualTo(expectedResult));
        });
    }

    [Test]
    public virtual void AsQueryable_ReturnsQueryable()
    {
        var expectedResult = Fixture.Create<TestEntity>();
        DbSet.Add(expectedResult);
        DbContext.SaveChanges();

        var queryable = DbSet.AsQueryable();

        Assert.Multiple(() =>
        {
            Assert.That(queryable.Single(), Is.EqualTo(expectedResult));
            Assert.That(queryable.Single(), Is.EqualTo(expectedResult));
        });
    }

    [Test]
    public virtual void FromSqlInterpolated_ThrowsException()
    {
        Invoking(() => DbSet.FromSqlInterpolated($"sp_NoParams").ToList()).Should().ThrowExactly<InvalidOperationException>();
    }

    [Test]
    public virtual void FromSqlRaw_ThrowsException()
    {
        Invoking(() => DbSet.FromSqlRaw("sp_NoParams").ToList()).Should().ThrowExactly<InvalidOperationException>();
    }
}