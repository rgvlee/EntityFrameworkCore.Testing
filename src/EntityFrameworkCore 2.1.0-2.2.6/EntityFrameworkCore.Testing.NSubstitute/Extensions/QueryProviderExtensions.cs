﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.Common.Extensions;
using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.Extensions;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>
    ///     Extensions for collection query providers.
    /// </summary>
    public static partial class QueryProviderExtensions
    {
        private static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(QueryProviderExtensions));

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

            Logger.LogDebug($"Setting up '{sql}'");

            var createQueryResult = new AsyncEnumerable<T>(fromSqlResult);

            //TODO: SpecifiedParametersMatchMethodCallExpression is being invoked during set up; is there an alternative way to do this?
            mockedQueryProvider.Configure()
                .CreateQuery<T>(Arg.Is<MethodCallExpression>(mce => SpecifiedParametersMatchMethodCallExpression(mce, sql, parameters)))
                .Returns(callInfo =>
                {
                    var mce = (MethodCallExpression) callInfo.Arg<Expression>();
                    var parts = new List<string>();
                    parts.Add("FromSql inputs:");
                    parts.Add(StringifyFromSqlMethodCallExpression(mce));
                    Logger.LogDebug(string.Join(Environment.NewLine, parts));

                    return createQueryResult;
                });

            return mockedQueryProvider;
        }

        private static bool SqlMatchesMethodCallExpression(MethodCallExpression mce, string sql)
        {
            EnsureArgument.IsNotNull(mce, nameof(mce));

            var mceRawSqlString = (RawSqlString) ((ConstantExpression) mce.Arguments[1]).Value;
            var mceSql = mceRawSqlString.Format;
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
            //NSubstitute invokes this method during set up; this is not desired
            //If this method is invoked with a null mce it was invoked during set up
            if (mce == null)
            {
                return false;
            }

            //EnsureArgument.IsNotNull(mce, nameof(mce));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));

            var result = mce.Method.Name.Equals(nameof(RelationalQueryableExtensions.FromSql)) &&
                         SqlMatchesMethodCallExpression(mce, sql) &&
                         ParameterMatchingHelper.DoInvocationParametersMatchSetUpParameters(parameters, (object[]) ((ConstantExpression) mce.Arguments[2]).Value);

            Logger.LogDebug($"Match? {result}");

            return result;
        }

        private static string StringifyFromSqlMethodCallExpression(MethodCallExpression mce)
        {
            EnsureArgument.IsNotNull(mce, nameof(mce));

            var mceRawSqlString = (RawSqlString) ((ConstantExpression) mce.Arguments[1]).Value;
            var mceSql = mceRawSqlString.Format;
            var mceParameters = (object[]) ((ConstantExpression) mce.Arguments[2]).Value;
            var parts = new List<string>();
            parts.Add($"Invocation sql: '{mceSql}'");
            parts.Add("Invocation Parameters:");
            parts.Add(ParameterMatchingHelper.StringifyParameters(mceParameters));
            return string.Join(Environment.NewLine, parts);
        }
    }
}