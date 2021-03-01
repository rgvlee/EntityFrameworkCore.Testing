using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using rgvlee.Core.Common.Extensions;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    /// <summary>
    ///     Extensions for db contexts.
    /// </summary>
    public static class DbContextExtensions
    {
        private static readonly ILogger Logger = LoggingHelper.CreateLogger(typeof(DbContextExtensions));

        /// <summary>
        ///     Sets up ExecuteSqlCommand invocations to return a specified result.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(
            this TDbContext mockedDbContext, int executeSqlCommandResult, Action<string, IEnumerable<object>> callback = null) where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            return mockedDbContext.AddExecuteSqlCommandResult(string.Empty, new List<object>(), executeSqlCommandResult, callback);
        }

        /// <summary>
        ///     Sets up ExecuteSqlCommand invocations containing a specified sql string to return a specified result.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlCommand sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(
            this TDbContext mockedDbContext, RawSqlString sql, int executeSqlCommandResult, Action<string, IEnumerable<object>> callback = null) where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            return mockedDbContext.AddExecuteSqlCommandResult(sql.Format, new List<object>(), executeSqlCommandResult, callback);
        }

        /// <summary>
        ///     Sets up ExecuteSqlCommand invocations containing a specified sql string to return a specified result.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlCommand sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(
            this TDbContext mockedDbContext, FormattableString sql, int executeSqlCommandResult, Action<string, IEnumerable<object>> callback = null) where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            return mockedDbContext.AddExecuteSqlCommandResult(sql.Format, sql.GetArguments(), executeSqlCommandResult, callback);
        }

        /// <summary>
        ///     Sets up ExecuteSqlCommand invocations containing a specified sql string and parameters to return a specified result.
        /// </summary>
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

            var relationalCommandMock = new Mock<IRelationalCommand>();
            relationalCommandMock.Setup(m => m.ExecuteNonQuery(It.IsAny<IRelationalConnection>(), It.IsAny<IReadOnlyDictionary<string, object>>()))
                .Returns((IRelationalConnection providedConnection, IReadOnlyDictionary<string, object> providedParameterValues) => executeSqlCommandResult);

            relationalCommandMock
                .Setup(m => m.ExecuteNonQueryAsync(It.IsAny<IRelationalConnection>(), It.IsAny<IReadOnlyDictionary<string, object>>(), It.IsAny<CancellationToken>()))
                .Returns((IRelationalConnection providedConnection, IReadOnlyDictionary<string, object> providedParameterValues, CancellationToken providedCancellationToken) =>
                    Task.FromResult(executeSqlCommandResult));
            var relationalCommand = relationalCommandMock.Object;

            var rawSqlCommandMock = new Mock<RawSqlCommand>(relationalCommand, new Dictionary<string, object>());
            rawSqlCommandMock.Setup(m => m.RelationalCommand).Returns(() => relationalCommand);
            rawSqlCommandMock.Setup(m => m.ParameterValues).Returns(() => new Dictionary<string, object>());
            var rawSqlCommand = rawSqlCommandMock.Object;

            var existingRawSqlCommandBuilder =
                (IRawSqlCommandBuilder) ((IInfrastructure<IServiceProvider>) mockedDbContext.Database).Instance.GetService(typeof(IRawSqlCommandBuilder));

            Mock.Get(existingRawSqlCommandBuilder)
                .Setup(m => m.Build(It.Is<string>(s => s.Contains(sql, StringComparison.CurrentCultureIgnoreCase)),
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