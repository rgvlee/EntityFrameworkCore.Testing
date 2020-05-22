#pragma warning disable EF1001 // Internal EF Core API usage.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>
    ///     Extensions for db contexts.
    /// </summary>
    public static partial class DbContextExtensions
    {
        private static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(DbContextExtensions));

        /// <summary>
        ///     Sets up ExecuteSqlInterpolated invocations to return a specified result.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="executeSqlInterpolatedResult">The integer to return when ExecuteSqlInterpolated is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlInterpolatedResult<TDbContext>(
            this TDbContext mockedDbContext, int executeSqlInterpolatedResult, Action<string, IEnumerable<object>> callback = null) where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            return mockedDbContext.AddExecuteSqlRawResult(string.Empty, new List<object>(), executeSqlInterpolatedResult, callback);
        }

        /// <summary>
        ///     Sets up ExecuteSqlInterpolated invocations containing a specified sql string to return a specified result.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlInterpolated sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="executeSqlInterpolatedResult">The integer to return when ExecuteSqlInterpolated is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlInterpolatedResult<TDbContext>(
            this TDbContext mockedDbContext, FormattableString sql, int executeSqlInterpolatedResult, Action<string, IEnumerable<object>> callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            return mockedDbContext.AddExecuteSqlRawResult(sql.Format, sql.GetArguments(), executeSqlInterpolatedResult, callback);
        }

        /// <summary>
        ///     Sets up ExecuteSqlInterpolated invocations containing a specified sql string to return a specified result.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlInterpolated sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The ExecuteSqlInterpolated parameters. Set up supports case insensitive partial parameter sequence matching.</param>
        /// <param name="executeSqlInterpolatedResult">The integer to return when ExecuteSqlInterpolated is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlInterpolatedResult<TDbContext>(
            this TDbContext mockedDbContext, string sql, IEnumerable<object> parameters, int executeSqlInterpolatedResult, Action<string, IEnumerable<object>> callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            return mockedDbContext.AddExecuteSqlRawResult(sql, parameters, executeSqlInterpolatedResult, callback);
        }

        /// <summary>
        ///     Sets up ExecuteSqlRaw invocations to return a specified result.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="executeSqlRawResult">The integer to return when ExecuteSqlRaw is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlRawResult<TDbContext>(this TDbContext mockedDbContext, int executeSqlRawResult, Action<string, IEnumerable<object>> callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            return mockedDbContext.AddExecuteSqlRawResult(string.Empty, new List<object>(), executeSqlRawResult, callback);
        }

        /// <summary>
        ///     Sets up ExecuteSqlRaw invocations containing a specified sql string and parameters to return a specified result.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlRaw sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="executeSqlRawResult">The integer to return when ExecuteSqlRaw is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlRawResult<TDbContext>(
            this TDbContext mockedDbContext, string sql, int executeSqlRawResult, Action<string, IEnumerable<object>> callback = null) where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            return mockedDbContext.AddExecuteSqlRawResult(sql, new List<object>(), executeSqlRawResult, callback);
        }

        /// <summary>
        ///     Sets up ExecuteSqlRaw invocations containing a specified sql string and parameters to return a specified result.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlRaw sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The ExecuteSqlRaw parameters. Set up supports case insensitive partial parameter sequence matching.</param>
        /// <param name="executeSqlRawResult">The integer to return when ExecuteSqlRaw is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlRawResult<TDbContext>(
            this TDbContext mockedDbContext, string sql, IEnumerable<object> parameters, int executeSqlRawResult, Action<string, IEnumerable<object>> callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));

            var relationalCommand = Substitute.For<IRelationalCommand>();
            relationalCommand.ExecuteNonQuery(Arg.Any<RelationalCommandParameterObject>()).Returns(callInfo => executeSqlRawResult);

            relationalCommand.ExecuteNonQueryAsync(Arg.Any<RelationalCommandParameterObject>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => Task.FromResult(executeSqlRawResult));

            var rawSqlCommand = Substitute.For<RawSqlCommand>(relationalCommand, new Dictionary<string, object>());
            rawSqlCommand.RelationalCommand.Returns(callInfo => relationalCommand);
            rawSqlCommand.ParameterValues.Returns(callInfo => new Dictionary<string, object>());

            var existingRawSqlCommandBuilder =
                (((IInfrastructure<IServiceProvider>) mockedDbContext).Instance.GetService(typeof(IDatabaseFacadeDependencies)) as IRelationalDatabaseFacadeDependencies)
                ?.RawSqlCommandBuilder;

            if (existingRawSqlCommandBuilder != null)
            {
                existingRawSqlCommandBuilder.Build(
                        Arg.Is<string>(s => s.Contains(sql, StringComparison.CurrentCultureIgnoreCase)),
                        Arg.Is<IEnumerable<object>>(p => ParameterMatchingHelper.DoInvocationParametersMatchSetUpParameters(parameters, p)))
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
            }
            else
            {
                var rawSqlCommandBuilder = Substitute.For<IRawSqlCommandBuilder>();
                rawSqlCommandBuilder.Build(Arg.Any<string>(), Arg.Any<IEnumerable<object>>())
                    .Throws(callInfo =>
                    {
                        Logger.LogDebug("Catch all exception invoked");
                        return new InvalidOperationException();
                    });

                rawSqlCommandBuilder.Build(
                        Arg.Is<string>(s => s.Contains(sql, StringComparison.CurrentCultureIgnoreCase)),
                        Arg.Is<IEnumerable<object>>(p => ParameterMatchingHelper.DoInvocationParametersMatchSetUpParameters(parameters, p)))
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

                var dependencies = Substitute.For<IRelationalDatabaseFacadeDependencies>();
                dependencies.ConcurrencyDetector.Returns(callInfo => Substitute.For<IConcurrencyDetector>());
                dependencies.CommandLogger.Returns(callInfo => Substitute.For<IDiagnosticsLogger<DbLoggerCategory.Database.Command>>());
                dependencies.RawSqlCommandBuilder.Returns(callInfo => rawSqlCommandBuilder);
                dependencies.RelationalConnection.Returns(callInfo => Substitute.For<IRelationalConnection>());

                var serviceProvider = Substitute.For<IServiceProvider>();
                serviceProvider.GetService(Arg.Is<Type>(t => t == typeof(IDatabaseFacadeDependencies))).Returns(callInfo => dependencies);

                ((IInfrastructure<IServiceProvider>) mockedDbContext).Instance.Returns(callInfo => serviceProvider);

                var databaseFacade = Substitute.For<DatabaseFacade>(mockedDbContext);
                mockedDbContext.Database.Returns(callInfo => databaseFacade);
            }

            return mockedDbContext;
        }
    }
}