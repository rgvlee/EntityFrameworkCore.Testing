using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests;

public abstract class Issue126Tests<TDbContext> : BaseForTests where TDbContext : DbContext
{
    protected Func<TDbContext> DbContextFactory;

    [Test]
    public virtual void BeginTransaction_ReturnsMockTransaction()
    {
        using var transaction = DbContextFactory().Database.BeginTransaction();
        transaction.Should().NotBeNull();
    }

    [TestCase(true)]
    [TestCase(false)]
    public virtual async Task BeginTransactionAsync_ReturnsMockTransaction(bool withCancellationTokenParameter)
    {
        await using var transaction = withCancellationTokenParameter
            ? await DbContextFactory().Database.BeginTransactionAsync(CancellationToken.None)
            : await DbContextFactory().Database.BeginTransactionAsync();
        
        transaction.Should().NotBeNull();
    }
}