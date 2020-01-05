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
        public override TDbContext Create()
        {
            var dbContextMock = new Mock<TDbContext>(ConstructorParametersProvided ? ConstructorParameters : DefaultConstructorParameters);

            dbContextMock.Setup(m => m.Add(It.IsAny<object>())).Returns((object providedEntity) => DbContextToMock.Add(providedEntity));
            dbContextMock.Setup(m => m.AddAsync(It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns((object providedEntity, CancellationToken providedCancellationToken) => DbContextToMock.AddAsync(providedEntity, providedCancellationToken));
            dbContextMock.Setup(m => m.AddRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => DbContextToMock.AddRange(providedEntities));
            dbContextMock.Setup(m => m.AddRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => DbContextToMock.AddRange(providedEntities));
            dbContextMock.Setup(m => m.AddRangeAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns((object[] providedEntities, CancellationToken providedCancellationToken) => DbContextToMock.AddRangeAsync(providedEntities, providedCancellationToken));
            dbContextMock.Setup(m => m.AddRangeAsync(It.IsAny<IEnumerable<object>>(), It.IsAny<CancellationToken>())).Returns((IEnumerable<object> providedEntities, CancellationToken providedCancellationToken) => DbContextToMock.AddRangeAsync(providedEntities, providedCancellationToken));

            dbContextMock.Setup(m => m.Attach(It.IsAny<object>())).Returns((object providedEntity) => DbContextToMock.Attach(providedEntity));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => DbContextToMock.AttachRange(providedEntities));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => DbContextToMock.AttachRange(providedEntities));

            dbContextMock.As<IDbContextDependencies>().Setup(m => m.ChangeDetector).Returns(((IDbContextDependencies) DbContextToMock).ChangeDetector);
            dbContextMock.Setup(m => m.ChangeTracker).Returns(DbContextToMock.ChangeTracker);
            dbContextMock.Setup(m => m.Database).Returns(DbContextToMock.Database);
            dbContextMock.Setup(m => m.Dispose()).Callback(DbContextToMock.Dispose);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.EntityFinderFactory).Returns(((IDbContextDependencies) DbContextToMock).EntityFinderFactory);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.EntityGraphAttacher).Returns(((IDbContextDependencies) DbContextToMock).EntityGraphAttacher);
            dbContextMock.Setup(m => m.Entry(It.IsAny<object>())).Returns((object providedEntity) => DbContextToMock.Entry(providedEntity));

            dbContextMock.Setup(m => m.FindAsync(It.IsAny<Type>(), It.IsAny<object[]>())).Returns((Type providedEntityType, object[] providedKeyValues) => DbContextToMock.FindAsync(providedEntityType, providedKeyValues));
            dbContextMock.Setup(m => m.FindAsync(It.IsAny<Type>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns((Type providedEntityType, object[] providedKeyValues, CancellationToken providedCancellationToken) => DbContextToMock.FindAsync(providedEntityType, providedKeyValues, providedCancellationToken));

            dbContextMock.As<IDbQueryCache>().Setup(m => m.GetOrAddQuery(It.IsAny<IDbQuerySource>(), It.IsAny<Type>())).Returns((IDbQuerySource providedSource, Type providedType) => ((IDbQueryCache) DbContextToMock).GetOrAddQuery(providedSource, providedType));
            dbContextMock.As<IDbSetCache>().Setup(m => m.GetOrAddSet(It.IsAny<IDbSetSource>(), It.IsAny<Type>())).Returns((IDbSetSource providedSource, Type providedType) => ((IDbSetCache) DbContextToMock).GetOrAddSet(providedSource, providedType));
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.InfrastructureLogger).Returns(((IDbContextDependencies) DbContextToMock).InfrastructureLogger);
            dbContextMock.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance).Returns(((IInfrastructure<IServiceProvider>) DbContextToMock).Instance);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.Model).Returns(((IDbContextDependencies) DbContextToMock).Model);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.QueryProvider).Returns(((IDbContextDependencies) DbContextToMock).QueryProvider);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.QuerySource).Returns(((IDbContextDependencies) DbContextToMock).QuerySource);

            dbContextMock.Setup(m => m.Remove(It.IsAny<object>())).Returns((object providedEntity) => DbContextToMock.Remove(providedEntity));
            dbContextMock.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => DbContextToMock.RemoveRange(providedEntities));
            dbContextMock.Setup(m => m.RemoveRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => DbContextToMock.RemoveRange(providedEntities));

            dbContextMock.As<IDbContextPoolable>().Setup(m => m.ResetState()).Callback(((IDbContextPoolable) DbContextToMock).ResetState);
            dbContextMock.As<IDbContextPoolable>().Setup(m => m.Resurrect(It.IsAny<DbContextPoolConfigurationSnapshot>())).Callback((DbContextPoolConfigurationSnapshot providedConfigurationSnapshot) => ((IDbContextPoolable) DbContextToMock).Resurrect(providedConfigurationSnapshot));

            dbContextMock.Setup(m => m.SaveChanges()).Returns(DbContextToMock.SaveChanges);
            dbContextMock.Setup(m => m.SaveChanges(It.IsAny<bool>())).Returns((bool providedAcceptAllChangesOnSuccess) => DbContextToMock.SaveChanges(providedAcceptAllChangesOnSuccess));
            dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken providedCancellationToken) => DbContextToMock.SaveChangesAsync(providedCancellationToken));
            dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>())).Returns((bool providedAcceptAllChangesOnSuccess, CancellationToken providedCancellationToken) => DbContextToMock.SaveChangesAsync(providedAcceptAllChangesOnSuccess, providedCancellationToken));

            dbContextMock.As<IDbContextPoolable>().Setup(m => m.SetPool(It.IsAny<IDbContextPool>())).Callback((IDbContextPool providedContextPool) => ((IDbContextPoolable) DbContextToMock).SetPool(providedContextPool));
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.SetSource).Returns(((IDbContextDependencies) DbContextToMock).SetSource);
            dbContextMock.As<IDbContextPoolable>().Setup(m => m.SnapshotConfiguration()).Returns(((IDbContextPoolable) DbContextToMock).SnapshotConfiguration());
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.StateManager).Returns(((IDbContextDependencies) DbContextToMock).StateManager);

            dbContextMock.Setup(m => m.Update(It.IsAny<object>())).Returns((object providedEntity) => DbContextToMock.Update(providedEntity));

            dbContextMock.As<IDbContextDependencies>().Setup(m => m.UpdateLogger).Returns(((IDbContextDependencies) DbContextToMock).UpdateLogger);

            dbContextMock.Setup(m => m.UpdateRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => DbContextToMock.UpdateRange(providedEntities));
            dbContextMock.Setup(m => m.UpdateRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => DbContextToMock.UpdateRange(providedEntities));

            foreach (var entity in DbContextToMock.Model.GetEntityTypes().Where(x => !x.IsQueryType))
            {
                typeof(MockedDbContextFactory<TDbContext>)
                    .GetMethod(nameof(SetUpDbSetFor), BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(entity.ClrType).Invoke(this, new object[] {dbContextMock});
            }

            foreach (var entity in DbContextToMock.Model.GetEntityTypes().Where(x => x.IsQueryType))
            {
                typeof(MockedDbContextFactory<TDbContext>)
                    .GetMethod(nameof(SetUpDbQueryFor), BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(entity.ClrType).Invoke(this, new object[] {dbContextMock});
            }

            return dbContextMock.Object;
        }

        private void SetUpDbSetFor<TEntity>(Mock<TDbContext> dbContextMock)
            where TEntity : class
        {
            var mockedDbSet = DbContextToMock.Set<TEntity>().CreateMockedDbSet();

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

            dbContextMock.Setup(m => m.Add(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => DbContextToMock.Add(providedEntity));
            dbContextMock.Setup(m => m.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>())).Returns((TEntity providedEntity, CancellationToken providedCancellationToken) => DbContextToMock.AddAsync(providedEntity, providedCancellationToken));

            dbContextMock.Setup(m => m.Attach(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => DbContextToMock.Attach(providedEntity));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => DbContextToMock.AttachRange(providedEntities));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => DbContextToMock.AttachRange(providedEntities));

            dbContextMock.Setup(m => m.Entry(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => DbContextToMock.Entry(providedEntity));

            dbContextMock.Setup(m => m.Find<TEntity>(It.IsAny<object[]>())).Returns((object[] providedKeyValues) => DbContextToMock.Find<TEntity>(providedKeyValues));
            dbContextMock.Setup(m => m.Find(typeof(TEntity), It.IsAny<object[]>())).Returns((Type providedType, object[] providedKeyValues) => DbContextToMock.Find(providedType, providedKeyValues));
            dbContextMock.Setup(m => m.FindAsync<TEntity>(It.IsAny<object[]>())).Returns((object[] providedKeyValues) => DbContextToMock.FindAsync<TEntity>(providedKeyValues));
            dbContextMock.Setup(m => m.FindAsync<TEntity>(It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns((object[] providedKeyValues, CancellationToken providedCancellationToken) => DbContextToMock.FindAsync<TEntity>(providedKeyValues, providedCancellationToken));

            dbContextMock.Setup(m => m.Remove(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => DbContextToMock.Remove(providedEntity));

            dbContextMock.Setup(m => m.Update(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => DbContextToMock.Update(providedEntity));
        }

        private void SetUpDbQueryFor<TQuery>(Mock<TDbContext> dbContextMock)
            where TQuery : class
        {
            var mockedDbQuery = DbContextToMock.Query<TQuery>().CreateMockedDbQuery();

            var property = typeof(TDbContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof(DbQuery<TQuery>));

            if (property != null)
            {
                var expression = ExpressionHelper.CreatePropertyExpression<TDbContext, DbQuery<TQuery>>(property);
                dbContextMock.Setup(expression).Returns(mockedDbQuery);
            }
            else
            {
                Logger.LogDebug($"Could not find a DbContext property for type '{typeof(TQuery)}'");
            }

            dbContextMock.Setup(m => m.Query<TQuery>()).Returns(mockedDbQuery);
        }
    }
}