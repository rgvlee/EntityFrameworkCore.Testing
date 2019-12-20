using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>Extensions for query provider and mock query provider types.</summary>
    public static class QueryProviderExtensions
    {
        private static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(QueryProviderExtensions));

        /// <summary>Creates a substitute query provider.</summary>
        /// <typeparam name="T">The query provider source item type.</typeparam>
        /// <param name="queryProviderToMock">The query provider to mock.</param>
        /// <param name="enumerable">The query provider source.</param>
        /// <returns>A substitute query provider.</returns>
        public static IQueryProvider CreateSubstituteQueryProvider<T>(this IQueryProvider queryProviderToMock, IEnumerable<T> enumerable)
            where T : class
        {
            EnsureArgument.IsNotNull(queryProviderToMock, nameof(queryProviderToMock));
            EnsureArgument.IsNotNull(enumerable, nameof(enumerable));

            var substituteQueryProvider = Substitute.ForPartsOf<AsyncQueryProvider<T>>(enumerable.AsQueryable());

            substituteQueryProvider
                .Configure()
                .CreateQuery<T>(Arg.Is<MethodCallExpression>(mce => mce.Method.Name.Equals("FromSqlOnQueryable")))
                .Throws(callInfo =>
                {
                    Logger.LogDebug("Catch all exception invoked");
                    return new NotSupportedException();
                });

            return substituteQueryProvider;
        }

        /// <summary>Creates a substitute query provider.</summary>
        /// <typeparam name="T">The query provider source item type.</typeparam>
        /// <param name="queryProviderToMock">The query provider to mock.</param>
        /// <param name="enumerable">The query provider source.</param>
        /// <returns>A substitute query provider.</returns>
        [Obsolete("This will be removed in a future version. Use QueryProviderExtensions.CreateSubstituteQueryProvider instead.")]
        public static IQueryProvider CreateMock<T>(this IQueryProvider queryProviderToMock, IEnumerable<T> enumerable)
            where T : class
        {
            return queryProviderToMock.CreateSubstituteQueryProvider(enumerable);
        }

        internal static void SetSource<T>(this AsyncQueryProvider<T> substituteQueryProvider, IEnumerable<T> enumerable)
            where T : class
        {
            EnsureArgument.IsNotNull(substituteQueryProvider, nameof(substituteQueryProvider));
            EnsureArgument.IsNotNull(enumerable, nameof(enumerable));

            var queryable = enumerable.AsQueryable();
            substituteQueryProvider.Configure().Source.Returns(callInfo => queryable);
        }

        /// <summary>Sets up FromSqlInterpolated invocations to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="substituteQueryProvider">The substitute query provider.</param>
        /// <param name="fromSqlInterpolatedResult">The FromSqlInterpolated result.</param>
        /// <returns>The substitute queryable.</returns>
        public static IQueryProvider AddFromSqlInterpolatedResult<T>(this IQueryProvider substituteQueryProvider, IEnumerable<T> fromSqlInterpolatedResult)
            where T : class
        {
            EnsureArgument.IsNotNull(substituteQueryProvider, nameof(substituteQueryProvider));
            EnsureArgument.IsNotNull(fromSqlInterpolatedResult, nameof(fromSqlInterpolatedResult));

            substituteQueryProvider.AddFromSqlRawResult(string.Empty, new List<object>(), fromSqlInterpolatedResult);
            return substituteQueryProvider;
        }

        /// <summary>Sets up FromSqlInterpolated invocations containing a specified sql string to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="substituteQueryProvider">The substitute query provider.</param>
        /// <param name="sql">The FromSqlInterpolated sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="fromSqlInterpolatedResult">The FromSqlInterpolated result.</param>
        /// <returns>The substitute queryable.</returns>
        public static IQueryProvider AddFromSqlInterpolatedResult<T>(this IQueryProvider substituteQueryProvider, FormattableString sql, IEnumerable<T> fromSqlInterpolatedResult)
            where T : class
        {
            EnsureArgument.IsNotNull(substituteQueryProvider, nameof(substituteQueryProvider));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(fromSqlInterpolatedResult, nameof(fromSqlInterpolatedResult));

            substituteQueryProvider.AddFromSqlRawResult(sql.Format, sql.GetArguments(), fromSqlInterpolatedResult);
            return substituteQueryProvider;
        }

        /// <summary>Sets up FromSqlInterpolated invocations containing a specified sql string and parameters to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="substituteQueryProvider">The substitute query provider.</param>
        /// <param name="sql">The FromSqlInterpolated sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The FromSqlInterpolated parameters. Set up supports case insensitive partial parameter sequence matching.</param>
        /// <param name="fromSqlInterpolatedResult">The sequence to return when FromSqlInterpolated is invoked.</param>
        /// <returns>The substitute query provider.</returns>
        public static IQueryProvider AddFromSqlInterpolatedResult<T>(this IQueryProvider substituteQueryProvider, string sql, IEnumerable<object> parameters, IEnumerable<T> fromSqlInterpolatedResult)
            where T : class
        {
            EnsureArgument.IsNotNull(substituteQueryProvider, nameof(substituteQueryProvider));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));
            EnsureArgument.IsNotNull(fromSqlInterpolatedResult, nameof(fromSqlInterpolatedResult));

            substituteQueryProvider.AddFromSqlRawResult(sql, parameters, fromSqlInterpolatedResult);
            return substituteQueryProvider;
        }

        /// <summary>Sets up FromSqlRaw invocations to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="substituteQueryProvider">The substitute query provider.</param>
        /// <param name="fromSqlRawResult">The FromSqlRaw result.</param>
        /// <returns>The substitute queryable.</returns>
        public static IQueryProvider AddFromSqlRawResult<T>(this IQueryProvider substituteQueryProvider, IEnumerable<T> fromSqlRawResult)
            where T : class
        {
            EnsureArgument.IsNotNull(substituteQueryProvider, nameof(substituteQueryProvider));
            EnsureArgument.IsNotNull(fromSqlRawResult, nameof(fromSqlRawResult));

            substituteQueryProvider.AddFromSqlRawResult(string.Empty, new List<object>(), fromSqlRawResult);
            return substituteQueryProvider;
        }

        /// <summary>Sets up FromSqlRaw invocations containing a specified sql string to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="substituteQueryProvider">The substitute query provider.</param>
        /// <param name="sql">The FromSqlRaw sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="fromSqlRawResult">The FromSqlRaw result.</param>
        /// <returns>The substitute queryable.</returns>
        public static IQueryProvider AddFromSqlRawResult<T>(this IQueryProvider substituteQueryProvider, string sql, IEnumerable<T> fromSqlRawResult)
            where T : class
        {
            EnsureArgument.IsNotNull(substituteQueryProvider, nameof(substituteQueryProvider));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(fromSqlRawResult, nameof(fromSqlRawResult));

            substituteQueryProvider.AddFromSqlRawResult(sql, new List<object>(), fromSqlRawResult);
            return substituteQueryProvider;
        }

        /// <summary>Sets up FromSqlRaw invocations containing a specified sql string and parameters to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="substituteQueryProvider">The substitute query provider.</param>
        /// <param name="sql">The FromSqlRaw sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The FromSqlRaw parameters. Set up supports case insensitive partial parameter sequence matching.</param>
        /// <param name="fromSqlResult">The sequence to return when FromSqlRaw is invoked.</param>
        /// <returns>The substitute query provider.</returns>
        public static IQueryProvider AddFromSqlRawResult<T>(this IQueryProvider substituteQueryProvider, string sql, IEnumerable<object> parameters, IEnumerable<T> fromSqlResult)
            where T : class
        {
            EnsureArgument.IsNotNull(substituteQueryProvider, nameof(substituteQueryProvider));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));
            EnsureArgument.IsNotNull(fromSqlResult, nameof(fromSqlResult));

            Logger.LogDebug($"Setting up '{sql}'");

            var createQueryResult = new AsyncEnumerable<T>(fromSqlResult);

            //TODO: SpecifiedParametersMatchMethodCallExpression is being invoked during set up; is there an alternative way to do this?
            substituteQueryProvider
                .Configure()
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

            return substituteQueryProvider;
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
            //NSubstitute invokes this method during set up; this is not desired
            //If this method is invoked with a null mce it was invoked during set up
            if (mce == null) return false;

            //EnsureArgument.IsNotNull(mce, nameof(mce));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));

            var result = mce.Method.Name.Equals("FromSqlOnQueryable")
                         && SqlMatchesMethodCallExpression(mce, sql)
                         && ParameterMatchingHelper.DoInvocationParametersMatchSetUpParameters(parameters, (object[]) ((ConstantExpression) mce.Arguments[2]).Value);

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