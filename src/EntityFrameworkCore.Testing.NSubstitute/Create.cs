using System.Linq;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using EntityFrameworkCore.Testing.NSubstitute.Helpers;
using Microsoft.EntityFrameworkCore;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.NSubstitute
{
    /// <summary>
    ///     Factory for creating mocked instances.
    /// </summary>
    public static class Create
    {
        /// <summary>
        ///     Creates a mocked db context.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="constructorParameters">
        ///     The parameters that will be used to create the mocked db context and, if one is not provided,
        ///     the in-memory context that the mocked db context will use for in-memory provider supported operations.
        /// </param>
        /// <returns>A mocked db context.</returns>
        /// <remarks>
        ///     If you do not provide any constructor arguments this method attempt to create a TDbContext
        ///     via a constructor with a single DbContextOptionsBuilder parameter or a parameterless constructor.
        /// </remarks>
        public static TDbContext MockedDbContextFor<TDbContext>(params object[] constructorParameters) where TDbContext : DbContext
        {
            return constructorParameters != null && constructorParameters.Any()
                ? new MockedDbContextBuilder<TDbContext>().UseConstructorWithParameters(constructorParameters).MockedDbContext
                : new MockedDbContextBuilder<TDbContext>().MockedDbContext;
        }

        /// <summary>
        ///     Creates a mocked query provider.
        /// </summary>
        /// <typeparam name="T">The queryable type.</typeparam>
        /// <param name="queryable">The query provider source.</param>
        /// <returns>A mocked query provider.</returns>
        public static IQueryProvider MockedQueryProviderFor<T>(IQueryable<T> queryable) where T : class
        {
            EnsureArgument.IsNotNull(queryable, nameof(queryable));

            return queryable.Provider.CreateMockedQueryProvider(queryable);
        }
    }
}