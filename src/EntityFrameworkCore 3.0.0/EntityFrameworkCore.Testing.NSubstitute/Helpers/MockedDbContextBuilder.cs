using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.NSubstitute.Helpers
{
    /// <summary>
    ///     The mocked db context builder.
    /// </summary>
    /// <typeparam name="TDbContext">The db context type.</typeparam>
    public class MockedDbContextBuilder<TDbContext> : IMockedDbContextBuilder<TDbContext> where TDbContext : DbContext
    {
        private readonly MockedDbContextFactoryOptions<TDbContext> _options = new MockedDbContextFactoryOptions<TDbContext>();

        internal MockedDbContextBuilder() { }

        /// <summary>
        ///     The parameters that will be used to create the mocked db context and, if one is not provided,
        ///     the in-memory context that the mocked db context will use for in-memory provider supported operations.
        /// </summary>
        /// <param name="constructorParameters">
        ///     The constructor parameters.
        /// </param>
        /// <returns>The mocked db context builder.</returns>
        public IMockedDbContextBuilder<TDbContext> UsingConstructorWithParameters(params object[] constructorParameters)
        {
            _options.ConstructorParameters = constructorParameters;
            return this;
        }

        /// <summary>
        ///     The db context instance that the mocked db context will use for in-memory provider supported operations.
        /// </summary>
        public IMockedDbContextBuilder<TDbContext> UsingDbContext(TDbContext dbContext)
        {
            _options.DbContext = dbContext;
            return this;
        }

        /// <summary>
        ///     Creates the mocked db context.
        /// </summary>
        /// <returns>A mocked db context.</returns>
        public TDbContext Create()
        {
            var factory = new MockedDbContextFactory<TDbContext>(_options);
            return factory.Create();
        }
    }
}