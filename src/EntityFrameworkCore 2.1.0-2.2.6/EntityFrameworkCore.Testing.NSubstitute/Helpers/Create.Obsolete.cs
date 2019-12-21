using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.NSubstitute.Helpers
{
    /// <summary>Factory for creating substitute instances.</summary>
    public static partial class Create
    {
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