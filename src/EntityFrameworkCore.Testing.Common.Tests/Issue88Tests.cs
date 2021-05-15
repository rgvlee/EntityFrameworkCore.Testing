using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public abstract class Issue88Tests<TDbContext, TEntity> : BaseForTests where TDbContext : DbContext
        where TEntity : BaseTestEntity
    {
        protected Func<TDbContext> DbContextFactory;

        [Test]
        public virtual void AddThenSaveChanges_ChangesVisibleAnyDbContext()
        {
            var entity = Fixture.Create<TEntity>();

            var dbContext = DbContextFactory();

            dbContext.Set<TEntity>().Add(entity);
            dbContext.SaveChanges();

            dbContext.Find<TEntity>(entity.Id).Should().BeEquivalentTo(entity);
            dbContext.Set<TEntity>().Single(x => x.Id.Equals(entity.Id)).Should().BeEquivalentTo(entity);

            var anotherDbContext = DbContextFactory();
            anotherDbContext.Find<TEntity>(entity.Id).Should().BeEquivalentTo(entity);
            anotherDbContext.Set<TEntity>().Single(x => x.Id.Equals(entity.Id)).Should().BeEquivalentTo(entity);
        }

        [Test]
        public virtual void BeginTransactionThenAddThenSaveChangesThenCommit_ChangesVisibleAnyDbContext()
        {
            var entity = Fixture.Create<TEntity>();

            var dbContext = DbContextFactory();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                dbContext.Set<TEntity>().Add(entity);
                dbContext.SaveChanges();
                transaction.Commit();
            }

            dbContext.Find<TEntity>(entity.Id).Should().BeEquivalentTo(entity);
            dbContext.Set<TEntity>().Single(x => x.Id.Equals(entity.Id)).Should().BeEquivalentTo(entity);

            var anotherDbContext = DbContextFactory();
            anotherDbContext.Find<TEntity>(entity.Id).Should().BeEquivalentTo(entity);
            anotherDbContext.Set<TEntity>().Single(x => x.Id.Equals(entity.Id)).Should().BeEquivalentTo(entity);
        }
    }
}