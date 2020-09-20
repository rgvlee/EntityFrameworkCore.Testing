using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.Common.Helpers
{
    /// <summary>
    ///     The mocked db context builder.
    /// </summary>
    /// <typeparam name="TDbContext">The db context type.</typeparam>
    public interface IMockedDbContextBuilder<TDbContext> where TDbContext : DbContext
    {
        /// <summary>
        ///     The mocked db context.
        /// </summary>
        TDbContext MockedDbContext { get; }

        /// <summary>
        ///     The parameters that will be used to create the mocked db context and, if one is not provided,
        ///     the in-memory context that the mocked db context will use for in-memory provider supported operations.
        /// </summary>
        /// <param name="constructorParameters">
        ///     The constructor parameters.
        /// </param>
        /// <returns>The mocked db context builder.</returns>
        IMockedDbContextBuilder<TDbContext> UseConstructorWithParameters(params object[] constructorParameters);

        /// <summary>
        ///     The db context instance that the mocked db context will use for in-memory provider supported operations.
        /// </summary>
        IMockedDbContextBuilder<TDbContext> UseDbContext(TDbContext dbContext);
    }
}