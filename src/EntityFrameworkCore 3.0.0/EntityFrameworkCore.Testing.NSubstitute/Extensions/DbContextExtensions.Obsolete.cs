﻿using System;
using System.Collections.Generic;
using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    public static partial class DbContextExtensions
    {
        /// <summary>
        ///     Sets up ExecuteSqlCommand invocations to return a specified result.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        [Obsolete(
            "This method will remain until EntityFrameworkCore no longer supports DbContext.Database.ExecuteSqlCommand(RawSqlString sql, params object[] parameters) method. Use DbContextExtensions.AddExecuteSqlRawResult instead.")]
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(
            this TDbContext mockedDbContext, int executeSqlCommandResult, Action<string, IEnumerable<object>> callback = null) where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            return mockedDbContext.AddExecuteSqlRawResult(executeSqlCommandResult, callback);
        }

        /// <summary>
        ///     Sets up ExecuteSqlCommand invocations containing a specified sql string to return a specified result.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlCommand sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        [Obsolete(
            "This method will remain until EntityFrameworkCore no longer supports DbContext.Database.ExecuteSqlCommand(RawSqlString sql, params object[] parameters) method. Use DbContextExtensions.AddExecuteSqlRawResult instead.")]
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(
            this TDbContext mockedDbContext, string sql, int executeSqlCommandResult, Action<string, IEnumerable<object>> callback = null) where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            return mockedDbContext.AddExecuteSqlRawResult(sql, executeSqlCommandResult, callback);
        }

        /// <summary>
        ///     Sets up ExecuteSqlCommand invocations containing a specified sql string and parameters to return a specified result.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlCommand sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The ExecuteSqlCommand parameters. Set up supports case insensitive partial parameter sequence matching.</param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        [Obsolete(
            "This method will remain until EntityFrameworkCore no longer supports DbContext.Database.ExecuteSqlCommand(RawSqlString sql, params object[] parameters) method. Use DbContextExtensions.AddExecuteSqlRawResult instead.")]
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(
            this TDbContext mockedDbContext, string sql, IEnumerable<object> parameters, int executeSqlCommandResult, Action<string, IEnumerable<object>> callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            return mockedDbContext.AddExecuteSqlRawResult(sql, parameters, executeSqlCommandResult, callback);
        }
    }
}