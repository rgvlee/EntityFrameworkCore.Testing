#pragma warning disable EF1001 // Internal EF Core API usage.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    /// <summary>Extensions for the db context type.</summary>
    public static partial class DbContextExtensions
    {
        private static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(DbContextExtensions));

        /// <summary>Creates and sets up a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextToMock">The db context to mock/proxy.</param>
        /// <returns>A mocked db context.</returns>
        /// <remarks>dbContextToMock would typically be an in-memory database instance.</remarks>
        public static TDbContext CreateMockedDbContext<TDbContext>(this TDbContext dbContextToMock)
            where TDbContext : DbContext
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
            dbContextMock.Setup(m => m.ContextId).Returns(dbContextToMock.ContextId);
            dbContextMock.Setup(m => m.Database).Returns(dbContextToMock.Database);
            dbContextMock.Setup(m => m.Dispose()).Callback(dbContextToMock.Dispose);
            dbContextMock.Setup(m => m.DisposeAsync()).Callback(() => dbContextToMock.DisposeAsync());
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.EntityFinderFactory).Returns(((IDbContextDependencies) dbContextToMock).EntityFinderFactory);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.EntityGraphAttacher).Returns(((IDbContextDependencies) dbContextToMock).EntityGraphAttacher);
            dbContextMock.Setup(m => m.Entry(It.IsAny<object>())).Returns((object providedEntity) => dbContextToMock.Entry(providedEntity));

            dbContextMock.Setup(m => m.Find(It.IsAny<Type>(), It.IsAny<object[]>())).Returns((Type providedEntityType, object[] providedKeyValues) => dbContextToMock.Find(providedEntityType, providedKeyValues));
            dbContextMock.Setup(m => m.FindAsync(It.IsAny<Type>(), It.IsAny<object[]>())).Returns((Type providedEntityType, object[] providedKeyValues) => dbContextToMock.FindAsync(providedEntityType, providedKeyValues));
            dbContextMock.Setup(m => m.FindAsync(It.IsAny<Type>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns((Type providedEntityType, object[] providedKeyValues, CancellationToken providedCancellationToken) => dbContextToMock.FindAsync(providedEntityType, providedKeyValues, providedCancellationToken));

            dbContextMock.As<IDbSetCache>().Setup(m => m.GetOrAddSet(It.IsAny<IDbSetSource>(), It.IsAny<Type>())).Returns((IDbSetSource providedSource, Type providedType) => ((IDbSetCache) dbContextToMock).GetOrAddSet(providedSource, providedType));
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.InfrastructureLogger).Returns(((IDbContextDependencies) dbContextToMock).InfrastructureLogger);
            dbContextMock.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance).Returns(((IInfrastructure<IServiceProvider>) dbContextToMock).Instance);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.Model).Returns(((IDbContextDependencies) dbContextToMock).Model);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.QueryProvider).Returns(((IDbContextDependencies) dbContextToMock).QueryProvider);

            dbContextMock.Setup(m => m.Remove(It.IsAny<object>())).Returns((object providedEntity) => dbContextToMock.Remove(providedEntity));
            dbContextMock.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> providedEntities) => dbContextToMock.RemoveRange(providedEntities));
            dbContextMock.Setup(m => m.RemoveRange(It.IsAny<object[]>())).Callback((object[] providedEntities) => dbContextToMock.RemoveRange(providedEntities));

            dbContextMock.As<IDbContextPoolable>().Setup(m => m.ResetState()).Callback(((IDbContextPoolable) dbContextToMock).ResetState);
            dbContextMock.As<IDbContextPoolable>().Setup(m => m.ResetStateAsync(It.IsAny<CancellationToken>())).Callback((CancellationToken providedCancellationToken) => ((IDbContextPoolable) dbContextToMock).ResetStateAsync(providedCancellationToken));
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

            foreach (var entity in dbContextToMock.Model.GetEntityTypes().Where(x => x.FindPrimaryKey() != null))
            {
                typeof(DbContextExtensions)
                    .GetMethod(nameof(CreateAndAttachMockedDbSetTo), BindingFlags.NonPublic | BindingFlags.Static)
                    .MakeGenericMethod(typeof(TDbContext), entity.ClrType).Invoke(null, new object[] {dbContextMock, dbContextToMock});
            }

            foreach (var entity in dbContextToMock.Model.GetEntityTypes().Where(x => x.FindPrimaryKey() == null))
            {
                typeof(DbContextExtensions)
                    .GetMethod(nameof(CreateAndAttachMockedReadOnlyDbSetTo), BindingFlags.NonPublic | BindingFlags.Static)
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

            var mockedDbSet = dbContextToMock.Set<TEntity>().CreateMockedDbSet();

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

        /// <summary>Creates and attaches a mocked readonly db set to a db context mock.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbContextMock">The db context mock.</param>
        /// <param name="dbContextToMock">The db context to mock/proxy.</param>
        private static void CreateAndAttachMockedReadOnlyDbSetTo<TDbContext, TEntity>(this Mock<TDbContext> dbContextMock, TDbContext dbContextToMock)
            where TDbContext : DbContext
            where TEntity : class
        {
            EnsureArgument.IsNotNull(dbContextMock, nameof(dbContextMock));
            EnsureArgument.IsNotNull(dbContextToMock, nameof(dbContextToMock));

            var mockedReadOnlyDbSet = dbContextToMock.Set<TEntity>().CreateMockedReadOnlyDbSet();

            var property = typeof(TDbContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof(DbSet<TEntity>) || p.PropertyType == typeof(DbQuery<TEntity>));

            if (property != null)
            {
                var setExpression = ExpressionHelper.CreatePropertyExpression<TDbContext, DbSet<TEntity>>(property);
                dbContextMock.Setup(setExpression).Returns(mockedReadOnlyDbSet);
            }
            else
            {
                Logger.LogDebug($"Could not find a DbContext property for type '{typeof(TEntity)}'");
            }

            dbContextMock.Setup(m => m.Set<TEntity>()).Returns(mockedReadOnlyDbSet);
            dbContextMock.Setup(m => m.Query<TEntity>()).Returns((DbQuery<TEntity>) mockedReadOnlyDbSet);
        }

        /// <summary>Sets up ExecuteSqlInterpolated invocations to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="executeSqlInterpolatedResult">The integer to return when ExecuteSqlInterpolated is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlInterpolatedResult<TDbContext>(this TDbContext mockedDbContext, int executeSqlInterpolatedResult, Action callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));

            return mockedDbContext.AddExecuteSqlRawResult(string.Empty, new List<object>(), executeSqlInterpolatedResult, callback);
        }

        /// <summary>Sets up ExecuteSqlInterpolated invocations containing a specified sql string to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlInterpolated sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="executeSqlInterpolatedResult">The integer to return when ExecuteSqlInterpolated is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlInterpolatedResult<TDbContext>(this TDbContext mockedDbContext, FormattableString sql, int executeSqlInterpolatedResult, Action callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            EnsureArgument.IsNotNull(sql, nameof(sql));

            return mockedDbContext.AddExecuteSqlRawResult(sql.Format, sql.GetArguments(), executeSqlInterpolatedResult, callback);
        }

        /// <summary>Sets up ExecuteSqlInterpolated invocations containing a specified sql string to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlInterpolated sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The ExecuteSqlInterpolated parameters. Set up supports case insensitive partial parameter sequence matching.</param>
        /// <param name="executeSqlInterpolatedResult">The integer to return when ExecuteSqlInterpolated is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlInterpolatedResult<TDbContext>(this TDbContext mockedDbContext, string sql, IEnumerable<object> parameters, int executeSqlInterpolatedResult, Action callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));

            return mockedDbContext.AddExecuteSqlRawResult(sql, parameters, executeSqlInterpolatedResult, callback);
        }

        /// <summary>Sets up ExecuteSqlRaw invocations to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="executeSqlRawResult">The integer to return when ExecuteSqlRaw is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlRawResult<TDbContext>(this TDbContext mockedDbContext, int executeSqlRawResult, Action callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));

            return mockedDbContext.AddExecuteSqlRawResult(string.Empty, new List<object>(), executeSqlRawResult, callback);
        }

        /// <summary>Sets up ExecuteSqlRaw invocations containing a specified sql string and parameters to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlRaw sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="executeSqlRawResult">The integer to return when ExecuteSqlRaw is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlRawResult<TDbContext>(this TDbContext mockedDbContext, string sql, int executeSqlRawResult, Action callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            EnsureArgument.IsNotNull(sql, nameof(sql));

            return mockedDbContext.AddExecuteSqlRawResult(sql, new List<object>(), executeSqlRawResult, callback);
        }

        /// <summary>Sets up ExecuteSqlRaw invocations containing a specified sql string and parameters to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlRaw sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The ExecuteSqlRaw parameters. Set up supports case insensitive partial parameter sequence matching.</param>
        /// <param name="executeSqlRawResult">The integer to return when ExecuteSqlRaw is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlRawResult<TDbContext>(this TDbContext mockedDbContext, string sql, IEnumerable<object> parameters, int executeSqlRawResult, Action callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));

            var relationalCommandMock = new Mock<IRelationalCommand>();
            relationalCommandMock
                .Setup(m => m.ExecuteNonQuery(It.IsAny<RelationalCommandParameterObject>()))
                .Returns((RelationalCommandParameterObject providedRelationalCommandParameterObject) => executeSqlRawResult)
                .Callback(() => { callback?.Invoke(); });

            relationalCommandMock
                .Setup(m => m.ExecuteNonQueryAsync(It.IsAny<RelationalCommandParameterObject>(), It.IsAny<CancellationToken>()))
                .Returns((RelationalCommandParameterObject providedRelationalCommandParameterObject, CancellationToken providedCancellationToken) => Task.FromResult(executeSqlRawResult))
                .Callback(() => { callback?.Invoke(); });
            var relationalCommand = relationalCommandMock.Object;

            var rawSqlCommandMock = new Mock<RawSqlCommand>(relationalCommand, new Dictionary<string, object>());
            rawSqlCommandMock.Setup(m => m.RelationalCommand).Returns(relationalCommand);
            rawSqlCommandMock.Setup(m => m.ParameterValues).Returns(new Dictionary<string, object>());
            var rawSqlCommand = rawSqlCommandMock.Object;

            var rawSqlCommandBuilderMock = new Mock<IRawSqlCommandBuilder>();
            rawSqlCommandBuilderMock
                .Setup(m => m.Build(It.IsAny<string>(),It.IsAny<IEnumerable<object>>()))
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

            var dependenciesMock = new Mock<IRelationalDatabaseFacadeDependencies>();
            dependenciesMock.Setup(m => m.ConcurrencyDetector).Returns(Mock.Of<IConcurrencyDetector>());
            dependenciesMock.Setup(m => m.CommandLogger).Returns(Mock.Of<IDiagnosticsLogger<DbLoggerCategory.Database.Command>>());
            dependenciesMock.Setup(m => m.RawSqlCommandBuilder).Returns(rawSqlCommandBuilder);
            dependenciesMock.Setup(m => m.RelationalConnection).Returns(Mock.Of<IRelationalConnection>());
            var dependencies = dependenciesMock.Object;

            var databaseFacadeMock = new Mock<DatabaseFacade>(mockedDbContext);
            databaseFacadeMock.As<IDatabaseFacadeDependenciesAccessor>().Setup(m => m.Context).Returns(mockedDbContext);
            databaseFacadeMock.As<IDatabaseFacadeDependenciesAccessor>().Setup(m => m.Dependencies).Returns(dependencies);
            var databaseFacade = databaseFacadeMock.Object;

            Mock.Get(mockedDbContext).Setup(m => m.Database).Returns(databaseFacade);

            return mockedDbContext;
        }
    }
}