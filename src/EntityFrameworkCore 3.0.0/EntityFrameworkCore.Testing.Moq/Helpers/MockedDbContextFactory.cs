#pragma warning disable EF1001 // Internal EF Core API usage.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using EntityFrameworkCore.Testing.Common.Helpers;
using EntityFrameworkCore.Testing.Moq.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Moq;

namespace EntityFrameworkCore.Testing.Moq.Helpers
{
    public class MockedDbContextFactory<TDbContext> : MockedDbContextFactoryBase<TDbContext> where TDbContext : DbContext
    {
        public MockedDbContextFactory(params object[] constructorParameters) : base(constructorParameters) { }

        /// <summary>Creates and sets up a mocked db context.</summary>
        /// <returns>A mocked db context.</returns>
        public override (TDbContext MockedDbContext, TDbContext DbContext) Create()
        {
            var dbContext = (TDbContext)Activator.CreateInstance(typeof(TDbContext), ConstructorParametersProvided ? ConstructorParameters : DefaultConstructorParameters);
            var dbContextMock = new Mock<TDbContext>(ConstructorParametersProvided ? ConstructorParameters : DefaultConstructorParameters);

            dbContextMock.Setup(m => m.Add(It.IsAny<object>())).Returns((object providedEntity) => dbContext.Add(providedEntity));
            dbContextMock.Setup(m => m.AddAsync(It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns((object providedEntity, CancellationToken providedCancellationToken) => dbContext.AddAsync(providedEntity, providedCancellationToken));
            dbContextMock.Setup(m => m.AddRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => dbContext.AddRange(providedEntities));
            dbContextMock.Setup(m => m.AddRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => dbContext.AddRange(providedEntities));
            dbContextMock.Setup(m => m.AddRangeAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns((object[] providedEntities, CancellationToken providedCancellationToken) => dbContext.AddRangeAsync(providedEntities, providedCancellationToken));
            dbContextMock.Setup(m => m.AddRangeAsync(It.IsAny<IEnumerable<object>>(), It.IsAny<CancellationToken>())).Returns((IEnumerable<object> providedEntities, CancellationToken providedCancellationToken) => dbContext.AddRangeAsync(providedEntities, providedCancellationToken));

            dbContextMock.Setup(m => m.Attach(It.IsAny<object>())).Returns((object providedEntity) => dbContext.Attach(providedEntity));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => dbContext.AttachRange(providedEntities));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => dbContext.AttachRange(providedEntities));

            dbContextMock.As<IDbContextDependencies>().Setup(m => m.ChangeDetector).Returns(((IDbContextDependencies) dbContext).ChangeDetector);
            dbContextMock.Setup(m => m.ChangeTracker).Returns(dbContext.ChangeTracker);
            dbContextMock.Setup(m => m.ContextId).Returns(dbContext.ContextId);
            dbContextMock.Setup(m => m.Database).Returns(dbContext.Database);
            dbContextMock.Setup(m => m.Dispose()).Callback(dbContext.Dispose);
            dbContextMock.Setup(m => m.DisposeAsync()).Callback(() => dbContext.DisposeAsync());
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.EntityFinderFactory).Returns(((IDbContextDependencies) dbContext).EntityFinderFactory);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.EntityGraphAttacher).Returns(((IDbContextDependencies) dbContext).EntityGraphAttacher);
            dbContextMock.Setup(m => m.Entry(It.IsAny<object>())).Returns((object providedEntity) => dbContext.Entry(providedEntity));

