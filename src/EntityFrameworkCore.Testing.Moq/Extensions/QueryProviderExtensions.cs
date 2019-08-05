// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EntityFrameworkCore.Testing.Moq.Extensions {
    /// <summary>
    /// Extensions for query provider mocks.
    /// </summary>
    public static class QueryProviderExtensions {
        /// <summary>
        /// Sets up FromSql invocations to return a specified result.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="queryProviderMock">The query provider mock.</param>
        /// <param name="expectedFromSqlResult">The sequence to return when FromSql is invoked.</param>
        /// <returns>The query provider mock.</returns>
        public static Mock<IQueryProvider> SetUpFromSql<TEntity>(this Mock<IQueryProvider> queryProviderMock, IEnumerable<TEntity> expectedFromSqlResult) where TEntity : class {

            var createQueryResult = new EntityFrameworkCore.Testing.Moq.AsyncEnumerable<TEntity>(expectedFromSqlResult);

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

            var createQueryResult = new EntityFrameworkCore.Testing.Moq.AsyncEnumerable<TEntity>(expectedFromSqlResult);

            queryProviderMock.Setup(
                    p => p.CreateQuery<TEntity>(It.Is<MethodCallExpression>(mce => SpecifiedParametersMatchMethodCallExpression(mce, sql, sqlParameters)))
                )
                .Returns(createQueryResult)
                .Callback((MethodCallExpression mce) => {
                    Console.WriteLine("FromSql inputs:");
                    Console.WriteLine(StringifyFromSqlMethodCallExpression(mce));
                });

            return queryProviderMock;
        }

        private static bool SqlMatchesMethodCallExpression(MethodCallExpression mce, string sql) {
            var mceRawSqlString = (RawSqlString)((ConstantExpression)mce.Arguments[1]).Value;

            var result = mceRawSqlString.Format.Contains(sql, StringComparison.CurrentCultureIgnoreCase);
            if (result) return mceRawSqlString.Format.Contains(sql, StringComparison.CurrentCultureIgnoreCase);
            Console.WriteLine($"mceRawSqlString: {mceRawSqlString.Format}");
            Console.WriteLine($"sql: {sql}");

            return mceRawSqlString.Format.Contains(sql, StringComparison.CurrentCultureIgnoreCase);
        }

        private static bool SqlParametersMatchMethodCallExpression(MethodCallExpression mce, IEnumerable<SqlParameter> sqlParameters) {
            var mceParameters = ((object[])((ConstantExpression)mce.Arguments[2]).Value);
            var mceSqlParameters = GetSqlParameters(mceParameters).ToList();

            Console.WriteLine("mceSqlParameters:");
            foreach (var parameter in mceSqlParameters) {
                Console.WriteLine($"'{parameter.ParameterName}': '{parameter.Value}'");
            }
            Console.WriteLine("sqlParameters:");
            foreach (var parameter in sqlParameters) {
                Console.WriteLine($"'{parameter.ParameterName}': '{parameter.Value}'");
            }

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
            var sb = new StringBuilder();

            var rawSqlString = ((RawSqlString)((ConstantExpression)mce.Arguments[1]).Value);

            sb.Append(nameof(RawSqlString));
            sb.Append(" sql: ");
            sb.AppendLine(rawSqlString.Format);

            var parameters = (object[])((ConstantExpression)mce.Arguments[2]).Value;
            if (!parameters.Any()) return sb.ToString();

            var sqlParameters = GetSqlParameters(parameters);
            sb.AppendLine("Parameters:");
            foreach (var sqlParameter in sqlParameters) {
                sb.Append(sqlParameter.ParameterName);
                sb.Append(": ");
                if (sqlParameter.Value == null)
                    sb.AppendLine("null");
                else
                    sb.AppendLine(sqlParameter.Value.ToString());
            }

            return sb.ToString();
        }
    }
}