using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.Common.Helpers
{
    /// <summary>
    ///     The mocked db context builder.
    /// </summary>
    /// <typeparam name="TDbContext">The db context type.</typeparam>
    public abstract class BaseMockedDbContextBuilder<TDbContext> : IMockedDbContextBuilder<TDbContext> where TDbContext : DbContext
    {
        /// <summary>
        ///     The create factory options.
        /// </summary>
        protected readonly MockedDbContextFactoryOptions<TDbContext> Options = new MockedDbContextFactoryOptions<TDbContext>();

        /// <summary>
        ///     The mocked db context.
        /// </summary>
        public abstract TDbContext MockedDbContext { get; }

        /// <summary>
        ///     The parameters that will be used to create the mocked db context and, if one is not provided,
        ///     the in-memory context that the mocked db context will use for in-memory provider supported operations.
        /// </summary>
        /// <param name="constructorParameters">
        ///     The constructor parameters.
        /// </param>
        /// <returns>The mocked db context builder.</returns>
        public IMockedDbContextBuilder<TDbContext> UseConstructorWithParameters(params object[] constructorParameters)
        {
            EnsureArgument.IsNotEmpty(constructorParameters, nameof(constructorParameters));
            Options.ConstructorParameters = constructorParameters;
            return this;
        }

        /// <summary>
        ///     The db context instance that the mocked db context will use for in-memory provider supported operations.
        /// </summary>
        public IMockedDbContextBuilder<TDbContext> UseDbContext(TDbContext dbContext)
        {
            EnsureArgument.IsNotNull(dbContext, nameof(dbContext));
            Options.DbContext = dbContext;
            return this;
        }
    }
}