            dbContextMock.Setup(m => m.Find(It.IsAny<Type>(), It.IsAny<object[]>())).Returns((Type providedEntityType, object[] providedKeyValues) => dbContext.Find(providedEntityType, providedKeyValues));
            dbContextMock.Setup(m => m.FindAsync(It.IsAny<Type>(), It.IsAny<object[]>())).Returns((Type providedEntityType, object[] providedKeyValues) => dbContext.FindAsync(providedEntityType, providedKeyValues));
            dbContextMock.Setup(m => m.FindAsync(It.IsAny<Type>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns((Type providedEntityType, object[] providedKeyValues, CancellationToken providedCancellationToken) => dbContext.FindAsync(providedEntityType, providedKeyValues, providedCancellationToken));

            dbContextMock.As<IDbSetCache>().Setup(m => m.GetOrAddSet(It.IsAny<IDbSetSource>(), It.IsAny<Type>())).Returns((IDbSetSource providedSource, Type providedType) => ((IDbSetCache) dbContext).GetOrAddSet(providedSource, providedType));
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.InfrastructureLogger).Returns(((IDbContextDependencies) dbContext).InfrastructureLogger);
            dbContextMock.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance).Returns(((IInfrastructure<IServiceProvider>) dbContext).Instance);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.Model).Returns(((IDbContextDependencies) dbContext).Model);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.QueryProvider).Returns(((IDbContextDependencies) dbContext).QueryProvider);

            dbContextMock.Setup(m => m.Remove(It.IsAny<object>())).Returns((object providedEntity) => dbContext.Remove(providedEntity));
            dbContextMock.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => dbContext.RemoveRange(providedEntities));
            dbContextMock.Setup(m => m.RemoveRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => dbContext.RemoveRange(providedEntities));

            dbContextMock.As<IDbContextPoolable>().Setup(m => m.ResetState()).Callback(((IDbContextPoolable) dbContext).ResetState);
            dbContextMock.As<IDbContextPoolable>().Setup(m => m.ResetStateAsync(It.IsAny<CancellationToken>())).Callback((CancellationToken providedCancellationToken) => ((IDbContextPoolable) dbContext).ResetStateAsync(providedCancellationToken));
            dbContextMock.As<IDbContextPoolable>().Setup(m => m.Resurrect(It.IsAny<DbContextPoolConfigurationSnapshot>())).Callback((DbContextPoolConfigurationSnapshot providedConfigurationSnapshot) => ((IDbContextPoolable) dbContext).Resurrect(providedConfigurationSnapshot));

            dbContextMock.Setup(m => m.SaveChanges()).Returns(dbContext.SaveChanges);
            dbContextMock.Setup(m => m.SaveChanges(It.IsAny<bool>())).Returns((bool providedAcceptAllChangesOnSuccess) => dbContext.SaveChanges(providedAcceptAllChangesOnSuccess));
            dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken providedCancellationToken) => dbContext.SaveChangesAsync(providedCancellationToken));
            dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>())).Returns((bool providedAcceptAllChangesOnSuccess, CancellationToken providedCancellationToken) => dbContext.SaveChangesAsync(providedAcceptAllChangesOnSuccess, providedCancellationToken));

            dbContextMock.As<IDbContextPoolable>().Setup(m => m.SetPool(It.IsAny<IDbContextPool>())).Callback((IDbContextPool providedContextPool) => ((IDbContextPoolable) dbContext).SetPool(providedContextPool));
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.SetSource).Returns(((IDbContextDependencies) dbContext).SetSource);
            dbContextMock.As<IDbContextPoolable>().Setup(m => m.SnapshotConfiguration()).Returns(((IDbContextPoolable) dbContext).SnapshotConfiguration());
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.StateManager).Returns(((IDbContextDependencies) dbContext).StateManager);

            dbContextMock.Setup(m => m.Update(It.IsAny<object>())).Returns((object providedEntity) => dbContext.Update(providedEntity));

            dbContextMock.As<IDbContextDependencies>().Setup(m => m.UpdateLogger).Returns(((IDbContextDependencies) dbContext).UpdateLogger);

            dbContextMock.Setup(m => m.UpdateRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => dbContext.UpdateRange(providedEntities));
            dbContextMock.Setup(m => m.UpdateRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => dbContext.UpdateRange(providedEntities));

            foreach (var entity in dbContext.Model.GetEntityTypes().Where(x => x.FindPrimaryKey() != null))
            {
                typeof(MockedDbContextFactory<TDbContext>)
                    .GetMethod(nameof(SetUpDbSetFor), BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(entity.ClrType).Invoke(this, new object[] {dbContextMock, dbContext });
            }

            foreach (var entity in dbContext.Model.GetEntityTypes().Where(x => x.FindPrimaryKey() == null))
            {
                typeof(MockedDbContextFactory<TDbContext>)
                    .GetMethod(nameof(SetUpReadOnlyDbSetFor), BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(entity.ClrType).Invoke(this, new object[] {dbContextMock, dbContext });
            }

            return (MockedDbContext: dbContextMock.Object, DbContext: dbContext);
        }

        private void SetUpDbSetFor<TEntity>(Mock<TDbContext> dbContextMock, TDbContext dbContext)
            where TEntity : class
        {
            var mockedDbSet = dbContext.Set<TEntity>().CreateMockedDbSet();

            var property = typeof(TDbContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof(DbSet<TEntity>));

            if (property != null)
            {
                var expression = ExpressionHelper.CreatePropertyExpression<TDbContext, DbSet<TEntity>>(property);
                dbContextMock.Setup(expression).Returns(mockedDbSet);
            }
            else
            {
                Logger.LogDebug($"Could not find a DbContext property for type '{typeof(TEntity)}'");
            }

            dbContextMock.Setup(m => m.Set<TEntity>()).Returns(mockedDbSet);

            dbContextMock.Setup(m => m.Add(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => dbContext.Add(providedEntity));
            dbContextMock.Setup(m => m.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>())).Returns((TEntity providedEntity, CancellationToken providedCancellationToken) => dbContext.AddAsync(providedEntity, providedCancellationToken));

            dbContextMock.Setup(m => m.Attach(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => dbContext.Attach(providedEntity));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => dbContext.AttachRange(providedEntities));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => dbContext.AttachRange(providedEntities));

            dbContextMock.Setup(m => m.Entry(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => dbContext.Entry(providedEntity));

            dbContextMock.Setup(m => m.Find<TEntity>(It.IsAny<object[]>())).Returns((object[] providedKeyValues) => dbContext.Find<TEntity>(providedKeyValues));
            dbContextMock.Setup(m => m.Find(typeof(TEntity), It.IsAny<object[]>())).Returns((Type providedType, object[] providedKeyValues) => dbContext.Find(providedType, providedKeyValues));
            dbContextMock.Setup(m => m.FindAsync<TEntity>(It.IsAny<object[]>())).Returns((object[] providedKeyValues) => dbContext.FindAsync<TEntity>(providedKeyValues));
            dbContextMock.Setup(m => m.FindAsync<TEntity>(It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns((object[] providedKeyValues, CancellationToken providedCancellationToken) => dbContext.FindAsync<TEntity>(providedKeyValues, providedCancellationToken));

            dbContextMock.Setup(m => m.Remove(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => dbContext.Remove(providedEntity));

            dbContextMock.Setup(m => m.Update(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => dbContext.Update(providedEntity));
        }

        private void SetUpReadOnlyDbSetFor<TEntity>(Mock<TDbContext> dbContextMock, TDbContext dbContext)
            where TEntity : class
        {
            var mockedReadOnlyDbSet = dbContext.Set<TEntity>().CreateMockedReadOnlyDbSet();

            var dbSetProperty = typeof(TDbContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof(DbSet<TEntity>));
            if (dbSetProperty != null)
            {
                var setExpression = ExpressionHelper.CreatePropertyExpression<TDbContext, DbSet<TEntity>>(dbSetProperty);
                dbContextMock.Setup(setExpression).Returns(mockedReadOnlyDbSet);
                dbContextMock.Setup(m => m.Set<TEntity>()).Returns(mockedReadOnlyDbSet);
                return;
            }

            var dbQueryProperty = typeof(TDbContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof(DbQuery<TEntity>));
            if (dbQueryProperty != null)
            {
                var setExpression = ExpressionHelper.CreatePropertyExpression<TDbContext, DbQuery<TEntity>>(dbQueryProperty);
                dbContextMock.Setup(setExpression).Returns(mockedReadOnlyDbSet);
                dbContextMock.Setup(m => m.Query<TEntity>()).Returns(mockedReadOnlyDbSet);
                return;
            }

            Logger.LogDebug($"Could not find a DbContext property for type '{typeof(TEntity)}'");
        }
    }
}