using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using EntityFrameworkCore.Testing.Common;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>Extensions for queryable types.</summary>
    public static class QueryableExtensions
    {
        /// <summary>Sets up FromSql invocations to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryable">The mocked queryable.</param>
        /// <param name="fromSqlResult">The FromSql result.</param>
        /// <returns>The mocked queryable.</returns>
        public static IQueryable<T> AddFromSqlResult<T>(this IQueryable<T> mockedQueryable, IEnumerable<T> fromSqlResult)
            where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryable, nameof(mockedQueryable));
            EnsureArgument.IsNotNull(fromSqlResult, nameof(fromSqlResult));

            return mockedQueryable.AddFromSqlResult(string.Empty, fromSqlResult);
        }

        /// <summary>Sets up FromSql invocations containing a specified sql string to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryable">The mocked queryable.</param>
        /// <param name="sql">The FromSql sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="fromSqlResult">The FromSql result.</param>
        /// <returns>The mocked queryable.</returns>
        public static IQueryable<T> AddFromSqlResult<T>(this IQueryable<T> mockedQueryable, string sql, IEnumerable<T> fromSqlResult)
            where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryable, nameof(mockedQueryable));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(fromSqlResult, nameof(fromSqlResult));

            return mockedQueryable.AddFromSqlResult(sql, new List<SqlParameter>(), fromSqlResult);
        }

        /// <summary>Sets up FromSql invocations containing a specified sql string and sql parameters to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryable">The mocked queryable.</param>
        /// <param name="sql">The FromSql sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The FromSql sql parameters. Set up supports case insensitive partial sql parameter sequence matching.</param>
        /// <param name="fromSqlResult">The FromSql result.</param>
        /// <returns>The mocked queryable.</returns>
        public static IQueryable<T> AddFromSqlResult<T>(this IQueryable<T> mockedQueryable, string sql, IEnumerable<SqlParameter> parameters, IEnumerable<T> fromSqlResult)
            where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryable, nameof(mockedQueryable));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));
            EnsureArgument.IsNotNull(fromSqlResult, nameof(fromSqlResult));

            mockedQueryable.Provider.AddFromSqlResult(sql, parameters, fromSqlResult);
            return mockedQueryable;
        }
    }
}