using System;
using System.Linq;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
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

        /// <summary>Creates a substitute readonly db set.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="readOnlyDbSet">The readonly db set to mock.</param>
        /// <returns>A substitute readonly db set.</returns>
        public static DbSet<TEntity> SubstituteReadOnlyDbSetFor<TEntity>(DbSet<TEntity> readOnlyDbSet)
            where TEntity : class
        {
            EnsureArgument.IsNotNull(readOnlyDbSet, nameof(readOnlyDbSet));

            return readOnlyDbSet.CreateSubstituteReadOnlyDbSet();
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
    }
}