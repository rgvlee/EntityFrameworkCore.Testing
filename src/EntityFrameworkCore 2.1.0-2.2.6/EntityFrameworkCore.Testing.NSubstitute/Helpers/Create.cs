using System;
using System.Linq;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.NSubstitute.Helpers
{
    /// <summary>Factory for creating substitute instances.</summary>
    public static class Create
    {
        /// <summary>Creates a substitute db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextToMock">The db context to mock.</param>
        /// <returns>A substitute db context.</returns>
        public static TDbContext SubstituteDbContextFor<TDbContext>(TDbContext dbContextToMock)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(dbContextToMock, nameof(dbContextToMock));

            return dbContextToMock.CreateSubstituteDbContext();
        }

        /// <summary>Creates a substitute db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <returns>A substitute db context.</returns>
        /// <remarks>TDbContext must have a constructor with a single parameter of type DbContextOptionsBuilder.</remarks>
        public static TDbContext SubstituteDbContextFor<TDbContext>()
            where TDbContext : DbContext
        {
            var options = new DbContextOptionsBuilder<TDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var dbContextToMock = (TDbContext) Activator.CreateInstance(typeof(TDbContext), options);
            return dbContextToMock.CreateSubstituteDbContext();
        }

        /// <summary>Creates a substitute db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="factory">A factory method that will create an instance of TDbContext.</param>
        /// <returns>A substitute db context.</returns>
        public static TDbContext SubstituteDbContextFor<TDbContext>(Func<TDbContext> factory)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(factory, nameof(factory));

            var dbContextToMock = factory();
            return dbContextToMock.CreateSubstituteDbContext();
        }

        /// <summary>Creates a substitute db set.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbSetToMock">The db set to mock.</param>
        /// <returns>A substitute db set.</returns>
        public static DbSet<TEntity> SubstituteDbSetFor<TEntity>(DbSet<TEntity> dbSetToMock)
            where TEntity : class
        {
            EnsureArgument.IsNotNull(dbSetToMock, nameof(dbSetToMock));

            return dbSetToMock.CreateSubstituteDbSet();
        }

        /// <summary>Creates a substitute db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQueryToMock">The db query to mock.</param>
        /// <returns>A substitute db query.</returns>
        public static DbQuery<TQuery> SubstituteDbQueryFor<TQuery>(DbQuery<TQuery> dbQueryToMock)
            where TQuery : class
        {
            EnsureArgument.IsNotNull(dbQueryToMock, nameof(dbQueryToMock));

            return dbQueryToMock.CreateSubstituteDbQuery();
        }

        /// <summary>
        ///     Creates a substitute query provider.
        /// </summary>
        /// <typeparam name="T">The queryable type.</typeparam>
        /// <param name="queryable">The query provider source.</param>
        /// <returns>A substitute query provider.</returns>
        public static IQueryProvider SubstituteQueryProviderFor<T>(IQueryable<T> queryable)
            where T : class
        {
            EnsureArgument.IsNotNull(queryable, nameof(queryable));

            return queryable.Provider.CreateSubstituteQueryProvider(queryable);
        }

        /// <summary>Creates a substitute db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextToMock">The db context to mock.</param>
        /// <returns>A substitute db context.</returns>
        [Obsolete("This will be removed in a future version. Use Create.SubstituteDbContextFor instead.")]
        public static TDbContext SubstituteFor<TDbContext>(TDbContext dbContextToMock)
            where TDbContext : DbContext
        {
            return SubstituteDbContextFor(dbContextToMock);
        }

        /// <summary>Creates a substitute db set.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbSetToMock">The db set to mock.</param>
        /// <returns>A substitute db set.</returns>
        [Obsolete("This will be removed in a future version. Use Create.SubstituteDbSetFor instead.")]
        public static DbSet<TEntity> SubstituteFor<TEntity>(DbSet<TEntity> dbSetToMock)
            where TEntity : class
        {
            return SubstituteDbSetFor(dbSetToMock);
        }

        /// <summary>Creates a substitute db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQueryToMock">The db query to mock.</param>
        /// <returns>A substitute db query.</returns>
        [Obsolete("This will be removed in a future version. Use Create.SubstituteDbQueryFor instead.")]
        public static DbQuery<TQuery> SubstituteFor<TQuery>(DbQuery<TQuery> dbQueryToMock)
            where TQuery : class
        {
            return SubstituteDbQueryFor(dbQueryToMock);
        }

        /// <summary>
        ///     Creates a substitute query provider.
        /// </summary>
        /// <typeparam name="T">The queryable type.</typeparam>
        /// <param name="queryable">The query provider source.</param>
        /// <returns>A substitute query provider.</returns>
        [Obsolete("This will be removed in a future version. Use Create.SubstituteQueryProviderFor instead.")]
        public static IQueryProvider SubstituteFor<T>(IQueryable<T> queryable)
            where T : class
        {
            return SubstituteQueryProviderFor(queryable);
        }
    }
}