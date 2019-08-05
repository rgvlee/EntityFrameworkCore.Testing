using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using EntityFrameworkCore.Testing.Moq.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    /// <summary>
    /// Extensions for db contexts.
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Creates and sets up a DbContext mock that delegates over the specified DbContext.
        /// </summary>
        /// <typeparam name="TDbContext">The DbContext to mock type.</typeparam>
        /// <param name="dbContextToMock">The DbContext to mock.</param>
        /// <returns>A DbContext mock that delegates over the specified DbContext.</returns>
        public static Mock<TDbContext> CreateDbContextMock<TDbContext>(this TDbContext dbContextToMock)
            where TDbContext : DbContext
        {
            var dbContextMock = new Mock<TDbContext>();

            dbContextMock.Setup(m => m.Add(It.IsAny<object>())).Returns((object entity) => dbContextToMock.Add(entity));
            dbContextMock.Setup(m => m.AddAsync(It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns(
                (object entity, CancellationToken cancellationToken) =>
                    dbContextToMock.AddAsync(entity, cancellationToken));
            dbContextMock.Setup(m => m.AddRange(It.IsAny<object[]>()))
                .Callback((object[] entities) => dbContextToMock.AddRange(entities));
            dbContextMock.Setup(m => m.AddRange(It.IsAny<IEnumerable<object>>()))
                .Callback((IEnumerable<object> entities) => dbContextToMock.AddRange(entities));
            dbContextMock.Setup(m => m.AddRangeAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(
                (object[] entities, CancellationToken cancellationToken) =>
                    dbContextToMock.AddRangeAsync(entities, cancellationToken));
            dbContextMock.Setup(m => m.AddRangeAsync(It.IsAny<IEnumerable<object>>(), It.IsAny<CancellationToken>()))
                .Returns((IEnumerable<object> entities, CancellationToken cancellationToken) =>
                    dbContextToMock.AddRangeAsync(entities, cancellationToken));
            dbContextMock.Setup(m => m.Attach(It.IsAny<object>()))
                .Returns((object entity) => dbContextToMock.Attach(entity));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<object[]>()))
                .Callback((object[] entities) => dbContextToMock.AttachRange(entities));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<IEnumerable<object>>()))
                .Callback((IEnumerable<object> entities) => dbContextToMock.AttachRange(entities));
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.ChangeDetector)
                .Returns(((IDbContextDependencies) dbContextToMock).ChangeDetector);
            dbContextMock.Setup(m => m.ChangeTracker).Returns(dbContextToMock.ChangeTracker);
            dbContextMock.Setup(m => m.Database).Returns(dbContextToMock.Database);
            dbContextMock.Setup(m => m.Dispose()).Callback(dbContextToMock.Dispose);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.EntityFinderFactory)
                .Returns(((IDbContextDependencies) dbContextToMock).EntityFinderFactory);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.EntityGraphAttacher)
                .Returns(((IDbContextDependencies) dbContextToMock).EntityGraphAttacher);
            dbContextMock.Setup(m => m.Entry(It.IsAny<object>()))
                .Returns((object entity) => dbContextToMock.Entry(entity));
            dbContextMock.Setup(m => m.FindAsync(It.IsAny<Type>(), It.IsAny<object[]>())).Returns(
                (Type entityType, object[] keyValues) => dbContextToMock.FindAsync(entityType, keyValues));
            dbContextMock.Setup(m => m.FindAsync(It.IsAny<Type>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns((Type entityType, object[] keyValues, CancellationToken cancellationToken) =>
                    dbContextToMock.FindAsync(entityType, keyValues, cancellationToken));
            dbContextMock.As<IDbQueryCache>().Setup(m => m.GetOrAddQuery(It.IsAny<IDbQuerySource>(), It.IsAny<Type>()))
                .Returns((IDbQuerySource source, Type type) =>
                    ((IDbQueryCache) dbContextToMock).GetOrAddQuery(source, type));
            dbContextMock.As<IDbSetCache>().Setup(m => m.GetOrAddSet(It.IsAny<IDbSetSource>(), It.IsAny<Type>()))
                .Returns((IDbSetSource source, Type type) => ((IDbSetCache) dbContextToMock).GetOrAddSet(source, type));
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.InfrastructureLogger)
                .Returns(((IDbContextDependencies) dbContextToMock).InfrastructureLogger);
            dbContextMock.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance)
                .Returns(((IInfrastructure<IServiceProvider>) dbContextToMock).Instance);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.Model)
                .Returns(((IDbContextDependencies) dbContextToMock).Model);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.QueryProvider)
                .Returns(((IDbContextDependencies) dbContextToMock).QueryProvider);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.QuerySource)
                .Returns(((IDbContextDependencies) dbContextToMock).QuerySource);
            dbContextMock.Setup(m => m.Remove(It.IsAny<object>()))
                .Returns((object entity) => dbContextToMock.Remove(entity));
            dbContextMock.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<object>>()))
                .Callback((IEnumerable<object> entities) => dbContextToMock.RemoveRange(entities));
            dbContextMock.Setup(m => m.RemoveRange(It.IsAny<object[]>()))
                .Callback((object[] entities) => dbContextToMock.RemoveRange(entities));
            dbContextMock.As<IDbContextPoolable>().Setup(m => m.ResetState())
                .Callback(() => ((IDbContextPoolable) dbContextToMock).ResetState());
            dbContextMock.As<IDbContextPoolable>()
                .Setup(m => m.Resurrect(It.IsAny<DbContextPoolConfigurationSnapshot>())).Callback(
                    (DbContextPoolConfigurationSnapshot configurationSnapshot) =>
                        ((IDbContextPoolable) dbContextToMock).Resurrect(configurationSnapshot));
            dbContextMock.Setup(m => m.SaveChanges()).Returns(dbContextToMock.SaveChanges);
            dbContextMock.Setup(m => m.SaveChanges(It.IsAny<bool>())).Returns((bool acceptAllChangesOnSuccess) =>
                dbContextToMock.SaveChanges(acceptAllChangesOnSuccess));
            dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(
                (CancellationToken cancellationToken) => dbContextToMock.SaveChangesAsync(cancellationToken));
            dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>())).Returns(
                (bool acceptAllChangesOnSuccess, CancellationToken cancellationToken) =>
                    dbContextToMock.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken));
            dbContextMock.As<IDbContextPoolable>().Setup(m => m.SetPool(It.IsAny<IDbContextPool>())).Callback(
                (IDbContextPool contextPool) => ((IDbContextPoolable) dbContextToMock).SetPool(contextPool));
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.SetSource)
                .Returns(((IDbContextDependencies) dbContextToMock).SetSource);
            dbContextMock.As<IDbContextPoolable>().Setup(m => m.SnapshotConfiguration())
                .Returns(((IDbContextPoolable) dbContextToMock).SnapshotConfiguration());
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.StateManager)
                .Returns(((IDbContextDependencies) dbContextToMock).StateManager);
            dbContextMock.Setup(m => m.Update(It.IsAny<object>()))
                .Returns((object entity) => dbContextToMock.Update(entity));

            dbContextMock.As<IDbContextDependencies>().Setup(m => m.UpdateLogger)
                .Returns(((IDbContextDependencies) dbContextToMock).UpdateLogger);

            dbContextMock.Setup(m => m.UpdateRange(It.IsAny<IEnumerable<object>>()))
                .Callback((IEnumerable<object> entities) => dbContextToMock.UpdateRange(entities));
            dbContextMock.Setup(m => m.UpdateRange(It.IsAny<object[]>()))
                .Callback((object[] entities) => dbContextToMock.UpdateRange(entities));

            return dbContextMock;
        }

        /// <summary>
        /// Creates and sets up a DbQuery mock for the specified entity.
        /// </summary>
        /// <typeparam name="TDbContext">The DbContext type.</typeparam>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbContext">The DbContext.</param>
        /// <param name="expression">The DbContext property to set up.</param>
        /// <param name="sequence">The sequence to use for the DbQuery.</param>
        /// <returns></returns>
        public static Mock<DbQuery<TQuery>> CreateDbQueryMockFor<TDbContext, TQuery>(this TDbContext dbContext,
            Expression<Func<TDbContext, DbQuery<TQuery>>> expression, IEnumerable<TQuery> sequence)
            where TDbContext : DbContext
            where TQuery : class
        {
            return DbQueryHelper.CreateDbQueryMock<TQuery>(sequence);
        }

        /// <summary>
        /// Creates and sets up a DbQuery mock for the specified entity.
        /// </summary>
        /// <typeparam name="TDbContext">The DbContext type.</typeparam>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbContext">The DbContext.</param>
        /// <param name="sequence">The sequence to use for the DbQuery.</param>
        /// <returns></returns>
        public static Mock<DbQuery<TQuery>> CreateDbQueryMockFor<TDbContext, TQuery>(this TDbContext dbContext,
            IEnumerable<TQuery> sequence)
            where TDbContext : DbContext
            where TQuery : class {
            return DbQueryHelper.CreateDbQueryMock<TQuery>(sequence);
        }
    }
}