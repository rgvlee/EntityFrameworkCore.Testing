﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EntityFrameworkCore.Testing.Common;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using rgvlee.Core.Common.Helpers;
using ProjectExpressionHelper = EntityFrameworkCore.Testing.Common.Helpers.ExpressionHelper;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    /// <summary>
    ///     Extensions for collection query providers.
    /// </summary>
    public static partial class QueryProviderExtensions
    {
        private static readonly ILogger Logger = LoggingHelper.CreateLogger(typeof(QueryProviderExtensions));

        /// <summary>
        ///     Sets up FromSqlInterpolated invocations to return a specified result.
        /// </summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryProvider">The mocked query provider.</param>
        /// <param name="fromSqlInterpolatedResult">The FromSqlInterpolated result.</param>
        /// <returns>The mocked queryable.</returns>
        public static IQueryProvider AddFromSqlInterpolatedResult<T>(this IQueryProvider mockedQueryProvider, IEnumerable<T> fromSqlInterpolatedResult) where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryProvider, nameof(mockedQueryProvider));
            mockedQueryProvider.AddFromSqlRawResult(string.Empty, new List<object>(), fromSqlInterpolatedResult);
            return mockedQueryProvider;
        }

        /// <summary>
        ///     Sets up FromSqlInterpolated invocations containing a specified sql string to return a specified result.
        /// </summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryProvider">The mocked query provider.</param>
        /// <param name="sql">The FromSqlInterpolated sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="fromSqlInterpolatedResult">The FromSqlInterpolated result.</param>
        /// <returns>The mocked queryable.</returns>
        public static IQueryProvider AddFromSqlInterpolatedResult<T>(this IQueryProvider mockedQueryProvider, FormattableString sql, IEnumerable<T> fromSqlInterpolatedResult)
            where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryProvider, nameof(mockedQueryProvider));
            mockedQueryProvider.AddFromSqlRawResult(sql.Format, sql.GetArguments(), fromSqlInterpolatedResult);
            return mockedQueryProvider;
        }

        /// <summary>
        ///     Sets up FromSqlInterpolated invocations containing a specified sql string and parameters to return a specified result.
        /// </summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryProvider">The mocked query provider.</param>
        /// <param name="sql">The FromSqlInterpolated sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The FromSqlInterpolated parameters. Set up supports case insensitive partial parameter sequence matching.</param>
        /// <param name="fromSqlInterpolatedResult">The sequence to return when FromSqlInterpolated is invoked.</param>
        /// <returns>The mocked query provider.</returns>
        public static IQueryProvider AddFromSqlInterpolatedResult<T>(
            this IQueryProvider mockedQueryProvider, string sql, IEnumerable<object> parameters, IEnumerable<T> fromSqlInterpolatedResult) where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryProvider, nameof(mockedQueryProvider));
            mockedQueryProvider.AddFromSqlRawResult(sql, parameters, fromSqlInterpolatedResult);
            return mockedQueryProvider;
        }

        /// <summary>
        ///     Sets up FromSqlRaw invocations to return a specified result.
        /// </summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryProvider">The mocked query provider.</param>
        /// <param name="fromSqlRawResult">The FromSqlRaw result.</param>
        /// <returns>The mocked queryable.</returns>
        public static IQueryProvider AddFromSqlRawResult<T>(this IQueryProvider mockedQueryProvider, IEnumerable<T> fromSqlRawResult) where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryProvider, nameof(mockedQueryProvider));
            mockedQueryProvider.AddFromSqlRawResult(string.Empty, new List<object>(), fromSqlRawResult);
            return mockedQueryProvider;
        }

        /// <summary>
        ///     Sets up FromSqlRaw invocations containing a specified sql string to return a specified result.
        /// </summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryProvider">The mocked query provider.</param>
        /// <param name="sql">The FromSqlRaw sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="fromSqlRawResult">The FromSqlRaw result.</param>
        /// <returns>The mocked queryable.</returns>
        public static IQueryProvider AddFromSqlRawResult<T>(this IQueryProvider mockedQueryProvider, string sql, IEnumerable<T> fromSqlRawResult) where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryProvider, nameof(mockedQueryProvider));
            mockedQueryProvider.AddFromSqlRawResult(sql, new List<object>(), fromSqlRawResult);
            return mockedQueryProvider;
        }

        /// <summary>
        ///     Sets up FromSqlRaw invocations containing a specified sql string and parameters to return a specified result.
        /// </summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryProvider">The mocked query provider.</param>
        /// <param name="sql">The FromSqlRaw sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The FromSqlRaw parameters. Set up supports case insensitive partial parameter sequence matching.</param>
        /// <param name="fromSqlResult">The sequence to return when FromSqlRaw is invoked.</param>
        /// <returns>The mocked query provider.</returns>
        public static IQueryProvider AddFromSqlRawResult<T>(this IQueryProvider mockedQueryProvider, string sql, IEnumerable<object> parameters, IEnumerable<T> fromSqlResult)
            where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryProvider, nameof(mockedQueryProvider));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));
            EnsureArgument.IsNotNull(fromSqlResult, nameof(fromSqlResult));

            Logger.LogDebug("Setting up '{sql}'", sql);

            var createQueryResult = new AsyncEnumerable<T>(fromSqlResult);

            Mock.Get(mockedQueryProvider)
                .Setup(m => m.CreateQuery<T>(It.Is<FromSqlQueryRootExpression>(fsqre => ProjectExpressionHelper.SqlAndParametersMatchFromSqlExpression(sql, parameters, fsqre))))
                .Returns((Expression providedExpression) =>
                {
                    ProjectExpressionHelper.ThrowIfExpressionIsNotSupported(providedExpression);
                    return createQueryResult;
                })
                .Callback((Expression providedExpression) =>
                {
                    var fsqre = (FromSqlQueryRootExpression) providedExpression;
                    var parts = new List<string>();
                    parts.Add("FromSql inputs:");
                    parts.Add(ProjectExpressionHelper.StringifyFromSqlExpression(fsqre));
                    Logger.LogDebug(string.Join(Environment.NewLine, parts));
                });

            return mockedQueryProvider;
        }
    }
}