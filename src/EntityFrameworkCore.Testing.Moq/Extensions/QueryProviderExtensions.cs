using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.Common.Extensions;
using EntityFrameworkCore.Testing.Common.Helpers;
using EntityFrameworkCore.Testing.Moq.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    /// <summary>
    ///     Extensions for query provider and mock query provider types.
    /// </summary>
    internal static class QueryProviderExtensions
    {
        private static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(QueryProviderExtensions));

        /// <summary>
        ///     Creates a mocked query provider.
        /// </summary>
        /// <typeparam name="T">The query provider source item type.</typeparam>
        /// <param name="queryProviderToMock">The query provider to mock.</param>
        /// <param name="enumerable">The query provider source.</param>
        /// <returns>A mocked query provider.</returns>
        /// <remarks>Extends queryProviderToMock is already a mock, it will be extended.</remarks>
        internal static IQueryProvider CreateMock<T>(this IQueryProvider queryProviderToMock, IEnumerable<T> enumerable) where T : class
        {
            EnsureArgument.IsNotNull(queryProviderToMock, nameof(queryProviderToMock));
            EnsureArgument.IsNotNull(enumerable, nameof(enumerable));

            if (!(queryProviderToMock is AsyncQueryProvider<T> asyncQueryProvider) ||
                MockHelper.TryGetMock(asyncQueryProvider, out var queryProviderMock))
            {
                queryProviderMock = new Mock<AsyncQueryProvider<T>>();
                queryProviderMock.CallBase = true;

                queryProviderMock.As<IQueryProvider>().Setup(
                        p => p.CreateQuery<T>(It.Is<MethodCallExpression>(
                            mce => mce.Method.Name.Equals(nameof(RelationalQueryableExtensions.FromSql))
                        ))
                    )
                    .Callback((Expression expression) => { Logger.LogDebug("Catch all exception invoked"); })
                    .Throws<NotSupportedException>();
            }

            queryProviderMock.SetUpSource(enumerable.AsQueryable());
            return queryProviderMock.Object;
        }

        internal static void SetUpSource<T>(this Mock<AsyncQueryProvider<T>> queryProviderMock, IEnumerable<T> enumerable) where T : class
        {
            EnsureArgument.IsNotNull(queryProviderMock, nameof(queryProviderMock));
            EnsureArgument.IsNotNull(enumerable, nameof(enumerable));

            var queryable = enumerable.AsQueryable();
            queryProviderMock.Setup(m => m.Source).Returns(queryable);
        }

        /// <summary>
        ///     Sets up FromSql invocations containing a specified sql string and sql parameters to return a specified result.
        /// </summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="queryProviderMock">The query provider mock.</param>
        /// <param name="sql">The FromSql sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">
        ///     The FromSql sql parameters. Set up supports case insensitive partial sql parameter sequence
        ///     matching.
        /// </param>
        /// <param name="fromSqlResult">The sequence to return when FromSql is invoked.</param>
        /// <returns>The query provider mock.</returns>
        internal static Mock<IQueryProvider> SetUpFromSql<T>(this Mock<IQueryProvider> queryProviderMock, string sql, IEnumerable<SqlParameter> parameters, IEnumerable<T> fromSqlResult) where T : class
        {
            //Microsoft.EntityFrameworkCore.RelationalQueryableExtensions

            //public static IQueryable<TEntity> FromSql<TEntity>(
            //  [NotNull] this IQueryable<TEntity> source,
            //  [NotParameterized] RawSqlString sql,
            //  [NotNull] params object[] parameters)

            //return source.Provider.CreateQuery<TEntity>((Expression) Expression.Call((Expression) null, RelationalQueryableExtensions.FromSqlMethodInfo.MakeGenericMethod(typeof (TEntity)), source.Expression, (Expression) Expression.Constant((object) sql), (Expression) Expression.Constant((object) parameters)));

            EnsureArgument.IsNotNull(queryProviderMock, nameof(queryProviderMock));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));
            EnsureArgument.IsNotNull(fromSqlResult, nameof(fromSqlResult));

            Logger.LogInformation($"Setting up '{sql}'");

            var createQueryResult = new AsyncEnumerable<T>(fromSqlResult);

            queryProviderMock.Setup(
                    p => p.CreateQuery<T>(It.Is<MethodCallExpression>(mce => SpecifiedParametersMatchMethodCallExpression(mce, sql, parameters)))
                )
                .Returns(() => { return createQueryResult; })
                .Callback((Expression expression) =>
                {
                    var mce = (MethodCallExpression) expression;
                    var parts = new List<string>();
                    parts.Add("FromSql inputs:");
                    parts.Add(StringifyFromSqlMethodCallExpression(mce));
                    Logger.LogInformation(string.Join(Environment.NewLine, parts));
                });

            return queryProviderMock;
        }

        private static bool SqlMatchesMethodCallExpression(MethodCallExpression mce, string sql)
        {
            EnsureArgument.IsNotNull(mce, nameof(mce));

            var parts = new List<string>();
            var mceRawSqlString = (RawSqlString) ((ConstantExpression) mce.Arguments[1]).Value;
            parts.Add($"Invocation RawSqlString: '{mceRawSqlString.Format}'");
            parts.Add($"Set up sql: '{sql}'");
            Logger.LogInformation(string.Join(Environment.NewLine, parts));

            return mceRawSqlString.Format.Contains(sql, StringComparison.CurrentCultureIgnoreCase);
        }

        private static bool SqlParametersMatchMethodCallExpression(MethodCallExpression mce, IEnumerable<SqlParameter> sqlParameters)
        {
            EnsureArgument.IsNotNull(mce, nameof(mce));
            EnsureArgument.IsNotNull(sqlParameters, nameof(sqlParameters));

            var parts = new List<string>();

            var mceParameters = (object[]) ((ConstantExpression) mce.Arguments[2]).Value;
            var mceSqlParameters = GetSqlParameters(mceParameters).ToList();

            parts.Add("Invocation SqlParameters:");
            foreach (var parameter in mceSqlParameters) parts.Add($"'{parameter.ParameterName}': '{parameter.Value}'");
            parts.Add("Set up sqlParameters:");
            foreach (var parameter in sqlParameters) parts.Add($"'{parameter.ParameterName}': '{parameter.Value}'");

            Logger.LogInformation(string.Join(Environment.NewLine, parts));

            return !sqlParameters.Except(mceSqlParameters, new SqlParameterParameterNameAndValueEqualityComparer()).Any();
        }

        private static bool SpecifiedParametersMatchMethodCallExpression(MethodCallExpression mce, string sql, IEnumerable<SqlParameter> sqlParameters)
        {
            EnsureArgument.IsNotNull(mce, nameof(mce));
            EnsureArgument.IsNotNull(sqlParameters, nameof(sqlParameters));

            return mce.Method.Name.Equals(nameof(RelationalQueryableExtensions.FromSql))
                   && SqlMatchesMethodCallExpression(mce, sql)
                   && SqlParametersMatchMethodCallExpression(mce, sqlParameters);
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

            var parts = new List<string>();

            var rawSqlString = (RawSqlString) ((ConstantExpression) mce.Arguments[1]).Value;
            var parameters = (object[]) ((ConstantExpression) mce.Arguments[2]).Value;

            parts.Add($"{nameof(RawSqlString)} sql: '{rawSqlString.Format}'");

            parts.Add("Parameters:");
            foreach (var sqlParameter in GetSqlParameters(parameters))
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