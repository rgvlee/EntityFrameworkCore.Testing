using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Testing.Common;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>Extensions for queryable types.</summary>
    public static class QueryableExtensions
    {
        /// <summary>Sets up FromSql invocations to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="substituteQueryable">The substitute queryable.</param>
        /// <param name="fromSqlResult">The FromSql result.</param>
        /// <returns>The substitute queryable.</returns>
        public static IQueryable<T> AddFromSqlResult<T>(this IQueryable<T> substituteQueryable, IEnumerable<T> fromSqlResult)
            where T : class
        {
            EnsureArgument.IsNotNull(substituteQueryable, nameof(substituteQueryable));
            EnsureArgument.IsNotNull(fromSqlResult, nameof(fromSqlResult));

            substituteQueryable.Provider.AddFromSqlResult(string.Empty, new List<object>(), fromSqlResult);
            return substituteQueryable;
        }

        /// <summary>Sets up FromSql invocations containing a specified sql string to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="substituteQueryable">The substitute queryable.</param>
        /// <param name="sql">The FromSql sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="fromSqlResult">The FromSql result.</param>
        /// <returns>The substitute queryable.</returns>
        public static IQueryable<T> AddFromSqlResult<T>(this IQueryable<T> substituteQueryable, string sql, IEnumerable<T> fromSqlResult)
            where T : class
        {
            EnsureArgument.IsNotNull(substituteQueryable, nameof(substituteQueryable));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(fromSqlResult, nameof(fromSqlResult));

            substituteQueryable.Provider.AddFromSqlResult(sql, new List<object>(), fromSqlResult);
            return substituteQueryable;
        }

        /// <summary>Sets up FromSql invocations containing a specified sql string and parameters to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="substituteQueryable">The substitute queryable.</param>
        /// <param name="sql">The FromSql sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The FromSql parameters. Set up supports case insensitive partial parameter sequence matching.</param>
        /// <param name="fromSqlResult">The FromSql result.</param>
        /// <returns>The substitute queryable.</returns>
        public static IQueryable<T> AddFromSqlResult<T>(this IQueryable<T> substituteQueryable, string sql, IEnumerable<object> parameters, IEnumerable<T> fromSqlResult)
            where T : class
        {
            EnsureArgument.IsNotNull(substituteQueryable, nameof(substituteQueryable));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));
            EnsureArgument.IsNotNull(fromSqlResult, nameof(fromSqlResult));

            substituteQueryable.Provider.AddFromSqlResult(sql, parameters, fromSqlResult);
            return substituteQueryable;
        }
    }
}