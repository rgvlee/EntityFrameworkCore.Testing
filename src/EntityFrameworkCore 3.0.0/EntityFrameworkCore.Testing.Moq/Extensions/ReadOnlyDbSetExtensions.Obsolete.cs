using System;
using EntityFrameworkCore.Testing.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    /// <summary>Extensions for the db set type.</summary>
    public static partial class ReadOnlyDbSetExtensions
    {
        /// <summary>Creates and sets up a mocked db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQuery">The db query to mock.</param>
        /// <returns>A mocked readonly db query.</returns>
        [Obsolete("This method will remain until EntityFrameworkCore no longer supports the DbQuery<TQuery> type. Use ReadOnlyDbSetExtensions.CreateMockedReadOnlyDbSet instead.")]
        public static DbQuery<TQuery> CreateMockedDbQuery<TQuery>(this DbQuery<TQuery> dbQuery)
            where TQuery : class
        {
            EnsureArgument.IsNotNull(dbQuery, nameof(dbQuery));
            return (DbQuery<TQuery>) dbQuery.CreateMockedReadOnlyDbSet();
        }

        /// <summary>Creates and sets up a mocked db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQuery">The db query to mock.</param>
        /// <returns>A mocked readonly db query.</returns>
        [Obsolete("This will be removed in a future version. Use ReadOnlyDbSetExtensions.CreateMockedDbQuery instead.")]
        public static DbQuery<TQuery> CreateReadOnlyMock<TQuery>(this DbQuery<TQuery> dbQuery)
            where TQuery : class
        {
            EnsureArgument.IsNotNull(dbQuery, nameof(dbQuery));
            return dbQuery.CreateMockedDbQuery();
        }

        /// <summary>Creates and sets up a mocked readonly db set.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="readOnlyDbSet">The readonly db set to mock.</param>
        /// <returns>A mocked readonly db set.</returns>
        [Obsolete("This will be removed in a future version. Use ReadOnlyDbSetExtensions.CreateMockedReadOnlyDbSet instead.")]
        public static DbSet<TEntity> CreateReadOnlyMock<TEntity>(this DbSet<TEntity> readOnlyDbSet)
            where TEntity : class
        {
            EnsureArgument.IsNotNull(readOnlyDbSet, nameof(readOnlyDbSet));
            return readOnlyDbSet.CreateMockedReadOnlyDbSet();
        }
    }
}