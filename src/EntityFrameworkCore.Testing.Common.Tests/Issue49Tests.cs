using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EntityFrameworkCore.Testing.Common.Tests
{
    public abstract class Issue49Tests<TDbContext, TEntity> : BaseForTests where TDbContext : DbContext
        where TEntity : BaseTestEntity
    {
        protected TDbContext DbContext;

        [Test]
        public virtual void EntityEntryState_Entity_IsDetached()
        {
            var entity = Fixture.Create<TEntity>();

            var actualResult = DbContext.Entry(entity).State;

            actualResult.Should().Be(EntityState.Detached);
        }

        [Test]
        public virtual void EntityEntryState_EntityAsObject_IsDetached()
        {
            var entity = Fixture.Create<TEntity>();

            var actualResult = DbContext.Entry((object) entity).State;

            actualResult.Should().Be(EntityState.Detached);
        }

        [Test]
        public virtual void DbContextAdd_Entity_IsAdded()
        {
            var entity = Fixture.Create<TEntity>();

            var actualResult = DbContext.Add(entity);

            actualResult.State.Should().Be(EntityState.Added);
        }
    }
}