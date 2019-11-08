using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    /// <summary>Extensions for query provider and mock query provider types.</summary>
    internal static class QueryProviderExtensions
    {
        private static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(QueryProviderExtensions));

        /// <summary>Creates a mocked query provider.</summary>
        /// <typeparam name="T">The query provider source item type.</typeparam>
        /// <param name="queryProviderToMock">The query provider to mock.</param>
        /// <param name="enumerable">The query provider source.</param>
        /// <returns>A mocked query provider.</returns>
        /// <remarks>Extends queryProviderToMock if it is a mock.</remarks>
        internal static IQueryProvider CreateMock<T>(this IQueryProvider queryProviderToMock, IEnumerable<T> enumerable) where T : class
        {
            EnsureArgument.IsNotNull(queryProviderToMock, nameof(queryProviderToMock));
            EnsureArgument.IsNotNull(enumerable, nameof(enumerable));

            var queryProviderMock = new Mock<AsyncQueryProvider<T>>();
            queryProviderMock.CallBase = true;

            queryProviderMock
                .As<IQueryProvider>()
                .Setup(m => m.CreateQuery<T>(It.Is<MethodCallExpression>(mce => mce.Method.Name.Equals("FromSqlOnQueryable"))))
                .Callback((Expression providedExpression) => { Logger.LogDebug("Catch all exception invoked"); })
                .Throws<NotSupportedException>();

            queryProviderMock.Setup(m => m.Source).Returns(enumerable.AsQueryable());

            return queryProviderMock.Object;
        }

        internal static void SetSource<T>(this AsyncQueryProvider<T> mockedQueryProvider, IEnumerable<T> enumerable) where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryProvider, nameof(mockedQueryProvider));
            EnsureArgument.IsNotNull(enumerable, nameof(enumerable));

            var queryProviderMock = Mock.Get(mockedQueryProvider);

            var queryable = enumerable.AsQueryable();
            queryProviderMock.Setup(m => m.Source).Returns(queryable);
        }

        /// <summary>Sets up FromSqlRaw invocations containing a specified sql string and sql parameters to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryProvider">The mocked query provider.</param>
        /// <param name="sql">The FromSqlRaw sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The FromSqlRaw sql parameters. Set up supports case insensitive partial sql parameter sequence matching.</param>
        /// <param name="fromSqlResult">The sequence to return when FromSqlRaw is invoked.</param>
        /// <returns>The mocked query provider.</returns>
        internal static IQueryProvider AddFromSqlRawResult<T>(this IQueryProvider mockedQueryProvider, string sql, IEnumerable<object> parameters, IEnumerable<T> fromSqlResult) where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryProvider, nameof(mockedQueryProvider));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));
            EnsureArgument.IsNotNull(fromSqlResult, nameof(fromSqlResult));

            Logger.LogDebug($"Setting up '{sql}'");

            var queryProviderMock = Mock.Get(mockedQueryProvider);

            var createQueryResult = new AsyncEnumerable<T>(fromSqlResult);

            queryProviderMock
                .Setup(m => m.CreateQuery<T>(It.Is<MethodCallExpression>(mce => SpecifiedParametersMatchMethodCallExpression(mce, sql, parameters))))
                .Returns((Expression providedExpression) => createQueryResult)
                .Callback((Expression providedExpression) =>
                {
                    var mce = (MethodCallExpression) providedExpression;
                    var parts = new List<string>();
                    parts.Add("FromSql inputs:");
                    parts.Add(StringifyFromSqlMethodCallExpression(mce));
                    Logger.LogDebug(string.Join(Environment.NewLine, parts));
                });

            return mockedQueryProvider;
        }

        private static bool SqlMatchesMethodCallExpression(MethodCallExpression mce, string sql)
        {
            EnsureArgument.IsNotNull(mce, nameof(mce));

            var mceSql = (string) ((ConstantExpression) mce.Arguments[1]).Value;
            var parts = new List<string>();
            parts.Add($"Invocation sql: '{mceSql}'");
            parts.Add($"Set up sql: '{sql}'");
            Logger.LogDebug(string.Join(Environment.NewLine, parts));

            var result = mceSql.Contains(sql, StringComparison.CurrentCultureIgnoreCase);

            Logger.LogDebug($"Match? {result}");

            return result;
        }

        private static bool SpecifiedParametersMatchMethodCallExpression(MethodCallExpression mce, string sql, IEnumerable<object> parameters)
        {
            EnsureArgument.IsNotNull(mce, nameof(mce));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));

            var result = mce.Method.Name.Equals("FromSqlOnQueryable")
                         && SqlMatchesMethodCallExpression(mce, sql)
                         && ParameterMatchingHelper.DoInvocationParametersMatchSetUpParameters((object[]) ((ConstantExpression) mce.Arguments[2]).Value, parameters);

            Logger.LogDebug($"Match? {result}");

            return result;
        }

        private static string StringifyFromSqlMethodCallExpression(MethodCallExpression mce)
        {
            EnsureArgument.IsNotNull(mce, nameof(mce));

            var mceSql = (string) ((ConstantExpression) mce.Arguments[1]).Value;
            var mceParameters = (object[]) ((ConstantExpression) mce.Arguments[2]).Value;
            var parts = new List<string>();
            parts.Add($"Invocation sql: '{mceSql}'");
            parts.Add("Invocation Parameters:");
            parts.Add(ParameterMatchingHelper.StringifyParameters(mceParameters));
            return string.Join(Environment.NewLine, parts);
        }
    }
}