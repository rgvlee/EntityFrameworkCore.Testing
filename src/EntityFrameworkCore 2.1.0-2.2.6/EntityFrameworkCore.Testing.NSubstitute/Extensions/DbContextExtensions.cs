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
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
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

            var mockedDbContext = (TDbContext)
                Substitute.For(new[] {
                        typeof(TDbContext),
                        typeof(IDbContextDependencies),
                        typeof(IDbQueryCache),
                        typeof(IDbSetCache),
                        typeof(IInfrastructure<IServiceProvider>),
                        typeof(IDbContextPoolable)
                    },
                    new object[] { }
                );

            mockedDbContext.Add(Arg.Any<object>()).Returns(callInfo => dbContextToMock.Add(callInfo.Arg<object>()));
            mockedDbContext.AddAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbContextToMock.AddAsync(callInfo.Arg<object>(), callInfo.Arg<CancellationToken>()));
            mockedDbContext.When(x => x.AddRange(Arg.Any<object[]>())).Do(callInfo => dbContextToMock.AddRange(callInfo.Arg<object[]>()));
            mockedDbContext.When(x => x.AddRange(Arg.Any<IEnumerable<object>>())).Do(callInfo => dbContextToMock.AddRange(callInfo.Arg<IEnumerable<object>>()));
            mockedDbContext.AddRangeAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbContextToMock.AddRangeAsync(callInfo.Arg<object[]>(), callInfo.Arg<CancellationToken>()));
            mockedDbContext.AddRangeAsync(Arg.Any<IEnumerable<object>>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbContextToMock.AddRangeAsync(callInfo.Arg<IEnumerable<object>>(), callInfo.Arg<CancellationToken>()));

            mockedDbContext.Attach(Arg.Any<object>()).Returns(callInfo => dbContextToMock.Attach(callInfo.Arg<object>()));
            mockedDbContext.When(x => x.AttachRange(Arg.Any<object[]>())).Do(callInfo => dbContextToMock.AttachRange(callInfo.Arg<object[]>()));
            mockedDbContext.When(x => x.AttachRange(Arg.Any<IEnumerable<object>>())).Do(callInfo => dbContextToMock.AttachRange(callInfo.Arg<IEnumerable<object>>()));

            ((IDbContextDependencies) mockedDbContext).ChangeDetector.Returns(callInfo => ((IDbContextDependencies) dbContextToMock).ChangeDetector);
            mockedDbContext.ChangeTracker.Returns(callInfo => dbContextToMock.ChangeTracker);
            mockedDbContext.Database.Returns(callInfo => dbContextToMock.Database);
            mockedDbContext.When(x => x.Dispose()).Do(callInfo => dbContextToMock.Dispose());
            ((IDbContextDependencies) mockedDbContext).EntityFinderFactory.Returns(callInfo => ((IDbContextDependencies) dbContextToMock).EntityFinderFactory);
            ((IDbContextDependencies) mockedDbContext).EntityGraphAttacher.Returns(callInfo => ((IDbContextDependencies) dbContextToMock).EntityGraphAttacher);
            mockedDbContext.Entry(Arg.Any<object>()).Returns(callInfo => dbContextToMock.Entry(callInfo.Arg<object>()));

            mockedDbContext.FindAsync(Arg.Any<Type>(), Arg.Any<object[]>()).Returns(callInfo => dbContextToMock.FindAsync(callInfo.Arg<Type>(), callInfo.Arg<object[]>()));
            mockedDbContext.FindAsync(Arg.Any<Type>(), Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbContextToMock.FindAsync(callInfo.Arg<Type>(), callInfo.Arg<object[]>(), callInfo.Arg<CancellationToken>()));

            ((IDbQueryCache) mockedDbContext).GetOrAddQuery(Arg.Any<IDbQuerySource>(), Arg.Any<Type>()).Returns(callInfo => ((IDbQueryCache) dbContextToMock).GetOrAddQuery(callInfo.Arg<IDbQuerySource>(), callInfo.Arg<Type>()));
            ((IDbSetCache) mockedDbContext).GetOrAddSet(Arg.Any<IDbSetSource>(), Arg.Any<Type>()).Returns(callInfo => ((IDbSetCache) dbContextToMock).GetOrAddSet(callInfo.Arg<IDbSetSource>(), callInfo.Arg<Type>()));
            ((IDbContextDependencies) mockedDbContext).InfrastructureLogger.Returns(callInfo => ((IDbContextDependencies) dbContextToMock).InfrastructureLogger);
            ((IInfrastructure<IServiceProvider>) mockedDbContext).Instance.Returns(callInfo => ((IInfrastructure<IServiceProvider>) dbContextToMock).Instance);
            ((IDbContextDependencies) mockedDbContext).Model.Returns(callInfo => ((IDbContextDependencies) dbContextToMock).Model);
            ((IDbContextDependencies) mockedDbContext).QueryProvider.Returns(callInfo => ((IDbContextDependencies) dbContextToMock).QueryProvider);
            ((IDbContextDependencies) mockedDbContext).QuerySource.Returns(callInfo => ((IDbContextDependencies) dbContextToMock).QuerySource);

            mockedDbContext.Remove(Arg.Any<object>()).Returns(callInfo => dbContextToMock.Remove(callInfo.Arg<object>()));
            mockedDbContext.When(x => x.RemoveRange(Arg.Any<object[]>())).Do(callInfo => dbContextToMock.RemoveRange(callInfo.Arg<object[]>()));
            mockedDbContext.When(x => x.RemoveRange(Arg.Any<IEnumerable<object>>())).Do(callInfo => dbContextToMock.RemoveRange(callInfo.Arg<IEnumerable<object>>()));

            ((IDbContextPoolable) mockedDbContext).When(x => x.ResetState()).Do(callInfo => ((IDbContextPoolable) dbContextToMock).ResetState());
            ((IDbContextPoolable) mockedDbContext).When(x => x.Resurrect(Arg.Any<DbContextPoolConfigurationSnapshot>())).Do(callInfo => ((IDbContextPoolable) dbContextToMock).Resurrect(callInfo.Arg<DbContextPoolConfigurationSnapshot>()));

            mockedDbContext.SaveChanges().Returns(callInfo => dbContextToMock.SaveChanges());
            mockedDbContext.SaveChanges(Arg.Any<bool>()).Returns(callInfo => dbContextToMock.SaveChanges(callInfo.Arg<bool>()));
            mockedDbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(callInfo => dbContextToMock.SaveChangesAsync(callInfo.Arg<CancellationToken>()));
            mockedDbContext.SaveChangesAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbContextToMock.SaveChangesAsync(callInfo.Arg<bool>(), callInfo.Arg<CancellationToken>()));

            ((IDbContextPoolable) mockedDbContext).When(x => x.SetPool(Arg.Any<IDbContextPool>())).Do(callInfo => ((IDbContextPoolable) dbContextToMock).SetPool(callInfo.Arg<IDbContextPool>()));
            ((IDbContextDependencies) mockedDbContext).SetSource.Returns(callInfo => ((IDbContextDependencies) dbContextToMock).SetSource);
            ((IDbContextPoolable) mockedDbContext).SnapshotConfiguration().Returns(callInfo => ((IDbContextPoolable) dbContextToMock).SnapshotConfiguration());
            ((IDbContextDependencies) mockedDbContext).StateManager.Returns(callInfo => ((IDbContextDependencies) dbContextToMock).StateManager);

            mockedDbContext.Update(Arg.Any<object>()).Returns(callInfo => dbContextToMock.Update(callInfo.Arg<object>()));

            ((IDbContextDependencies) mockedDbContext).UpdateLogger.Returns(callInfo => ((IDbContextDependencies) dbContextToMock).UpdateLogger);

            mockedDbContext.When(x => x.UpdateRange(Arg.Any<object[]>())).Do(callInfo => dbContextToMock.UpdateRange(callInfo.Arg<object[]>()));
            mockedDbContext.When(x => x.UpdateRange(Arg.Any<IEnumerable<object>>())).Do(callInfo => dbContextToMock.UpdateRange(callInfo.Arg<IEnumerable<object>>()));

            foreach (var entity in dbContextToMock.Model.GetEntityTypes().Where(x => !x.IsQueryType))
            {
                typeof(DbContextExtensions)
                    .GetMethod(nameof(CreateAndAttachMockedDbSetTo), BindingFlags.NonPublic | BindingFlags.Static)
                    .MakeGenericMethod(typeof(TDbContext), entity.ClrType).Invoke(null, new object[] {mockedDbContext, dbContextToMock});
            }

            foreach (var entity in dbContextToMock.Model.GetEntityTypes().Where(x => x.IsQueryType))
            {
                typeof(DbContextExtensions)
                    .GetMethod(nameof(CreateAndAttachMockedDbQueryTo), BindingFlags.NonPublic | BindingFlags.Static)
                    .MakeGenericMethod(typeof(TDbContext), entity.ClrType).Invoke(null, new object[] {mockedDbContext, dbContextToMock});
            }

            return mockedDbContext;
        }

        /// <summary>Creates and attaches a mocked db set to a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="dbContextToMock">The db context to mock/proxy.</param>
        private static void CreateAndAttachMockedDbSetTo<TDbContext, TEntity>(this TDbContext mockedDbContext, TDbContext dbContextToMock)
            where TDbContext : DbContext
            where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            EnsureArgument.IsNotNull(dbContextToMock, nameof(dbContextToMock));

            var mockedDbSet = dbContextToMock.Set<TEntity>().CreateMockedDbSet();

            var property = typeof(TDbContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof(DbSet<TEntity>));

            if (property != null)
            {
                property.GetValue(mockedDbContext.Configure()).Returns(mockedDbSet);
            }
            else
            {
                Logger.LogDebug($"Could not find a DbContext property for type '{typeof(TEntity)}'");
            }

            mockedDbContext.Configure().Set<TEntity>().Returns(callInfo => mockedDbSet);

            mockedDbContext.Add(Arg.Any<TEntity>()).Returns(callInfo => dbContextToMock.Add(callInfo.Arg<TEntity>()));
            mockedDbContext.AddAsync(Arg.Any<TEntity>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbContextToMock.AddAsync(callInfo.Arg<TEntity>(), callInfo.Arg<CancellationToken>()));

            mockedDbContext.Attach(Arg.Any<TEntity>()).Returns(callInfo => dbContextToMock.Attach(callInfo.Arg<TEntity>()));
            mockedDbContext.When(x => x.AttachRange(Arg.Any<object[]>())).Do(callInfo => dbContextToMock.AttachRange(callInfo.Arg<object[]>()));
            mockedDbContext.When(x => x.AttachRange(Arg.Any<IEnumerable<object>>())).Do(callInfo => dbContextToMock.AttachRange(callInfo.Arg<IEnumerable<object>>()));

            mockedDbContext.Entry(Arg.Any<TEntity>()).Returns(callInfo => dbContextToMock.Entry(callInfo.Arg<TEntity>()));

            mockedDbContext.Find<TEntity>(Arg.Any<object[]>()).Returns(callInfo => dbContextToMock.Find<TEntity>(callInfo.Arg<object[]>()));
            mockedDbContext.Find(typeof(TEntity), Arg.Any<object[]>()).Returns(callInfo => dbContextToMock.Find(callInfo.Arg<Type>(), callInfo.Arg<object[]>()));
            mockedDbContext.FindAsync<TEntity>(Arg.Any<object[]>()).Returns(callInfo => dbContextToMock.FindAsync<TEntity>(callInfo.Arg<object[]>()));
            mockedDbContext.FindAsync<TEntity>(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbContextToMock.FindAsync<TEntity>(callInfo.Arg<object[]>(), callInfo.Arg<CancellationToken>()));

            mockedDbContext.Remove(Arg.Any<TEntity>()).Returns(callInfo => dbContextToMock.Remove(callInfo.Arg<TEntity>()));

            mockedDbContext.Update(Arg.Any<TEntity>()).Returns(callInfo => dbContextToMock.Update(callInfo.Arg<TEntity>()));
        }

        /// <summary>Creates and attaches a mocked db query to a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="dbContextToMock">The db context to mock/proxy.</param>
        private static void CreateAndAttachMockedDbQueryTo<TDbContext, TQuery>(this TDbContext mockedDbContext, TDbContext dbContextToMock)
            where TDbContext : DbContext
            where TQuery : class
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            EnsureArgument.IsNotNull(dbContextToMock, nameof(dbContextToMock));

            var mockedDbQuery = dbContextToMock.Query<TQuery>().CreateMockedDbQuery();

            var property = typeof(TDbContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof(DbQuery<TQuery>));

            if (property != null)
            {
                property.GetValue(mockedDbContext.Configure()).Returns(mockedDbQuery);
            }
            else
            {
                Logger.LogDebug($"Could not find a DbContext property for type '{typeof(TQuery)}'");
            }

            mockedDbContext.Configure().Query<TQuery>().Returns(callInfo => mockedDbQuery);
        }

        /// <summary>Sets up ExecuteSqlCommand invocations to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(this TDbContext mockedDbContext, int executeSqlCommandResult, Action<string, IEnumerable<object>> callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            return mockedDbContext.AddExecuteSqlCommandResult(string.Empty, new List<object>(), executeSqlCommandResult, callback);
        }

        /// <summary>Sets up ExecuteSqlCommand invocations containing a specified sql string to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlCommand sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(this TDbContext mockedDbContext, string sql, int executeSqlCommandResult, Action<string, IEnumerable<object>> callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            return mockedDbContext.AddExecuteSqlCommandResult(sql, new List<object>(), executeSqlCommandResult, callback);
        }

        /// <summary>Sets up ExecuteSqlCommand invocations containing a specified sql string and parameters to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlCommand sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The ExecuteSqlCommand parameters. Set up supports case insensitive partial parameter sequence matching.</param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(
            this TDbContext mockedDbContext, string sql, IEnumerable<object> parameters, int executeSqlCommandResult, Action<string, IEnumerable<object>> callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));

            var relationalCommand = Substitute.For<IRelationalCommand>();
            relationalCommand
                .ExecuteNonQuery(Arg.Any<IRelationalConnection>(), Arg.Any<IReadOnlyDictionary<string, object>>())
                .Returns(callInfo => executeSqlCommandResult);

            relationalCommand
                .ExecuteNonQueryAsync(Arg.Any<IRelationalConnection>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => Task.FromResult(executeSqlCommandResult));

            var rawSqlCommand = Substitute.For<RawSqlCommand>(relationalCommand, new Dictionary<string, object>());
            rawSqlCommand.RelationalCommand.Returns(callInfo => relationalCommand);
            rawSqlCommand.ParameterValues.Returns(callInfo => new Dictionary<string, object>());

            var rawSqlCommandBuilder = Substitute.For<IRawSqlCommandBuilder>();
            rawSqlCommandBuilder
                .Build(Arg.Any<string>(), Arg.Any<IEnumerable<object>>())
                .Throws(callInfo =>
                {
                    Logger.LogDebug("Catch all exception invoked");
                    return new InvalidOperationException();
                });

            rawSqlCommandBuilder
                .Build(
                    Arg.Is<string>(s => s.Contains(sql, StringComparison.CurrentCultureIgnoreCase)),
                    Arg.Is<IEnumerable<object>>(p => ParameterMatchingHelper.DoInvocationParametersMatchSetUpParameters(parameters, p))
                )
                .Returns(callInfo => rawSqlCommand)
                .AndDoes(callInfo =>
                {
                    var providedSql = callInfo.Arg<string>();
                    var providedParameters = callInfo.Arg<IEnumerable<object>>();

                    callback?.Invoke(providedSql, providedParameters);

                    var parts = new List<string>();
                    parts.Add($"Invocation sql: {providedSql}");
                    parts.Add("Invocation Parameters:");
                    parts.Add(ParameterMatchingHelper.StringifyParameters(providedParameters));
                    Logger.LogDebug(string.Join(Environment.NewLine, parts));
                });

            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(Arg.Is<Type>(t => t == typeof(IConcurrencyDetector))).Returns(callInfo => Substitute.For<IConcurrencyDetector>());
            serviceProvider.GetService(Arg.Is<Type>(t => t == typeof(IRawSqlCommandBuilder))).Returns(callInfo => rawSqlCommandBuilder);
            serviceProvider.GetService(Arg.Is<Type>(t => t == typeof(IRelationalConnection))).Returns(callInfo => Substitute.For<IRelationalConnection>());

            var databaseFacade = Substitute.For<DatabaseFacade>(mockedDbContext);
            ((IInfrastructure<IServiceProvider>) databaseFacade).Instance.Returns(callInfo => serviceProvider);

            mockedDbContext.Database.Returns(callInfo => databaseFacade);

            return mockedDbContext;
        }
    }
}