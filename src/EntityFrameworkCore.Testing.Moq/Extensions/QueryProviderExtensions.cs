// ReSharper disable UnusedMember.Global

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
using Moq;

namespace EntityFrameworkCore.Testing.Moq.Extensions {
    /// <summary>
    /// Extensions for query provider mocks.
    /// </summary>
    public static class QueryProviderExtensions {
        private static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(QueryProviderExtensions));

        /// <summary>
        /// Sets up FromSql invocations to return a specified result.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="queryProviderMock">The query provider mock.</param>
        /// <param name="expectedFromSqlResult">The sequence to return when FromSql is invoked.</param>
        /// <returns>The query provider mock.</returns>
        public static Mock<IQueryProvider> SetUpFromSql<TEntity>(this Mock<IQueryProvider> queryProviderMock, IEnumerable<TEntity> expectedFromSqlResult) where TEntity : class {

            var createQueryResult = new AsyncEnumerable<TEntity>(expectedFromSqlResult);

            queryProviderMock.Setup(p => p.CreateQuery<TEntity>(It.IsAny<MethodCallExpression>()))
                .Returns(createQueryResult);

            return queryProviderMock;
        }

        /// <summary>
        /// Sets up FromSql invocations containing a specified sql string to return a specified result. 
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="queryProviderMock">The query provider mock.</param>
        /// <param name="sql">The FromSql sql string. Mock set up supports case insensitive partial matches.</param>
        /// <param name="expectedFromSqlResult">The sequence to return when FromSql is invoked.</param>
        /// <returns>The query provider mock.</returns>
        public static Mock<IQueryProvider> SetUpFromSql<TEntity>(this Mock<IQueryProvider> queryProviderMock, string sql, IEnumerable<TEntity> expectedFromSqlResult) where TEntity : class {
            return queryProviderMock.SetUpFromSql(sql, new List<SqlParameter>(), expectedFromSqlResult);
        }

        /// <summary>
        /// Sets up FromSql invocations containing a specified sql string and sql parameters to return a specified result. 
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="queryProviderMock">The query provider mock.</param>
        /// <param name="sql">The FromSql sql string. Mock set up supports case insensitive partial matches.</param>
        /// <param name="sqlParameters">The FromSql sql parameters. Mock set up supports case insensitive partial sql parameter sequence matching.</param>
        /// <param name="expectedFromSqlResult">The sequence to return when FromSql is invoked.</param>
        /// <returns>The query provider mock.</returns>
        public static Mock<IQueryProvider> SetUpFromSql<TEntity>(this Mock<IQueryProvider> queryProviderMock, string sql, IEnumerable<SqlParameter> sqlParameters, IEnumerable<TEntity> expectedFromSqlResult) where TEntity : class {
            //Microsoft.EntityFrameworkCore.RelationalQueryableExtensions

            //public static IQueryable<TEntity> FromSql<TEntity>(
            //  [NotNull] this IQueryable<TEntity> source,
            //  [NotParameterized] RawSqlString sql,
            //  [NotNull] params object[] parameters)

            //return source.Provider.CreateQuery<TEntity>((Expression) Expression.Call((Expression) null, RelationalQueryableExtensions.FromSqlMethodInfo.MakeGenericMethod(typeof (TEntity)), source.Expression, (Expression) Expression.Constant((object) sql), (Expression) Expression.Constant((object) parameters)));

            var createQueryResult = new AsyncEnumerable<TEntity>(expectedFromSqlResult);

            queryProviderMock.Setup(
                    p => p.CreateQuery<TEntity>(It.Is<MethodCallExpression>(mce => SpecifiedParametersMatchMethodCallExpression(mce, sql, sqlParameters)))
                )
                .Returns(createQueryResult)
                .Callback((MethodCallExpression mce) => {
                    var parts = new List<string>();
                    parts.Add("FromSql inputs:");
                    parts.Add(StringifyFromSqlMethodCallExpression(mce));

                    Logger.LogDebug(string.Join(Environment.NewLine, parts));
                });

            return queryProviderMock;
        }

        private static bool SqlMatchesMethodCallExpression(MethodCallExpression mce, string sql) {
            var parts = new List<string>();

            var mceRawSqlString = (RawSqlString)((ConstantExpression)mce.Arguments[1]).Value;
            
            parts.Add($"Invocation RawSqlString: {mceRawSqlString.Format}");
            parts.Add($"Set up sql: {sql}");

            Logger.LogDebug(string.Join(Environment.NewLine, parts));
            
            return mceRawSqlString.Format.Contains(sql, StringComparison.CurrentCultureIgnoreCase);
        }

        private static bool SqlParametersMatchMethodCallExpression(MethodCallExpression mce, IEnumerable<SqlParameter> sqlParameters) {
            var parts = new List<string>();

            var mceParameters = ((object[])((ConstantExpression)mce.Arguments[2]).Value);
            var mceSqlParameters = GetSqlParameters(mceParameters).ToList();

            parts.Add("Invocation SqlParameters:");
            foreach (var parameter in mceSqlParameters) {
                parts.Add($"'{parameter.ParameterName}': '{parameter.Value}'");
            }
            parts.Add("Set up sqlParameters:");
            foreach (var parameter in sqlParameters) {
                parts.Add($"'{parameter.ParameterName}': '{parameter.Value}'");
            }
            
            Logger.LogDebug(string.Join(Environment.NewLine, parts));

            return !sqlParameters.Except(mceSqlParameters, new SqlParameterParameterNameAndValueEqualityComparer()).Any();
        }

        private static bool SpecifiedParametersMatchMethodCallExpression(MethodCallExpression mce, string sql, IEnumerable<SqlParameter> sqlParameters) {
            return SqlMatchesMethodCallExpression(mce, sql) && SqlParametersMatchMethodCallExpression(mce, sqlParameters);
        }

        private static IEnumerable<SqlParameter> GetSqlParameters(object[] parameters) {
            var result = new List<SqlParameter>();

            if (!parameters.Any()) return result;

            foreach (var parameter in parameters) {
                if (parameter is SqlParameter sqlParameter) {
                    result.Add(sqlParameter);
                }
            }
            return result;
        }

        private static string StringifyFromSqlMethodCallExpression(MethodCallExpression mce) {
            var sb1 = new StringBuilder();

            var rawSqlString = ((RawSqlString)((ConstantExpression)mce.Arguments[1]).Value);

            sb1.Append(nameof(RawSqlString));
            sb1.Append(" sql: ");
            sb1.AppendLine(rawSqlString.Format);

            var parameters = (object[])((ConstantExpression)mce.Arguments[2]).Value;
            if (!parameters.Any()) return sb1.ToString();

            var sqlParameters = GetSqlParameters(parameters);
            sb1.AppendLine("Parameters:");

            var parts = new List<string>();
            foreach (var sqlParameter in sqlParameters) {
                var sb2 = new StringBuilder();
                sb2.Append(sqlParameter.ParameterName);
                sb2.Append(": ");
                if (sqlParameter.Value == null)
                    sb2.Append("null");
                else
                    sb2.Append(sqlParameter.Value);
                parts.Add(sb2.ToString());
            }

            if(parts.Any()) sb1.Append(string.Join(Environment.NewLine, parts));

            return sb1.ToString();
        }
    }
}