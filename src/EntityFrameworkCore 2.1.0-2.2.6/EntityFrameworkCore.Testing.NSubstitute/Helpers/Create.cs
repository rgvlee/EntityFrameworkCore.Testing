using System;
using System.Linq;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.NSubstitute.Helpers
{
    /// <summary>Factory for creating mocked instances.</summary>
    public static class Create
    {
        /// <summary>Creates a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextToMock">The db context to mock.</param>
        /// <returns>A mocked db context.</returns>
        public static TDbContext SubstituteDbContextFor<TDbContext>(TDbContext dbContextToMock)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(dbContextToMock, nameof(dbContextToMock));

            return dbContextToMock.CreateDbContextSubstitute();
        }
        
        /// <summary>Creates a mocked db set.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbSetToMock">The db set to mock.</param>
        /// <returns>A mocked db set.</returns>
        public static DbSet<TEntity> SubstituteDbSetFor<TEntity>(DbSet<TEntity> dbSetToMock)
            where TEntity : class
        {
            EnsureArgument.IsNotNull(dbSetToMock, nameof(dbSetToMock));

            return dbSetToMock.CreateDbSetSubstitute();
        }
        
        /// <summary>Creates a mocked db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQueryToMock">The db query to mock.</param>
        /// <returns>A mocked db query.</returns>
        public static DbQuery<TQuery> SubstituteDbQueryFor<TQuery>(DbQuery<TQuery> dbQueryToMock)
            where TQuery : class
        {
            EnsureArgument.IsNotNull(dbQueryToMock, nameof(dbQueryToMock));

            return dbQueryToMock.CreateDbQuerySubstitute();
        }
        
        /// <summary>
        ///     Creates a mocked query provider.
        /// </summary>
        /// <typeparam name="T">The queryable type.</typeparam>
        /// <param name="queryable">The query provider source.</param>
        /// <returns>A mocked query provider.</returns>
        public static IQueryProvider SubstituteQueryProviderFor<T>(IQueryable<T> queryable)
            where T : class
        {
            EnsureArgument.IsNotNull(queryable, nameof(queryable));

            return queryable.Provider.CreateQueryProviderSubstitute(queryable);
        }

        /// <summary>Creates a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextToMock">The db context to mock.</param>
        /// <returns>A mocked db context.</returns>
        [Obsolete("This will be removed in a future version. Use Create.SubstituteDbContextFor instead.")]
        public static TDbContext SubstituteFor<TDbContext>(TDbContext dbContextToMock)
            where TDbContext : DbContext
        {
            return SubstituteDbContextFor(dbContextToMock);
        }

        /// <summary>Creates a mocked db set.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbSetToMock">The db set to mock.</param>
        /// <returns>A mocked db set.</returns>
        [Obsolete("This will be removed in a future version. Use Create.SubstituteDbSetFor instead.")]
        public static DbSet<TEntity> SubstituteFor<TEntity>(DbSet<TEntity> dbSetToMock)
            where TEntity : class
        {
            return SubstituteDbSetFor(dbSetToMock);
        }

        /// <summary>Creates a mocked db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQueryToMock">The db query to mock.</param>
        /// <returns>A mocked db query.</returns>
        [Obsolete("This will be removed in a future version. Use Create.SubstituteDbQueryFor instead.")]
        public static DbQuery<TQuery> SubstituteFor<TQuery>(DbQuery<TQuery> dbQueryToMock)
            where TQuery : class
        {
            return SubstituteDbQueryFor(dbQueryToMock);
        }

        /// <summary>
        ///     Creates a mocked query provider.
        /// </summary>
        /// <typeparam name="T">The queryable type.</typeparam>
        /// <param name="queryable">The query provider source.</param>
        /// <returns>A mocked query provider.</returns>
        [Obsolete("This will be removed in a future version. Use Create.SubstituteQueryProviderFor instead.")]
        public static IQueryProvider SubstituteFor<T>(IQueryable<T> queryable)
            where T : class
        {
            return SubstituteQueryProviderFor(queryable);
        }
    }
}