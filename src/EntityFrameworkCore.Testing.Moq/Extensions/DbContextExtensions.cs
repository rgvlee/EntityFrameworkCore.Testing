using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
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
    /// <summary>
    ///     Extensions for the db context type.
    /// </summary>
    public static class DbContextExtensions
    {
        private static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(DbContextExtensions));

        /// <summary>
        ///     Creates and sets up a mocked db context.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextToMock">The db context to mock/proxy.</param>
        /// <returns>A mocked db context.</returns>
        /// <remarks>dbContextToMock would typically be an in-memory database instance.</remarks>
        public static TDbContext CreateMock<TDbContext>(this TDbContext dbContextToMock) where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(dbContextToMock, nameof(dbContextToMock));

            var dbContextMock = new Mock<TDbContext>();
            dbContextMock.SetUp(dbContextToMock);
            return dbContextMock.Object;
        }

        /// <summary>
        ///     Sets up a db context mock.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextMock">The db context mock.</param>
        /// <param name="dbContextToMock">The db context to mock/proxy.</param>
        private static void SetUp<TDbContext>(this Mock<TDbContext> dbContextMock, TDbContext dbContextToMock) where TDbContext : DbContext
        {
            dbContextMock.Setup(m => m.Add(It.IsAny<object>())).Returns((object entity) => dbContextToMock.Add(entity));
            dbContextMock.Setup(m => m.AddAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns((object entity, CancellationToken cancellationToken) => dbContextToMock.AddAsync(entity, cancellationToken));
            dbContextMock.Setup(m => m.AddRange(It.IsAny<object[]>())).Callback((object[] entities) => dbContextToMock.AddRange(entities));
            dbContextMock.Setup(m => m.AddRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> entities) => dbContextToMock.AddRange(entities));
            dbContextMock.Setup(m => m.AddRangeAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns((object[] entities, CancellationToken cancellationToken) => dbContextToMock.AddRangeAsync(entities, cancellationToken));
            dbContextMock.Setup(m => m.AddRangeAsync(It.IsAny<IEnumerable<object>>(), It.IsAny<CancellationToken>()))
                .Returns((IEnumerable<object> entities, CancellationToken cancellationToken) => dbContextToMock.AddRangeAsync(entities, cancellationToken));
            dbContextMock.Setup(m => m.Attach(It.IsAny<object>())).Returns((object entity) => dbContextToMock.Attach(entity));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<object[]>())).Callback((object[] entities) => dbContextToMock.AttachRange(entities));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> entities) => dbContextToMock.AttachRange(entities));
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.ChangeDetector).Returns(((IDbContextDependencies) dbContextToMock).ChangeDetector);
            dbContextMock.Setup(m => m.ChangeTracker).Returns(dbContextToMock.ChangeTracker);
            dbContextMock.Setup(m => m.Database).Returns(dbContextToMock.Database);
            dbContextMock.Setup(m => m.Dispose()).Callback(dbContextToMock.Dispose);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.EntityFinderFactory).Returns(((IDbContextDependencies) dbContextToMock).EntityFinderFactory);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.EntityGraphAttacher).Returns(((IDbContextDependencies) dbContextToMock).EntityGraphAttacher);
            dbContextMock.Setup(m => m.Entry(It.IsAny<object>())).Returns((object entity) => dbContextToMock.Entry(entity));
            dbContextMock.Setup(m => m.FindAsync(It.IsAny<Type>(), It.IsAny<object[]>())).Returns((Type entityType, object[] keyValues) => dbContextToMock.FindAsync(entityType, keyValues));
            dbContextMock.Setup(m => m.FindAsync(It.IsAny<Type>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns((Type entityType, object[] keyValues, CancellationToken cancellationToken) => dbContextToMock.FindAsync(entityType, keyValues, cancellationToken));
            dbContextMock.As<IDbQueryCache>().Setup(m => m.GetOrAddQuery(It.IsAny<IDbQuerySource>(), It.IsAny<Type>()))
                .Returns((IDbQuerySource source, Type type) => ((IDbQueryCache) dbContextToMock).GetOrAddQuery(source, type));
            dbContextMock.As<IDbSetCache>().Setup(m => m.GetOrAddSet(It.IsAny<IDbSetSource>(), It.IsAny<Type>()))
                .Returns((IDbSetSource source, Type type) => ((IDbSetCache) dbContextToMock).GetOrAddSet(source, type));
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.InfrastructureLogger).Returns(((IDbContextDependencies) dbContextToMock).InfrastructureLogger);
            dbContextMock.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance).Returns(((IInfrastructure<IServiceProvider>) dbContextToMock).Instance);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.Model).Returns(((IDbContextDependencies) dbContextToMock).Model);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.QueryProvider).Returns(((IDbContextDependencies) dbContextToMock).QueryProvider);
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.QuerySource).Returns(((IDbContextDependencies) dbContextToMock).QuerySource);
            dbContextMock.Setup(m => m.Remove(It.IsAny<object>())).Returns((object entity) => dbContextToMock.Remove(entity));
            dbContextMock.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> entities) => dbContextToMock.RemoveRange(entities));
            dbContextMock.Setup(m => m.RemoveRange(It.IsAny<object[]>())).Callback((object[] entities) => dbContextToMock.RemoveRange(entities));
            dbContextMock.As<IDbContextPoolable>().Setup(m => m.ResetState()).Callback(() => ((IDbContextPoolable) dbContextToMock).ResetState());
            dbContextMock.As<IDbContextPoolable>().Setup(m => m.Resurrect(It.IsAny<DbContextPoolConfigurationSnapshot>())).Callback(
                (DbContextPoolConfigurationSnapshot configurationSnapshot) => ((IDbContextPoolable) dbContextToMock).Resurrect(configurationSnapshot));
            dbContextMock.Setup(m => m.SaveChanges()).Returns(dbContextToMock.SaveChanges);
            dbContextMock.Setup(m => m.SaveChanges(It.IsAny<bool>())).Returns((bool acceptAllChangesOnSuccess) => dbContextToMock.SaveChanges(acceptAllChangesOnSuccess));
            dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns((CancellationToken cancellationToken) => dbContextToMock.SaveChangesAsync(cancellationToken));
            dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns((bool acceptAllChangesOnSuccess, CancellationToken cancellationToken) => dbContextToMock.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken));
            dbContextMock.As<IDbContextPoolable>().Setup(m => m.SetPool(It.IsAny<IDbContextPool>())).Callback((IDbContextPool contextPool) => ((IDbContextPoolable) dbContextToMock).SetPool(contextPool));
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.SetSource).Returns(((IDbContextDependencies) dbContextToMock).SetSource);
            dbContextMock.As<IDbContextPoolable>().Setup(m => m.SnapshotConfiguration()).Returns(((IDbContextPoolable) dbContextToMock).SnapshotConfiguration());
            dbContextMock.As<IDbContextDependencies>().Setup(m => m.StateManager).Returns(((IDbContextDependencies) dbContextToMock).StateManager);
            dbContextMock.Setup(m => m.Update(It.IsAny<object>())).Returns((object entity) => dbContextToMock.Update(entity));

            dbContextMock.As<IDbContextDependencies>().Setup(m => m.UpdateLogger).Returns(((IDbContextDependencies) dbContextToMock).UpdateLogger);

            dbContextMock.Setup(m => m.UpdateRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> entities) => dbContextToMock.UpdateRange(entities));
            dbContextMock.Setup(m => m.UpdateRange(It.IsAny<object[]>())).Callback((object[] entities) => dbContextToMock.UpdateRange(entities));

            foreach (var entity in dbContextToMock.Model.GetEntityTypes().Where(x => !x.IsQueryType))
            {
                typeof(DbContextExtensions)
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                    .Single(mi => mi.Name.Equals(nameof(SetUp)) && mi.IsGenericMethod && mi.GetGenericArguments().ToList().Count.Equals(2))
                    .MakeGenericMethod(typeof(TDbContext), entity.ClrType).Invoke(null, new object[] {dbContextMock, dbContextToMock});
                typeof(DbContextExtensions)
                    .GetMethod(nameof(CreateAndAttachMockedDbSetTo), BindingFlags.NonPublic | BindingFlags.Static)
                    .MakeGenericMethod(typeof(TDbContext), entity.ClrType).Invoke(null, new object[] {dbContextMock, dbContextToMock});
            }

            foreach (var entity in dbContextToMock.Model.GetEntityTypes().Where(x => x.IsQueryType))
                typeof(DbContextExtensions)
                    .GetMethod(nameof(CreateAndAttachMockedDbQueryTo), BindingFlags.NonPublic | BindingFlags.Static)
                    .MakeGenericMethod(typeof(TDbContext), entity.ClrType).Invoke(null, new object[] {dbContextMock, dbContextToMock});
        }

        /// <summary>
        ///     Sets up a db context mock.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbContextMock">The db context mock.</param>
        /// <param name="dbContextToMock">The db context to mock/proxy.</param>
        private static void SetUp<TDbContext, TEntity>(this Mock<TDbContext> dbContextMock, TDbContext dbContextToMock)
            where TDbContext : DbContext
            where TEntity : class
        {
            EnsureArgument.IsNotNull(dbContextMock, nameof(dbContextMock));
            EnsureArgument.IsNotNull(dbContextToMock, nameof(dbContextToMock));

            dbContextMock.Setup(m => m.Add(It.IsAny<TEntity>())).Returns((TEntity entity) => dbContextToMock.Add(entity));
            dbContextMock.Setup(m => m.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>())).Returns((TEntity entity, CancellationToken cancellationToken) => dbContextToMock.AddAsync(entity, cancellationToken));
            dbContextMock.Setup(m => m.Attach(It.IsAny<TEntity>())).Returns((TEntity entity) => dbContextToMock.Attach(entity));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<object[]>())).Callback((object[] entities) => dbContextToMock.AttachRange(entities));
            dbContextMock.Setup(m => m.AttachRange(It.IsAny<IEnumerable<object>>())).Callback((IEnumerable<object> entities) => dbContextToMock.AttachRange(entities));
            dbContextMock.Setup(m => m.Entry(It.IsAny<TEntity>())).Returns((TEntity entity) => dbContextToMock.Entry(entity));
            dbContextMock.Setup(m => m.Find<TEntity>(It.IsAny<object[]>())).Returns((object[] keyValues) => dbContextToMock.Find<TEntity>(keyValues));
            dbContextMock.Setup(m => m.Find(typeof(TEntity), It.IsAny<object[]>())).Returns((Type type, object[] keyValues) => dbContextToMock.Find(type, keyValues));
            dbContextMock.Setup(m => m.FindAsync<TEntity>(It.IsAny<object[]>())).Returns((object[] keyValues) => dbContextToMock.FindAsync<TEntity>(keyValues));
            dbContextMock.Setup(m => m.FindAsync<TEntity>(It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns((object[] keyValues, CancellationToken cancellationToken) => dbContextToMock.FindAsync<TEntity>(keyValues, cancellationToken));
            dbContextMock.Setup(m => m.Remove(It.IsAny<TEntity>())).Returns((TEntity entity) => dbContextToMock.Remove(entity));
            dbContextMock.Setup(m => m.Update(It.IsAny<TEntity>())).Returns((TEntity entity) => dbContextToMock.Update(entity));
        }

        /// <summary>
        ///     Creates and attaches a mocked db set to a db context mock.
        /// </summary>
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

            dbContextMock.AttachMockedDbSet(dbContextToMock.Set<TEntity>().CreateMock());
        }

        /// <summary>
        ///     Creates and attaches a mocked db query to a db context mock.
        /// </summary>
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

            dbContextMock.AttachMockedDbQuery(dbContextToMock.Query<TQuery>().CreateMock());
        }

        /// <summary>
        ///     Attaches a mocked db set to a db context mock.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <typeparam name="TEntity">The db set entity type.</typeparam>
        /// <param name="dbContextMock">The db context mock.</param>
        /// <param name="mockedDbSet">The mocked db set.</param>
        /// <returns>The db context mock.</returns>
        public static Mock<TDbContext> AttachMockedDbSet<TDbContext, TEntity>(this Mock<TDbContext> dbContextMock, DbSet<TEntity> mockedDbSet)
            where TDbContext : DbContext
            where TEntity : class
        {
            EnsureArgument.IsNotNull(dbContextMock, nameof(dbContextMock));
            EnsureArgument.IsNotNull(mockedDbSet, nameof(mockedDbSet));

            var property = typeof(TDbContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof(DbSet<TEntity>));

            if (property != null)
            {
                var expression = ExpressionHelper.CreatePropertyExpression<TDbContext, DbSet<TEntity>>(property);
                dbContextMock.Setup(expression).Returns(() => mockedDbSet);
            }
            else
            {
                Logger.LogDebug($"Could not find a DbContext DbSet property for type '{typeof(TEntity)}'");
            }

            dbContextMock.Setup(m => m.Set<TEntity>()).Returns(() => mockedDbSet);

            return dbContextMock;
        }

        /// <summary>
        ///     Attaches a mocked db query to a db context mock.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <typeparam name="TQuery">The db query type.</typeparam>
        /// <param name="dbContextMock">The db context mock.</param>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        /// <returns>The db context mock.</returns>
        public static Mock<TDbContext> AttachMockedDbQuery<TDbContext, TQuery>(this Mock<TDbContext> dbContextMock, DbQuery<TQuery> mockedDbQuery)
            where TDbContext : DbContext
            where TQuery : class
        {
            EnsureArgument.IsNotNull(dbContextMock, nameof(dbContextMock));
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));

            var property = typeof(TDbContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof(DbQuery<TQuery>));

            if (property != null)
            {
                var expression = ExpressionHelper.CreatePropertyExpression<TDbContext, DbQuery<TQuery>>(property);
                dbContextMock.Setup(expression).Returns(() => mockedDbQuery);
            }
            else
            {
                Logger.LogDebug($"Could not find a DbContext DbQuery property for type '{typeof(TQuery)}'");
            }

            dbContextMock.Setup(m => m.Query<TQuery>()).Returns(() => mockedDbQuery);

            return dbContextMock;
        }

        /// <summary>
        ///     Sets up ExecuteSqlCommand invocations to return a specified result.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(this TDbContext mockedDbContext, int executeSqlCommandResult)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));

            return mockedDbContext.AddExecuteSqlCommandResult(string.Empty, executeSqlCommandResult);
        }

        /// <summary>
        ///     Sets up ExecuteSqlCommand invocations containing a specified sql string to return a specified result.
        /// </summary>
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

            return mockedDbContext.AddExecuteSqlCommandResult(sql, new List<SqlParameter>(), executeSqlCommandResult);
        }

        /// <summary>
        ///     Sets up ExecuteSqlCommand invocations containing a specified sql string and sql parameters to return a specified
        ///     result.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlCommand sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">
        ///     The ExecuteSqlCommand sql parameters. Set up supports case insensitive partial sql parameter
        ///     sequence matching.
        /// </param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(this TDbContext mockedDbContext, string sql, IEnumerable<SqlParameter> parameters, int executeSqlCommandResult)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            EnsureArgument.IsNotNull(sql, nameof(sql));

            Mock.Get(mockedDbContext).SetUpExecuteSqlCommand(sql, parameters, executeSqlCommandResult);
            return mockedDbContext;
        }

        /// <summary>
        ///     Sets up ExecuteSqlCommand invocations containing a specified sql string and sql parameters to return a specified
        ///     result.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextMock">The db context mock.</param>
        /// <param name="sql">The ExecuteSqlCommand sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">
        ///     The ExecuteSqlCommand sql parameters. Set up supports case insensitive partial sql parameter
        ///     sequence matching.
        /// </param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <returns>The db context mock.</returns>
        internal static Mock<TDbContext> SetUpExecuteSqlCommand<TDbContext>(this Mock<TDbContext> dbContextMock, string sql, IEnumerable<SqlParameter> parameters, int executeSqlCommandResult)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(dbContextMock, nameof(dbContextMock));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));

            //ExecuteSqlCommand creates a RawSqlCommand then ExecuteNonQuery is executed on the relational command property.
            //We need to:
            //1) Mock the relational command ExecuteNonQuery method
            //2) Mock the RawSqlCommand (doesn't implement any interfaces so we have to use a the concrete class which requires a constructor to be specified)
            //3) Mock the IRawSqlCommandBuilder build method to return our RawSqlCommand
            //4) Mock multiple the database facade GetService methods to avoid the 'Relational-specific methods can only be used when the context is using a relational database provider.' exception.

            var relationalCommand = new Mock<IRelationalCommand>();
            relationalCommand.Setup(m => m.ExecuteNonQuery(It.IsAny<IRelationalConnection>(), It.IsAny<IReadOnlyDictionary<string, object>>())).Returns(() => executeSqlCommandResult);
            relationalCommand.Setup(m => m.ExecuteNonQueryAsync(It.IsAny<IRelationalConnection>(), It.IsAny<IReadOnlyDictionary<string, object>>(), It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(executeSqlCommandResult));

            var rawSqlCommand = new Mock<RawSqlCommand>(MockBehavior.Strict, relationalCommand.Object, new Dictionary<string, object>());
            rawSqlCommand.Setup(m => m.RelationalCommand).Returns(() => relationalCommand.Object);
            rawSqlCommand.Setup(m => m.ParameterValues).Returns(new Dictionary<string, object>());

            var rawSqlCommandBuilder = new Mock<IRawSqlCommandBuilder>();
            rawSqlCommandBuilder.Setup(m => m.Build(It.Is<string>(s => s.Contains(sql, StringComparison.CurrentCultureIgnoreCase)), It.Is<IEnumerable<object>>(
                    p => !parameters.Except(p.Select(sp => (SqlParameter) sp), new SqlParameterParameterNameAndValueEqualityComparer()).Any()
                )))
                .Returns(rawSqlCommand.Object)
                .Callback((string callbackSql, IEnumerable<object> callbackParameters) =>
                {
                    var parts = new List<string>();
                    parts.Add($"{callbackSql.GetType().Name} sql: {callbackSql}");

                    parts.Add("Parameters:");
                    foreach (var sqlParameter in callbackParameters.Select(sp => (SqlParameter) sp))
                    {
                        var sb2 = new StringBuilder();
                        sb2.Append(sqlParameter.ParameterName);
                        sb2.Append(": ");
                        if (sqlParameter.Value == null)
                            sb2.Append("null");
                        else
                            sb2.Append(sqlParameter.Value);
                        parts.Add(sb2.ToString());
                    }

                    Logger.LogInformation(string.Join(Environment.NewLine, parts));
                });

            var databaseFacade = new Mock<DatabaseFacade>(MockBehavior.Strict, new Mock<TDbContext>().Object);
            databaseFacade.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance.GetService(It.Is<Type>(t => t == typeof(IConcurrencyDetector)))).Returns(new Mock<IConcurrencyDetector>().Object);
            databaseFacade.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance.GetService(It.Is<Type>(t => t == typeof(IRawSqlCommandBuilder)))).Returns(rawSqlCommandBuilder.Object);
            databaseFacade.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance.GetService(It.Is<Type>(t => t == typeof(IRelationalConnection)))).Returns(new Mock<IRelationalConnection>().Object);

            dbContextMock.Setup(m => m.Database).Returns(databaseFacade.Object);

            return dbContextMock;
        }
    }
}