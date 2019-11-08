using System;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Testing.Common;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    /// <summary>Extensions for queryable types.</summary>
    public static class QueryableExtensions
    {
        /// <summary>Sets up FromSqlInterpolated invocations to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryable">The mocked queryable.</param>
        /// <param name="fromSqlInterpolatedResult">The FromSqlInterpolated result.</param>
        /// <returns>The mocked queryable.</returns>
        public static IQueryable<T> AddFromSqlInterpolatedResult<T>(this IQueryable<T> mockedQueryable, IEnumerable<T> fromSqlInterpolatedResult)
            where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryable, nameof(mockedQueryable));
            EnsureArgument.IsNotNull(fromSqlInterpolatedResult, nameof(fromSqlInterpolatedResult));

            mockedQueryable.Provider.AddFromSqlRawResult(string.Empty, new List<object>(), fromSqlInterpolatedResult);
            return mockedQueryable;
        }

        /// <summary>Sets up FromSqlInterpolated invocations to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryable">The mocked queryable.</param>
        /// <param name="sql">The FromSqlInterpolated sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="fromSqlInterpolatedResult">The FromSqlInterpolated result.</param>
        /// <returns>The mocked queryable.</returns>
        public static IQueryable<T> AddFromSqlInterpolatedResult<T>(this IQueryable<T> mockedQueryable, string sql, IEnumerable<T> fromSqlInterpolatedResult)
            where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryable, nameof(mockedQueryable));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(fromSqlInterpolatedResult, nameof(fromSqlInterpolatedResult));

            mockedQueryable.Provider.AddFromSqlRawResult(sql, new List<object>(), fromSqlInterpolatedResult);
            return mockedQueryable;
        }

        /// <summary>Sets up FromSqlInterpolated invocations to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryable">The mocked queryable.</param>
        /// <param name="sql">The FromSqlInterpolated sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="fromSqlInterpolatedResult">The FromSqlInterpolated result.</param>
        /// <returns>The mocked queryable.</returns>
        public static IQueryable<T> AddFromSqlInterpolatedResult<T>(this IQueryable<T> mockedQueryable, FormattableString sql, IEnumerable<T> fromSqlInterpolatedResult)
            where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryable, nameof(mockedQueryable));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(fromSqlInterpolatedResult, nameof(fromSqlInterpolatedResult));

            mockedQueryable.Provider.AddFromSqlRawResult(sql.Format, sql.GetArguments(), fromSqlInterpolatedResult);
            return mockedQueryable;
        }

        /// <summary>Sets up FromSqlRaw invocations to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryable">The mocked queryable.</param>
        /// <param name="fromSqlRawResult">The FromSqlRaw result.</param>
        /// <returns>The mocked queryable.</returns>
        public static IQueryable<T> AddFromSqlRawResult<T>(this IQueryable<T> mockedQueryable, IEnumerable<T> fromSqlRawResult)
            where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryable, nameof(mockedQueryable));
            EnsureArgument.IsNotNull(fromSqlRawResult, nameof(fromSqlRawResult));

            mockedQueryable.Provider.AddFromSqlRawResult(string.Empty, new List<object>(), fromSqlRawResult);
            return mockedQueryable;
        }

        /// <summary>Sets up FromSqlRaw invocations containing a specified sql string to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryable">The mocked queryable.</param>
        /// <param name="sql">The FromSqlRaw sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="fromSqlRawResult">The FromSqlRaw result.</param>
        /// <returns>The mocked queryable.</returns>
        public static IQueryable<T> AddFromSqlRawResult<T>(this IQueryable<T> mockedQueryable, string sql, IEnumerable<T> fromSqlRawResult)
            where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryable, nameof(mockedQueryable));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(fromSqlRawResult, nameof(fromSqlRawResult));

            mockedQueryable.Provider.AddFromSqlRawResult(sql, new List<object>(), fromSqlRawResult);
            return mockedQueryable;
        }

        /// <summary>Sets up FromSqlRaw invocations containing a specified sql string and sql parameters to return a specified result.</summary>
        /// <typeparam name="T">The queryable source type.</typeparam>
        /// <param name="mockedQueryable">The mocked queryable.</param>
        /// <param name="sql">The FromSqlRaw sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The FromSqlRaw sql parameters. Set up supports case insensitive partial sql parameter sequence matching.</param>
        /// <param name="fromSqlRawResult">The FromSqlRaw result.</param>
        /// <returns>The mocked queryable.</returns>
        public static IQueryable<T> AddFromSqlRawResult<T>(this IQueryable<T> mockedQueryable, string sql, IEnumerable<object> parameters, IEnumerable<T> fromSqlRawResult)
            where T : class
        {
            EnsureArgument.IsNotNull(mockedQueryable, nameof(mockedQueryable));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));
            EnsureArgument.IsNotNull(fromSqlRawResult, nameof(fromSqlRawResult));

            mockedQueryable.Provider.AddFromSqlRawResult(sql, parameters, fromSqlRawResult);
            return mockedQueryable;
        }
    }
}