using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.Common.Extensions;
using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
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

            var mockedQueryProvider = Substitute.ForPartsOf<AsyncQueryProvider<T>>();

            mockedQueryProvider
                .Configure()
                .CreateQuery<T>(Arg.Is<MethodCallExpression>(mce => mce.Method.Name.Equals(nameof(RelationalQueryableExtensions.FromSql))))
                .Throws(callInfo =>
                {
                    Logger.LogDebug("Catch all exception invoked");
                    return new NotSupportedException();
                });

            mockedQueryProvider.Configure().Source.Returns(enumerable.AsQueryable());

            return mockedQueryProvider;
        }

        internal static void SetSource<T>(this AsyncQueryProvider<T> mockedQueryProvider, IEnumerable<T> enumerable) where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryProvider, nameof(mockedQueryProvider));
            EnsureArgument.IsNotNull(enumerable, nameof(enumerable));

            var queryable = enumerable.AsQueryable();
            mockedQueryProvider.Configure().Source.Returns(queryable);
        }

        /// <summary>Sets up FromSql invocations containing a specified sql string and sql parameters to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryProvider">The mocked query provider.</param>
        /// <param name="sql">The FromSql sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The FromSql sql parameters. Set up supports case insensitive partial sql parameter sequence matching.</param>
        /// <param name="fromSqlResult">The sequence to return when FromSql is invoked.</param>
        /// <returns>The mocked query provider.</returns>
        internal static IQueryProvider AddFromSqlResult<T>(this IQueryProvider mockedQueryProvider, string sql, IEnumerable<SqlParameter> parameters, IEnumerable<T> fromSqlResult) where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryProvider, nameof(mockedQueryProvider));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));
            EnsureArgument.IsNotNull(fromSqlResult, nameof(fromSqlResult));

            Logger.LogDebug($"Setting up '{sql}'");

            var createQueryResult = new AsyncEnumerable<T>(fromSqlResult);

            //TODO: SpecifiedParametersMatchMethodCallExpression is being invoked during set up; is there an alternative way to do this?
            mockedQueryProvider
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

        private static bool SqlParametersMatchMethodCallExpression(MethodCallExpression mce, IEnumerable<SqlParameter> sqlParameters)
        {
            EnsureArgument.IsNotNull(mce, nameof(mce));
            EnsureArgument.IsNotNull(sqlParameters, nameof(sqlParameters));

            var mceParameters = (object[]) ((ConstantExpression) mce.Arguments[2]).Value;
            var mceSqlParameters = GetSqlParameters(mceParameters).ToList();
            var parts = new List<string>();
            parts.Add("Invocation SqlParameters:");
            parts.AddRange(mceSqlParameters.Select(parameter => $"'{parameter.ParameterName}': '{parameter.Value}'"));
            parts.Add("Set up sqlParameters:");
            parts.AddRange(sqlParameters.Select(parameter => $"'{parameter.ParameterName}': '{parameter.Value}'"));

            Logger.LogDebug(string.Join(Environment.NewLine, parts));

            if (!mceSqlParameters.Any())
            {
                return true;
            }

            var result = !sqlParameters.Except(mceSqlParameters, new SqlParameterParameterNameAndValueEqualityComparer()).Any();

            Logger.LogDebug($"Match? {result}");

            return result;
        }

        private static bool SpecifiedParametersMatchMethodCallExpression(MethodCallExpression mce, string sql, IEnumerable<SqlParameter> sqlParameters)
        {
            //NSubstitute invokes this method during set up; this is not desired
            //If this method is invoked with a null mce it was invoked during set up
            if (mce == null) return false;

            //EnsureArgument.IsNotNull(mce, nameof(mce));
            EnsureArgument.IsNotNull(sqlParameters, nameof(sqlParameters));

            var result = mce.Method.Name.Equals(nameof(RelationalQueryableExtensions.FromSql))
                         && SqlMatchesMethodCallExpression(mce, sql)
                         && SqlParametersMatchMethodCallExpression(mce, sqlParameters);

            Logger.LogDebug($"Match? {result}");

            return result;
        }

        private static IEnumerable<SqlParameter> GetSqlParameters(object[] parameters)
        {
            EnsureArgument.IsNotNull(parameters, nameof(parameters));

            var result = new List<SqlParameter>();

            if (!parameters.Any())
            {
                return result;
            }

            foreach (var parameter in parameters)
                if (parameter is SqlParameter sqlParameter)
                {
                    result.Add(sqlParameter);
                }

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
            foreach (var sqlParameter in GetSqlParameters(mceParameters))
            {
                var sb2 = new StringBuilder();
                sb2.Append(sqlParameter.ParameterName);
                sb2.Append(": ");
                if (sqlParameter.Value == null)
                {
                    sb2.Append("null");
                }
                else
                {
                    sb2.Append(sqlParameter.Value);
                }

                parts.Add(sb2.ToString());
            }

            return string.Join(Environment.NewLine, parts);
        }
    }
}