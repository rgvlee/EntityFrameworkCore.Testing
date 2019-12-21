﻿using System;
using System.Collections.Generic;
using EntityFrameworkCore.Testing.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>Extensions for the db context type.</summary>
    public static partial class DbContextExtensions
    {
        /// <summary>Sets up ExecuteSqlCommand invocations to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="substituteDbContext">The substitute db context.</param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The substitute db context.</returns>
        [Obsolete("This method will remain until EntityFrameworkCore no longer supports DbContext.Database.ExecuteSqlCommand(RawSqlString sql, params object[] parameters) method. Use DbContextExtensions.AddExecuteSqlRawResult instead.")]
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(this TDbContext substituteDbContext, int executeSqlCommandResult, Action callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(substituteDbContext, nameof(substituteDbContext));

            return substituteDbContext.AddExecuteSqlRawResult(string.Empty, new List<object>(), executeSqlCommandResult, callback);
        }

        /// <summary>Sets up ExecuteSqlCommand invocations containing a specified sql string to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="substituteDbContext">The substitute db context.</param>
        /// <param name="sql">The ExecuteSqlCommand sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The substitute db context.</returns>
        [Obsolete("This method will remain until EntityFrameworkCore no longer supports DbContext.Database.ExecuteSqlCommand(RawSqlString sql, params object[] parameters) method. Use DbContextExtensions.AddExecuteSqlRawResult instead.")]
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(this TDbContext substituteDbContext, string sql, int executeSqlCommandResult, Action callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(substituteDbContext, nameof(substituteDbContext));
            EnsureArgument.IsNotNull(sql, nameof(sql));

            return substituteDbContext.AddExecuteSqlRawResult(sql, new List<object>(), executeSqlCommandResult, callback);
        }

        /// <summary>Sets up ExecuteSqlCommand invocations containing a specified sql string and parameters to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="substituteDbContext">The substitute db context.</param>
        /// <param name="sql">The ExecuteSqlCommand sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The ExecuteSqlCommand parameters. Set up supports case insensitive partial parameter sequence matching.</param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The substitute db context.</returns>
        [Obsolete("This method will remain until EntityFrameworkCore no longer supports DbContext.Database.ExecuteSqlCommand(RawSqlString sql, params object[] parameters) method. Use DbContextExtensions.AddExecuteSqlRawResult instead.")]
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(this TDbContext substituteDbContext, string sql, IEnumerable<object> parameters, int executeSqlCommandResult, Action callback = null)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(substituteDbContext, nameof(substituteDbContext));
            EnsureArgument.IsNotNull(sql, nameof(sql));
            EnsureArgument.IsNotNull(parameters, nameof(parameters));

            return substituteDbContext.AddExecuteSqlRawResult(sql, parameters, executeSqlCommandResult, callback);
        }
    }
}