#pragma warning disable EF1001 // Internal EF Core API usage.

using System;
using System.Collections.Generic;
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

        /// <summary>Sets up ExecuteSqlInterpolated invocations to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="executeSqlInterpolatedResult">The integer to return when ExecuteSqlInterpolated is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlInterpolatedResult<TDbContext>(this TDbContext mockedDbContext, int executeSqlInterpolatedResult, Action<string, IEnumerable<object>> callback = null)
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
        public static TDbContext AddExecuteSqlInterpolatedResult<TDbContext>(
            this TDbContext mockedDbContext, FormattableString sql, int executeSqlInterpolatedResult, Action<string, IEnumerable<object>> callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
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
        public static TDbContext AddExecuteSqlInterpolatedResult<TDbContext>(
            this TDbContext mockedDbContext, string sql, IEnumerable<object> parameters, int executeSqlInterpolatedResult, Action<string, IEnumerable<object>> callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            return mockedDbContext.AddExecuteSqlRawResult(sql, parameters, executeSqlInterpolatedResult, callback);
        }

        /// <summary>Sets up ExecuteSqlRaw invocations to return a specified result.</summary>
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

        /// <summary>Sets up ExecuteSqlRaw invocations containing a specified sql string and parameters to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlRaw sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="executeSqlRawResult">The integer to return when ExecuteSqlRaw is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        public static TDbContext AddExecuteSqlRawResult<TDbContext>(this TDbContext mockedDbContext, string sql, int executeSqlRawResult, Action<string, IEnumerable<object>> callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
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
        public static TDbContext AddExecuteSqlRawResult<TDbContext>(
            this TDbContext mockedDbContext, string sql, IEnumerable<object> parameters, int executeSqlRawResult, Action<string, IEnumerable<object>> callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));

            var relationalCommandMock = new Mock<IRelationalCommand>();
            relationalCommandMock
                .Setup(m => m.ExecuteNonQuery(It.IsAny<RelationalCommandParameterObject>()))
                .Returns((RelationalCommandParameterObject providedRelationalCommandParameterObject) => executeSqlRawResult);

            relationalCommandMock
                .Setup(m => m.ExecuteNonQueryAsync(It.IsAny<RelationalCommandParameterObject>(), It.IsAny<CancellationToken>()))
                .Returns((RelationalCommandParameterObject providedRelationalCommandParameterObject, CancellationToken providedCancellationToken) => Task.FromResult(executeSqlRawResult));
            var relationalCommand = relationalCommandMock.Object;

            var rawSqlCommandMock = new Mock<RawSqlCommand>(MockBehavior.Strict, relationalCommand, new Dictionary<string, object>());
            rawSqlCommandMock.Setup(m => m.RelationalCommand).Returns(relationalCommand);
            rawSqlCommandMock.Setup(m => m.ParameterValues).Returns(new Dictionary<string, object>());
            var rawSqlCommand = rawSqlCommandMock.Object;

            var rawSqlCommandBuilderMock = new Mock<IRawSqlCommandBuilder>();
            rawSqlCommandBuilderMock
                .Setup(m => m.Build(It.IsAny<string>(), It.IsAny<IEnumerable<object>>()))
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
                    callback?.Invoke(providedSql, providedParameters);

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

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(m => m.GetService(It.Is<Type>(t => t == typeof(IDatabaseFacadeDependencies)))).Returns((Type providedType) => dependencies);
            var serviceProvider = serviceProviderMock.Object;

            Mock.Get(mockedDbContext).As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance).Returns(serviceProvider);

            var databaseFacadeMock = new Mock<DatabaseFacade>(MockBehavior.Strict, mockedDbContext);
            var databaseFacade = databaseFacadeMock.Object;

            Mock.Get(mockedDbContext).Setup(m => m.Database).Returns(databaseFacade);

            return mockedDbContext;
        }
    }
}