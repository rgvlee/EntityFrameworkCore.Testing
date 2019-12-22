using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.Moq.Helpers
{
    /// <summary>Factory for creating mocked instances.</summary>
    [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.Moq.Create instead.")]
    public static class Create
    {
        /// <summary>Creates a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextToMock">The db context to mock.</param>
        /// <returns>A mocked db context.</returns>
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.Moq.Create.MockedDbContextFor instead.")]
        public static TDbContext MockedDbContextFor<TDbContext>(TDbContext dbContextToMock)
            where TDbContext : DbContext
        {
            return Moq.Create.MockedDbContextFor(dbContextToMock);
        }

        /// <summary>Creates a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <returns>A mocked db context.</returns>
        /// <remarks>TDbContext must have a constructor with a single parameter of type DbContextOptionsBuilder.</remarks>
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.Moq.Create.MockedDbContextFor instead.")]
        public static TDbContext MockedDbContextFor<TDbContext>()
            where TDbContext : DbContext
        {
            return Moq.Create.MockedDbContextFor<TDbContext>();
        }

        /// <summary>Creates a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="factory">A factory method that will create an instance of TDbContext.</param>
        /// <returns>A mocked db context.</returns>
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.Moq.Create.MockedDbContextUsing instead.")]
        public static TDbContext MockedDbContextFor<TDbContext>(Func<TDbContext> factory)
            where TDbContext : DbContext
        {
            return Moq.Create.MockedDbContextUsingResultFrom(factory);
        }

        /// <summary>Creates a mocked db set.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbSetToMock">The db set to mock.</param>
        /// <returns>A mocked db set.</returns>
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.Moq.Create.MockedDbSetFor instead.")]
        public static DbSet<TEntity> MockedDbSetFor<TEntity>(DbSet<TEntity> dbSetToMock)
            where TEntity : class
        {
            return Moq.Create.MockedDbSetFor(dbSetToMock);
        }

        /// <summary>Creates a mocked db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQueryToMock">The db query to mock.</param>
        /// <returns>A mocked db query.</returns>
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.Moq.Create.MockedDbQueryFor instead.")]
        public static DbQuery<TQuery> MockedDbQueryFor<TQuery>(DbQuery<TQuery> dbQueryToMock)
            where TQuery : class
        {
            return Moq.Create.MockedDbQueryFor(dbQueryToMock);
        }

        /// <summary>
        ///     Creates a mocked query provider.
        /// </summary>
        /// <typeparam name="T">The queryable type.</typeparam>
        /// <param name="queryable">The query provider source.</param>
        /// <returns>A mocked query provider.</returns>
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.Moq.Create.MockedQueryProviderFor instead.")]
        public static IQueryProvider MockedQueryProviderFor<T>(IQueryable<T> queryable)
            where T : class
        {
            return Moq.Create.MockedQueryProviderFor(queryable);
        }
    }
}