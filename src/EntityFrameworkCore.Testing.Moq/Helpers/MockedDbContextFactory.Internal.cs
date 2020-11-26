#pragma warning disable EF1001 // Internal EF Core API usage.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using EntityFrameworkCore.Testing.Common.Helpers;
using EntityFrameworkCore.Testing.Moq.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace EntityFrameworkCore.Testing.Moq.Helpers
{
    internal class MockedDbContextFactory<TDbContext> : BaseMockedDbContextFactory<TDbContext> where TDbContext : DbContext
    {
        public MockedDbContextFactory(MockedDbContextFactoryOptions<TDbContext> options) : base(options) { }

        public override TDbContext Create()
        {
            var dbContextMock = new Mock<TDbContext>(ConstructorParameters.ToArray());

            dbContextMock.DefaultValueProvider = new NoSetUpDefaultValueProvider();

            dbContextMock.Setup(m => m.Add(It.IsAny<object>())).Returns((object providedEntity) => DbContext.Add(providedEntity));
            dbContextMock.Setup(m => m.AddAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns((object providedEntity, CancellationToken providedCancellationToken) => DbContext.AddAsync(providedEntity, providedCancellationToken));
            dbContextMock.Setup(m => m.AddRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => DbContext.AddRange(providedEntities));
            dbContextMock.Setup(m => m.AddRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => DbContext.AddRange(providedEntities));
            dbContextMock.Setup(m => m.AddRangeAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns((object[] providedEntities, CancellationToken providedCancellationToken) => DbContext.AddRangeAsync(providedEntities, providedCancellationToken));
            dbContextMock.Setup(m => m.AddRangeAsync(It.IsAny<IEnumerable<object>>(), It.IsAny<CancellationToken>()))
                .Returns((IEnumerable<object> providedEntities, CancellationToken providedCancellationToken) =>
                    DbContext.AddRangeAsync(providedEntities, providedCancellationToken));

            dbContextMock.Setup(m => m.Attach(It.IsAny<object>())).Returns((object providedEntity) => DbContext.Attach(providedEntity));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => DbContext.AttachRange(providedEntities));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => DbContext.AttachRange(providedEntities));

            dbContextMock.As<IDbContextDependencies>().Setup(m => m.ChangeDetector).Returns(((IDbContextDependencies) DbContext).ChangeDetector);
            dbContextMock.Setup(m => m.ChangeTracker).Returns(DbContext.ChangeTracker);
            dbContextMock.Setup(m => m.ContextId).Returns(DbContext.ContextId);
            dbContextMock.Setup(m => m.Database).Returns(DbContext.Database);
            dbContextMock.Setup(m => m.Dispose()).Callback(DbContext.Dispose);
            dbContextMock.Setup(m => m.DisposeAsync()).Callback(() => DbContext.DisposeAsync());
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.EntityFinderFactory).Returns(((IDbContextDependencies) DbContext).EntityFinderFactory);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.EntityGraphAttacher).Returns(((IDbContextDependencies) DbContext).EntityGraphAttacher);
            dbContextMock.Setup(m => m.Entry(It.IsAny<object>())).Returns((object providedEntity) => DbContext.Entry(providedEntity));

            dbContextMock.Setup(m => m.Find(It.IsAny<Type>(), It.IsAny<object[]>()))
                .Returns((Type providedEntityType, object[] providedKeyValues) => DbContext.Find(providedEntityType, providedKeyValues));
            dbContextMock.Setup(m => m.FindAsync(It.IsAny<Type>(), It.IsAny<object[]>()))
                .Returns((Type providedEntityType, object[] providedKeyValues) => DbContext.FindAsync(providedEntityType, providedKeyValues));
            dbContextMock.Setup(m => m.FindAsync(It.IsAny<Type>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns((Type providedEntityType, object[] providedKeyValues, CancellationToken providedCancellationToken) =>
                    DbContext.FindAsync(providedEntityType, providedKeyValues, providedCancellationToken));

            dbContextMock.As<IDbSetCache>()
                .Setup(m => m.GetOrAddSet(It.IsAny<IDbSetSource>(), It.IsAny<Type>()))
                .Returns((IDbSetSource providedSource, Type providedType) => ((IDbSetCache) DbContext).GetOrAddSet(providedSource, providedType));
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.InfrastructureLogger).Returns(((IDbContextDependencies) DbContext).InfrastructureLogger);
            dbContextMock.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance).Returns(((IInfrastructure<IServiceProvider>) DbContext).Instance);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.Model).Returns(((IDbContextDependencies) DbContext).Model);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.QueryProvider).Returns(((IDbContextDependencies) DbContext).QueryProvider);

            dbContextMock.Setup(m => m.Remove(It.IsAny<object>())).Returns((object providedEntity) => DbContext.Remove(providedEntity));
            dbContextMock.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => DbContext.RemoveRange(providedEntities));
            dbContextMock.Setup(m => m.RemoveRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => DbContext.RemoveRange(providedEntities));

            dbContextMock.As<IDbContextPoolable>().Setup(m => m.ResetState()).Callback(((IDbContextPoolable) DbContext).ResetState);
            dbContextMock.As<IDbContextPoolable>()
                .Setup(m => m.ResetStateAsync(It.IsAny<CancellationToken>()))
                .Callback((CancellationToken providedCancellationToken) => ((IDbContextPoolable) DbContext).ResetStateAsync(providedCancellationToken));
            // dbContextMock.As<IDbContextPoolable>()
            //     .Setup(m => m.Resurrect(It.IsAny<DbContextPoolConfigurationSnapshot>()))
            //     .Callback((DbContextPoolConfigurationSnapshot providedConfigurationSnapshot) => ((IDbContextPoolable) DbContext).Resurrect(providedConfigurationSnapshot));

            dbContextMock.Setup(m => m.SaveChanges()).Returns(DbContext.SaveChanges);
            dbContextMock.Setup(m => m.SaveChanges(It.IsAny<bool>())).Returns((bool providedAcceptAllChangesOnSuccess) => DbContext.SaveChanges(providedAcceptAllChangesOnSuccess));
            dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns((CancellationToken providedCancellationToken) => DbContext.SaveChangesAsync(providedCancellationToken));
            dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns((bool providedAcceptAllChangesOnSuccess, CancellationToken providedCancellationToken) =>
                    DbContext.SaveChangesAsync(providedAcceptAllChangesOnSuccess, providedCancellationToken));

            // dbContextMock.As<IDbContextPoolable>()
            //     .Setup(m => m.SetPool(It.IsAny<IDbContextPool>()))
            //     .Callback((IDbContextPool providedContextPool) => ((IDbContextPoolable) DbContext).SetPool(providedContextPool));
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.SetSource).Returns(((IDbContextDependencies) DbContext).SetSource);
            // dbContextMock.As<IDbContextPoolable>().Setup(m => m.SnapshotConfiguration()).Returns(((IDbContextPoolable) DbContext).SnapshotConfiguration());
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.StateManager).Returns(((IDbContextDependencies) DbContext).StateManager);

            dbContextMock.Setup(m => m.Update(It.IsAny<object>())).Returns((object providedEntity) => DbContext.Update(providedEntity));

            dbContextMock.As<IDbContextDependencies>().Setup(m => m.UpdateLogger).Returns(((IDbContextDependencies) DbContext).UpdateLogger);

            dbContextMock.Setup(m => m.UpdateRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => DbContext.UpdateRange(providedEntities));
            dbContextMock.Setup(m => m.UpdateRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => DbContext.UpdateRange(providedEntities));

            foreach (var entity in DbContext.Model.GetEntityTypes().Where(x => x.FindPrimaryKey() != null))
            {
                typeof(MockedDbContextFactory<TDbContext>).GetMethod(nameof(SetUpDbSetFor), BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(entity.ClrType)
                    .Invoke(this, new object[] { dbContextMock });
            }

            foreach (var entity in DbContext.Model.GetEntityTypes().Where(x => x.FindPrimaryKey() == null))
            {
                typeof(MockedDbContextFactory<TDbContext>).GetMethod(nameof(SetUpReadOnlyDbSetFor), BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(entity.ClrType)
                    .Invoke(this, new object[] { dbContextMock });
            }

            //Imported from AddExecuteSqlRawResult start
            var rawSqlCommandBuilderMock = new Mock<IRawSqlCommandBuilder>();
            rawSqlCommandBuilderMock.Setup(m => m.Build(It.IsAny<string>(), It.IsAny<IEnumerable<object>>()))
                .Callback((string providedSql, IEnumerable<object> providedParameters) => Logger.LogDebug("Catch all exception invoked"))
                .Throws<InvalidOperationException>();

            var rawSqlCommandBuilder = rawSqlCommandBuilderMock.Object;

            var concurrencyDetectorMock = new Mock<IConcurrencyDetector>();
            var concurrencyDetector = concurrencyDetectorMock.Object;
            concurrencyDetectorMock.Setup(x => x.EnterCriticalSection()).Returns(new ConcurrencyDetectorCriticalSectionDisposer(concurrencyDetector));

            var dependenciesMock = new Mock<IRelationalDatabaseFacadeDependencies>();
            dependenciesMock.Setup(m => m.ConcurrencyDetector).Returns(concurrencyDetector);
            dependenciesMock.Setup(m => m.CommandLogger).Returns(Mock.Of<IDiagnosticsLogger<DbLoggerCategory.Database.Command>>());
            dependenciesMock.Setup(m => m.RawSqlCommandBuilder).Returns(rawSqlCommandBuilder);
            dependenciesMock.Setup(m => m.RelationalConnection).Returns(Mock.Of<IRelationalConnection>());
            var dependencies = dependenciesMock.Object;

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IDatabaseFacadeDependencies)))).Returns((Type providedType) => dependencies);
            var serviceProvider = serviceProviderMock.Object;

            dbContextMock.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance).Returns(serviceProvider);
            //Imported from AddExecuteSqlRawResult end

            var mockedDbContext = dbContextMock.Object;

            var databaseFacadeMock = new Mock<DatabaseFacade>(mockedDbContext);
            databaseFacadeMock.As<IDatabaseFacadeDependenciesAccessor>().Setup(x => x.Dependencies).Returns(dependencies);
            var databaseFacade = databaseFacadeMock.Object;

            dbContextMock.Setup(m => m.Database).Returns(databaseFacade);

            return mockedDbContext;
        }

        private void SetUpDbSetFor<TEntity>(Mock<TDbContext> dbContextMock) where TEntity : class
        {
            var mockedDbSet = DbContext.Set<TEntity>().CreateMockedDbSet();

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

            dbContextMock.Setup(m => m.Add(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => DbContext.Add(providedEntity));
            dbContextMock.Setup(m => m.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
                .Returns((TEntity providedEntity, CancellationToken providedCancellationToken) => DbContext.AddAsync(providedEntity, providedCancellationToken));

            dbContextMock.Setup(m => m.Attach(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => DbContext.Attach(providedEntity));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => DbContext.AttachRange(providedEntities));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => DbContext.AttachRange(providedEntities));

            dbContextMock.Setup(m => m.Entry(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => DbContext.Entry(providedEntity));

            dbContextMock.Setup(m => m.Find<TEntity>(It.IsAny<object[]>())).Returns((object[] providedKeyValues) => DbContext.Find<TEntity>(providedKeyValues));
            dbContextMock.Setup(m => m.Find(typeof(TEntity), It.IsAny<object[]>()))
                .Returns((Type providedType, object[] providedKeyValues) => DbContext.Find(providedType, providedKeyValues));
            dbContextMock.Setup(m => m.FindAsync<TEntity>(It.IsAny<object[]>())).Returns((object[] providedKeyValues) => DbContext.FindAsync<TEntity>(providedKeyValues));
            dbContextMock.Setup(m => m.FindAsync<TEntity>(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns((object[] providedKeyValues, CancellationToken providedCancellationToken) => DbContext.FindAsync<TEntity>(providedKeyValues, providedCancellationToken));

            dbContextMock.Setup(m => m.Remove(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => DbContext.Remove(providedEntity));

            dbContextMock.Setup(m => m.Update(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => DbContext.Update(providedEntity));
        }

        private void SetUpReadOnlyDbSetFor<TEntity>(Mock<TDbContext> dbContextMock) where TEntity : class
        {
            var mockedReadOnlyDbSet = DbContext.Set<TEntity>().CreateMockedReadOnlyDbSet();

            var property = typeof(TDbContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof(DbSet<TEntity>));
            if (property != null)
            {
                var setExpression = ExpressionHelper.CreatePropertyExpression<TDbContext, DbSet<TEntity>>(property);
                dbContextMock.Setup(setExpression).Returns(mockedReadOnlyDbSet);

                dbContextMock.Setup(m => m.Set<TEntity>()).Returns(mockedReadOnlyDbSet);
            }
            else
            {
                Logger.LogDebug($"Could not find a DbContext property for type '{typeof(TEntity)}'");
            }
        }
    }
}