using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EntityFrameworkCore.Testing.Common;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.Extensions;
using rgvlee.Core.Common.Helpers;
using ProjectExpressionHelper = EntityFrameworkCore.Testing.Common.Helpers.ExpressionHelper;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>
    ///     Extensions for collection query providers.
    /// </summary>
    public static partial class QueryProviderExtensions
    {
        private static readonly ILogger Logger = LoggingHelper.CreateLogger(typeof(QueryProviderExtensions));

        /// <summary>
        ///     Sets up FromSql invocations to return a specified result.
        /// </summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryProvider">The mocked query provider.</param>
        /// <param name="fromSqlResult">The FromSql result.</param>
        /// <returns>The mocked queryable.</returns>
        public static IQueryProvider AddFromSqlResult<T>(this IQueryProvider mockedQueryProvider, IEnumerable<T> fromSqlResult) where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryProvider, nameof(mockedQueryProvider));
            mockedQueryProvider.AddFromSqlResult(string.Empty, new List<object>(), fromSqlResult);
            return mockedQueryProvider;
        }

        /// <summary>
        ///     Sets up FromSql invocations containing a specified sql string to return a specified result.
        /// </summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryProvider">The mocked query provider.</param>
        /// <param name="sql">The FromSql sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="fromSqlResult">The FromSql result.</param>
        /// <returns>The mocked queryable.</returns>
        public static IQueryProvider AddFromSqlResult<T>(this IQueryProvider mockedQueryProvider, string sql, IEnumerable<T> fromSqlResult) where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryProvider, nameof(mockedQueryProvider));
            mockedQueryProvider.AddFromSqlResult(sql, new List<object>(), fromSqlResult);
            return mockedQueryProvider;
        }

        /// <summary>
        ///     Sets up FromSql invocations containing a specified sql string and parameters to return a specified result.
        /// </summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryProvider">The mocked query provider.</param>
        /// <param name="sql">The FromSql sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The FromSql parameters. Set up supports case insensitive partial parameter sequence matching.</param>
        /// <param name="fromSqlResult">The sequence to return when FromSql is invoked.</param>
        /// <returns>The mocked query provider.</returns>
        public static IQueryProvider AddFromSqlResult<T>(this IQueryProvider mockedQueryProvider, string sql, IEnumerable<object> parameters, IEnumerable<T> fromSqlResult)
            where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryProvider, nameof(mockedQueryProvider));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));
            EnsureArgument.IsNotNull(fromSqlResult, nameof(fromSqlResult));

            Logger.LogDebug("Setting up '{sql}'", sql);

            var createQueryResult = new AsyncEnumerable<T>(fromSqlResult);

            mockedQueryProvider.Configure()
                .CreateQuery<T>(Arg.Is<MethodCallExpression>(mce => ProjectExpressionHelper.SqlAndParametersMatchFromSqlExpression(sql, parameters, mce)))
                .Returns(callInfo =>
                {
                    ProjectExpressionHelper.ThrowIfExpressionIsNotSupported(callInfo.Arg<Expression>());

                    var mce = (MethodCallExpression) callInfo.Arg<Expression>();
                    var parts = new List<string>();
                    parts.Add("FromSql inputs:");
                    parts.Add(ProjectExpressionHelper.StringifyFromSqlExpression(mce));
                    Logger.LogDebug(string.Join(Environment.NewLine, parts));

                    return createQueryResult;
                });

            return mockedQueryProvider;
        }
    }
}