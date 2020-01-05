using System.Linq;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.Moq.Extensions;
using EntityFrameworkCore.Testing.Moq.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.Moq
{
    /// <summary>Factory for creating mocked instances.</summary>
    public static partial class Create
    {
        /// <summary>Creates a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="constructorParameters">The db context constructor parameters.</param>
        /// <returns>A mocked db context.</returns>
        /// <remarks>If you do not provide any constructor arguments this method attempt to create TDbContext via a constructor with a single DbContextOptionsBuilder parameter.</remarks>
        public static TDbContext MockedDbContextFor<TDbContext>(params object[] constructorParameters)
            where TDbContext : DbContext
        {
            return new MockedDbContextFactory<TDbContext>(constructorParameters).Create();
        }

        /// <summary>
        ///     Creates a mocked query provider.
        /// </summary>
        /// <typeparam name="T">The queryable type.</typeparam>
        /// <param name="queryable">The query provider source.</param>
        /// <returns>A mocked query provider.</returns>
        public static IQueryProvider MockedQueryProviderFor<T>(IQueryable<T> queryable)
            where T : class
        {
            EnsureArgument.IsNotNull(queryable, nameof(queryable));

            return queryable.Provider.CreateMockedQueryProvider(queryable);
        }
    }
}