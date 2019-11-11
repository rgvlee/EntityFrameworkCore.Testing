using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.Common.Extensions;
using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    /// <summary>Extensions for the db context type.</summary>
    public static class DbContextExtensions
    {
        private static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(DbContextExtensions));

        /// <summary>Creates and sets up a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextToMock">The db context to mock/proxy.</param>
        /// <returns>A mocked db context.</returns>
        /// <remarks>dbContextToMock would typically be an in-memory database instance.</remarks>
        public static TDbContext CreateMock<TDbContext>(this TDbContext dbContextToMock) where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(dbContextToMock, nameof(dbContextToMock));

            var dbContextMock = new Mock<TDbContext>();

            dbContextMock.Setup(m => m.Add(It.IsAny<object>())).Returns((object providedEntity) => dbContextToMock.Add(providedEntity));
            dbContextMock.Setup(m => m.AddAsync(It.IsAny<object>(), It.IsAny<CancellationToken>())).Returns((object providedEntity, CancellationToken providedCancellationToken) => dbContextToMock.AddAsync(providedEntity, providedCancellationToken));
            dbContextMock.Setup(m => m.AddRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => dbContextToMock.AddRange(providedEntities));
            dbContextMock.Setup(m => m.AddRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => dbContextToMock.AddRange(providedEntities));
            dbContextMock.Setup(m => m.AddRangeAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns((object[] providedEntities, CancellationToken providedCancellationToken) => dbContextToMock.AddRangeAsync(providedEntities, providedCancellationToken));
            dbContextMock.Setup(m => m.AddRangeAsync(It.IsAny<IEnumerable<object>>(), It.IsAny<CancellationToken>())).Returns((IEnumerable<object> providedEntities, CancellationToken providedCancellationToken) => dbContextToMock.AddRangeAsync(providedEntities, providedCancellationToken));

            dbContextMock.Setup(m => m.Attach(It.IsAny<object>())).Returns((object providedEntity) => dbContextToMock.Attach(providedEntity));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => dbContextToMock.AttachRange(providedEntities));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => dbContextToMock.AttachRange(providedEntities));

            dbContextMock.As<IDbContextDependencies>().Setup(m => m.ChangeDetector).Returns(((IDbContextDependencies) dbContextToMock).ChangeDetector);
            dbContextMock.Setup(m => m.ChangeTracker).Returns(dbContextToMock.ChangeTracker);
            dbContextMock.Setup(m => m.Database).Returns(dbContextToMock.Database);
            dbContextMock.Setup(m => m.Dispose()).Callback(dbContextToMock.Dispose);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.EntityFinderFactory).Returns(((IDbContextDependencies) dbContextToMock).EntityFinderFactory);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.EntityGraphAttacher).Returns(((IDbContextDependencies) dbContextToMock).EntityGraphAttacher);
            dbContextMock.Setup(m => m.Entry(It.IsAny<object>())).Returns((object providedEntity) => dbContextToMock.Entry(providedEntity));

            dbContextMock.Setup(m => m.FindAsync(It.IsAny<Type>(), It.IsAny<object[]>())).Returns((Type providedEntityType, object[] providedKeyValues) => dbContextToMock.FindAsync(providedEntityType, providedKeyValues));
            dbContextMock.Setup(m => m.FindAsync(It.IsAny<Type>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns((Type providedEntityType, object[] providedKeyValues, CancellationToken providedCancellationToken) => dbContextToMock.FindAsync(providedEntityType, providedKeyValues, providedCancellationToken));

            dbContextMock.As<IDbQueryCache>().Setup(m => m.GetOrAddQuery(It.IsAny<IDbQuerySource>(), It.IsAny<Type>())).Returns((IDbQuerySource providedSource, Type providedType) => ((IDbQueryCache) dbContextToMock).GetOrAddQuery(providedSource, providedType));
            dbContextMock.As<IDbSetCache>().Setup(m => m.GetOrAddSet(It.IsAny<IDbSetSource>(), It.IsAny<Type>())).Returns((IDbSetSource providedSource, Type providedType) => ((IDbSetCache) dbContextToMock).GetOrAddSet(providedSource, providedType));
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.InfrastructureLogger).Returns(((IDbContextDependencies) dbContextToMock).InfrastructureLogger);
            dbContextMock.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance).Returns(((IInfrastructure<IServiceProvider>) dbContextToMock).Instance);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.Model).Returns(((IDbContextDependencies) dbContextToMock).Model);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.QueryProvider).Returns(((IDbContextDependencies) dbContextToMock).QueryProvider);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.QuerySource).Returns(((IDbContextDependencies) dbContextToMock).QuerySource);

            dbContextMock.Setup(m => m.Remove(It.IsAny<object>())).Returns((object providedEntity) => dbContextToMock.Remove(providedEntity));
            dbContextMock.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => dbContextToMock.RemoveRange(providedEntities));
            dbContextMock.Setup(m => m.RemoveRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => dbContextToMock.RemoveRange(providedEntities));

            dbContextMock.As<IDbContextPoolable>().Setup(m => m.ResetState()).Callback(((IDbContextPoolable) dbContextToMock).ResetState);
            dbContextMock.As<IDbContextPoolable>().Setup(m => m.Resurrect(It.IsAny<DbContextPoolConfigurationSnapshot>())).Callback((DbContextPoolConfigurationSnapshot providedConfigurationSnapshot) => ((IDbContextPoolable) dbContextToMock).Resurrect(providedConfigurationSnapshot));

            dbContextMock.Setup(m => m.SaveChanges()).Returns(dbContextToMock.SaveChanges);
            dbContextMock.Setup(m => m.SaveChanges(It.IsAny<bool>())).Returns((bool providedAcceptAllChangesOnSuccess) => dbContextToMock.SaveChanges(providedAcceptAllChangesOnSuccess));
            dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken providedCancellationToken) => dbContextToMock.SaveChangesAsync(providedCancellationToken));
            dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>())).Returns((bool providedAcceptAllChangesOnSuccess, CancellationToken providedCancellationToken) => dbContextToMock.SaveChangesAsync(providedAcceptAllChangesOnSuccess, providedCancellationToken));

            dbContextMock.As<IDbContextPoolable>().Setup(m => m.SetPool(It.IsAny<IDbContextPool>())).Callback((IDbContextPool providedContextPool) => ((IDbContextPoolable) dbContextToMock).SetPool(providedContextPool));
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.SetSource).Returns(((IDbContextDependencies) dbContextToMock).SetSource);
            dbContextMock.As<IDbContextPoolable>().Setup(m => m.SnapshotConfiguration()).Returns(((IDbContextPoolable) dbContextToMock).SnapshotConfiguration());
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.StateManager).Returns(((IDbContextDependencies) dbContextToMock).StateManager);

            dbContextMock.Setup(m => m.Update(It.IsAny<object>())).Returns((object providedEntity) => dbContextToMock.Update(providedEntity));

            dbContextMock.As<IDbContextDependencies>().Setup(m => m.UpdateLogger).Returns(((IDbContextDependencies) dbContextToMock).UpdateLogger);

            dbContextMock.Setup(m => m.UpdateRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => dbContextToMock.UpdateRange(providedEntities));
            dbContextMock.Setup(m => m.UpdateRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => dbContextToMock.UpdateRange(providedEntities));

            foreach (var entity in dbContextToMock.Model.GetEntityTypes().Where(x => !x.IsQueryType))
            {
                typeof(DbContextExtensions)
                    .GetMethod(nameof(CreateAndAttachMockedDbSetTo), BindingFlags.NonPublic | BindingFlags.Static)
                    .MakeGenericMethod(typeof(TDbContext), entity.ClrType).Invoke(null, new object[] {dbContextMock, dbContextToMock});
            }

            foreach (var entity in dbContextToMock.Model.GetEntityTypes().Where(x => x.IsQueryType))
            {
                typeof(DbContextExtensions)
                    .GetMethod(nameof(CreateAndAttachMockedDbQueryTo), BindingFlags.NonPublic | BindingFlags.Static)
                    .MakeGenericMethod(typeof(TDbContext), entity.ClrType).Invoke(null, new object[] {dbContextMock, dbContextToMock});
            }

            return dbContextMock.Object;
        }

        /// <summary>Creates and attaches a mocked db set to a db context mock.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbContextMock">The db context mock.</param>
        /// <param name="dbContextToMock">The db context to mock/proxy.</param>
        private static void CreateAndAttachMockedDbSetTo<TDbContext, TEntity>(this Mock<TDbContext> dbContextMock, TDbContext dbContextToMock)
            where TDbContext : DbContext
            where TEntity : class
        {
            EnsureArgument.IsNotNull(dbContextMock, nameof(dbContextMock));
            EnsureArgument.IsNotNull(dbContextToMock, nameof(dbContextToMock));

            var mockedDbSet = dbContextToMock.Set<TEntity>().CreateMock();

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

            dbContextMock.Setup(m => m.Add(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => dbContextToMock.Add(providedEntity));
            dbContextMock.Setup(m => m.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>())).Returns((TEntity providedEntity, CancellationToken providedCancellationToken) => dbContextToMock.AddAsync(providedEntity, providedCancellationToken));

            dbContextMock.Setup(m => m.Attach(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => dbContextToMock.Attach(providedEntity));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => dbContextToMock.AttachRange(providedEntities));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => dbContextToMock.AttachRange(providedEntities));

            dbContextMock.Setup(m => m.Entry(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => dbContextToMock.Entry(providedEntity));

            dbContextMock.Setup(m => m.Find<TEntity>(It.IsAny<object[]>())).Returns((object[] providedKeyValues) => dbContextToMock.Find<TEntity>(providedKeyValues));
            dbContextMock.Setup(m => m.Find(typeof(TEntity), It.IsAny<object[]>())).Returns((Type providedType, object[] providedKeyValues) => dbContextToMock.Find(providedType, providedKeyValues));
            dbContextMock.Setup(m => m.FindAsync<TEntity>(It.IsAny<object[]>())).Returns((object[] providedKeyValues) => dbContextToMock.FindAsync<TEntity>(providedKeyValues));
            dbContextMock.Setup(m => m.FindAsync<TEntity>(It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns((object[] providedKeyValues, CancellationToken providedCancellationToken) => dbContextToMock.FindAsync<TEntity>(providedKeyValues, providedCancellationToken));

            dbContextMock.Setup(m => m.Remove(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => dbContextToMock.Remove(providedEntity));

            dbContextMock.Setup(m => m.Update(It.IsAny<TEntity>())).Returns((TEntity providedEntity) => dbContextToMock.Update(providedEntity));
        }

        /// <summary>Creates and attaches a mocked db query to a db context mock.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbContextMock">The db context mock.</param>
        /// <param name="dbContextToMock">The db context to mock/proxy.</param>
        private static void CreateAndAttachMockedDbQueryTo<TDbContext, TQuery>(this Mock<TDbContext> dbContextMock, TDbContext dbContextToMock)
            where TDbContext : DbContext
            where TQuery : class
        {
            EnsureArgument.IsNotNull(dbContextMock, nameof(dbContextMock));
            EnsureArgument.IsNotNull(dbContextToMock, nameof(dbContextToMock));

            var mockedDbQuery = dbContextToMock.Query<TQuery>().CreateMock();

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

        /// <summary>Sets up ExecuteSqlCommand invocations to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(this TDbContext mockedDbContext, int executeSqlCommandResult)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));

            return mockedDbContext.AddExecuteSqlCommandResult(string.Empty, new List<object>(), executeSqlCommandResult);
        }

        /// <summary>Sets up ExecuteSqlCommand invocations containing a specified sql string to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlCommand sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(this TDbContext mockedDbContext, string sql, int executeSqlCommandResult)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            EnsureArgument.IsNotNull(sql, nameof(sql));

            return mockedDbContext.AddExecuteSqlCommandResult(sql, new List<object>(), executeSqlCommandResult);
        }

        /// <summary>Sets up ExecuteSqlCommand invocations containing a specified sql string and parameters to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlCommand sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The ExecuteSqlCommand parameters. Set up supports case insensitive partial parameter sequence matching.</param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(this TDbContext mockedDbContext, string sql, IEnumerable<object> parameters, int executeSqlCommandResult)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));

            var relationalCommandMock = new Mock<IRelationalCommand>();
            relationalCommandMock
                .Setup(m => m.ExecuteNonQuery(It.IsAny<IRelationalConnection>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
                .Returns((IRelationalConnection providedConnection, IReadOnlyDictionary<string, object> providedParameterValues) => executeSqlCommandResult);
            relationalCommandMock
                .Setup(m => m.ExecuteNonQueryAsync(It.IsAny<IRelationalConnection>(), It.IsAny<IReadOnlyDictionary<string, object>>(), It.IsAny<CancellationToken>()))
                .Returns((IRelationalConnection providedConnection, IReadOnlyDictionary<string, object> providedParameterValues, CancellationToken providedCancellationToken) => Task.FromResult(executeSqlCommandResult));
            var relationalCommand = relationalCommandMock.Object;

            var rawSqlCommandMock = new Mock<RawSqlCommand>(MockBehavior.Strict, relationalCommand, new Dictionary<string, object>());
            rawSqlCommandMock.Setup(m => m.RelationalCommand).Returns(relationalCommand);
            rawSqlCommandMock.Setup(m => m.ParameterValues).Returns(new Dictionary<string, object>());
            var rawSqlCommand = rawSqlCommandMock.Object;

            var rawSqlCommandBuilderMock = new Mock<IRawSqlCommandBuilder>();

            rawSqlCommandBuilderMock.Setup(m =>
                    m.Build(
                        It.IsAny<string>(),
                        It.IsAny<IEnumerable<object>>())
                )
                .Callback((string providedSql, IEnumerable<object> providedParameters) => Logger.LogDebug("Catch all exception invoked"))
                .Throws<InvalidOperationException>();

            rawSqlCommandBuilderMock.Setup(m =>
                    m.Build(
                        It.Is<string>(s => s.Contains(sql, StringComparison.CurrentCultureIgnoreCase)),
                        It.Is<IEnumerable<object>>(p => ParameterMatchingHelper.DoInvocationParametersMatchSetUpParameters(parameters, p))
                    )
                )
                .Returns((string providedSql, IEnumerable<object> providedParameters) => rawSqlCommand)
                .Callback((string providedSql, IEnumerable<object> providedParameters) =>
                {
                    var parts = new List<string>();
                    parts.Add($"Invocation sql: {providedSql}");
                    parts.Add("Invocation Parameters:");
                    parts.Add(ParameterMatchingHelper.StringifyParameters(providedParameters));
                    Logger.LogDebug(string.Join(Environment.NewLine, parts));
                });
            var rawSqlCommandBuilder = rawSqlCommandBuilderMock.Object;

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IConcurrencyDetector)))).Returns((Type providedType) => Mock.Of<IConcurrencyDetector>());
            serviceProviderMock.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IRawSqlCommandBuilder)))).Returns((Type providedType) => rawSqlCommandBuilder);
            serviceProviderMock.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IRelationalConnection)))).Returns((Type providedType) => Mock.Of<IRelationalConnection>());
            var serviceProvider = serviceProviderMock.Object;

            var databaseFacadeMock = new Mock<DatabaseFacade>(MockBehavior.Strict, mockedDbContext);
            databaseFacadeMock.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance).Returns(serviceProvider);
            var databaseFacade = databaseFacadeMock.Object;

            Mock.Get(mockedDbContext).Setup(m => m.Database).Returns(databaseFacade);

            return mockedDbContext;
        }
    }
}