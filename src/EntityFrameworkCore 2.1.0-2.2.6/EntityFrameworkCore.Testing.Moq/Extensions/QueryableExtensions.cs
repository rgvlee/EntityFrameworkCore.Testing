using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Testing.Common;

namespace EntityFrameworkCore.Testing.Moq.Extensions
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
            mockedQueryable.Provider.AddFromSqlResult(string.Empty, new List<object>(), fromSqlResult);
            return mockedQueryable;
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
            mockedQueryable.Provider.AddFromSqlResult(sql, new List<object>(), fromSqlResult);
            return mockedQueryable;
        }

        /// <summary>Sets up FromSql invocations containing a specified sql string and parameters to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryable">The mocked queryable.</param>
        /// <param name="sql">The FromSql sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The FromSql parameters. Set up supports case insensitive partial parameter sequence matching.</param>
        /// <param name="fromSqlResult">The FromSql result.</param>
        /// <returns>The mocked queryable.</returns>
        public static IQueryable<T> AddFromSqlResult<T>(this IQueryable<T> mockedQueryable, string sql, IEnumerable<object> parameters, IEnumerable<T> fromSqlResult)
            where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryable, nameof(mockedQueryable));
            mockedQueryable.Provider.AddFromSqlResult(sql, parameters, fromSqlResult);
            return mockedQueryable;
        }
    }
}