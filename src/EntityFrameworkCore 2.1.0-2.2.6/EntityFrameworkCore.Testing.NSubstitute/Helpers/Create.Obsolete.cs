using System;
using System.Linq;
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
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.NSubstitute.Create.MockedDbContextFor instead.")]
        public static TDbContext SubstituteFor<TDbContext>(TDbContext dbContextToMock)
            where TDbContext : DbContext
        {
            return NSubstitute.Create.MockedDbContextFor(dbContextToMock);
        }

        /// <summary>Creates a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextToMock">The db context to mock.</param>
        /// <returns>A mocked db context.</returns>
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.NSubstitute.Create.MockedDbContextFor instead.")]
        public static TDbContext SubstituteDbContextFor<TDbContext>(TDbContext dbContextToMock)
            where TDbContext : DbContext
        {
            return NSubstitute.Create.MockedDbContextFor(dbContextToMock);
        }
        
        /// <summary>Creates a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <returns>A mocked db context.</returns>
        /// <remarks>TDbContext must have a constructor with a single parameter of type DbContextOptionsBuilder.</remarks>
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.NSubstitute.Create.MockedDbContextFor instead.")]
        public static TDbContext SubstituteDbContextFor<TDbContext>()
            where TDbContext : DbContext
        {
            return NSubstitute.Create.MockedDbContextFor<TDbContext>();
        }

        /// <summary>Creates a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="factory">A factory method that will create an instance of TDbContext.</param>
        /// <returns>A mocked db context.</returns>
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.NSubstitute.Create.MockedDbContextUsing instead.")]
        public static TDbContext SubstituteDbContextFor<TDbContext>(Func<TDbContext> factory)
            where TDbContext : DbContext
        {
            return NSubstitute.Create.MockedDbContextUsing(factory);
        }

        /// <summary>Creates a mocked db set.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbSetToMock">The db set to mock.</param>
        /// <returns>A mocked db set.</returns>
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.NSubstitute.Create.MockedDbSetFor instead.")]
        public static DbSet<TEntity> SubstituteFor<TEntity>(DbSet<TEntity> dbSetToMock)
            where TEntity : class
        {
            return NSubstitute.Create.MockedDbSetFor(dbSetToMock);
        }

        /// <summary>Creates a mocked db set.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbSetToMock">The db set to mock.</param>
        /// <returns>A mocked db set.</returns>
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.NSubstitute.Create.MockedDbSetFor instead.")]
        public static DbSet<TEntity> SubstituteDbSetFor<TEntity>(DbSet<TEntity> dbSetToMock)
            where TEntity : class
        {
            return NSubstitute.Create.MockedDbSetFor(dbSetToMock);
        }

        /// <summary>Creates a mocked db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQueryToMock">The db query to mock.</param>
        /// <returns>A mocked db query.</returns>
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.NSubstitute.Create.MockedDbQueryFor instead.")]
        public static DbQuery<TQuery> SubstituteFor<TQuery>(DbQuery<TQuery> dbQueryToMock)
            where TQuery : class
        {
            return NSubstitute.Create.MockedDbQueryFor(dbQueryToMock);
        }

        /// <summary>Creates a mocked db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQueryToMock">The db query to mock.</param>
        /// <returns>A mocked db query.</returns>
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.NSubstitute.Create.MockedDbQueryFor instead.")]
        public static DbQuery<TQuery> SubstituteDbQueryFor<TQuery>(DbQuery<TQuery> dbQueryToMock)
            where TQuery : class
        {
            return NSubstitute.Create.MockedDbQueryFor(dbQueryToMock);
        }

        /// <summary>Creates a mocked query provider.</summary>
        /// <typeparam name="T">The queryable type.</typeparam>
        /// <param name="queryable">The query provider source.</param>
        /// <returns>A mocked query provider.</returns>
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.NSubstitute.Create.MockedQueryProviderFor instead.")]
        public static IQueryProvider SubstituteFor<T>(IQueryable<T> queryable)
            where T : class
        {
            return NSubstitute.Create.MockedQueryProviderFor(queryable);
        }

        /// <summary>Creates a mocked query provider.</summary>
        /// <typeparam name="T">The queryable type.</typeparam>
        /// <param name="queryable">The query provider source.</param>
        /// <returns>A mocked query provider.</returns>
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.NSubstitute.Create.MockedQueryProviderFor instead.")]
        public static IQueryProvider SubstituteQueryProviderFor<T>(IQueryable<T> queryable)
            where T : class
        {
            return NSubstitute.Create.MockedQueryProviderFor(queryable);
        }
    }
}