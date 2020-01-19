using System;
using System.Collections.Generic;
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

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>Extensions for the db context type.</summary>
    public static partial class DbContextExtensions
    {
        private static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(DbContextExtensions));

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

            if (((IInfrastructure<IServiceProvider>) mockedDbContext.Database).Instance.GetService(typeof(IRawSqlCommandBuilder)) is IRawSqlCommandBuilder existingRawSqlCommandBuilder)
            {
                existingRawSqlCommandBuilder
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
            }
            else
            {
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
            }

            return mockedDbContext;
        }
    }
}