#pragma warning disable EF1001 // Internal EF Core API usage.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    /// <summary>
    ///     Extensions for db contexts.
    /// </summary>
    public static partial class DbContextExtensions
    {
        private static readonly ILogger Logger = LoggingHelper.CreateLogger(typeof(DbContextExtensions));

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

            var relationalCommandMock = new Mock<IRelationalCommand>();
            relationalCommandMock.Setup(m => m.ExecuteNonQuery(It.IsAny<RelationalCommandParameterObject>()))
                .Returns((RelationalCommandParameterObject providedRelationalCommandParameterObject) => executeSqlRawResult);

            relationalCommandMock.Setup(m => m.ExecuteNonQueryAsync(It.IsAny<RelationalCommandParameterObject>(), It.IsAny<CancellationToken>()))
                .Returns((RelationalCommandParameterObject providedRelationalCommandParameterObject, CancellationToken providedCancellationToken) =>
                    Task.FromResult(executeSqlRawResult));
            var relationalCommand = relationalCommandMock.Object;

            var rawSqlCommandMock = new Mock<RawSqlCommand>(relationalCommand, new Dictionary<string, object>());
            rawSqlCommandMock.Setup(m => m.RelationalCommand).Returns(() => relationalCommand);
            rawSqlCommandMock.Setup(m => m.ParameterValues).Returns(() => new Dictionary<string, object>());
            var rawSqlCommand = rawSqlCommandMock.Object;

            var existingRawSqlCommandBuilder =
                ((IRelationalDatabaseFacadeDependencies) ((IInfrastructure<IServiceProvider>) mockedDbContext).Instance.GetService(typeof(IDatabaseFacadeDependencies)))
                .RawSqlCommandBuilder;

            Mock.Get(existingRawSqlCommandBuilder)
                .Setup(m => m.Build(It.Is<string>(s => s.Contains(sql, StringComparison.OrdinalIgnoreCase)),
                    It.Is<IEnumerable<object>>(p => ParameterMatchingHelper.DoInvocationParametersMatchSetUpParameters(parameters, p))))
                .Returns((string providedSql, IEnumerable<object> providedParameters) => rawSqlCommand)
                .Callback((string providedSql, IEnumerable<object> providedParameters) =>
                {
                    callback?.Invoke(providedSql, providedParameters);

                    var parts = new List<string>();
                    parts.Add($"Invocation sql: {providedSql}");
                    parts.Add("Invocation Parameters:");
                    parts.Add(ParameterMatchingHelper.StringifyParameters(providedParameters));
                    Logger.LogDebug(string.Join(Environment.NewLine, parts));
                });

            return mockedDbContext;
        }
    }
}