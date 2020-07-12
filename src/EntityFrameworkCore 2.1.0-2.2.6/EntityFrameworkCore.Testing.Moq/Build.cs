using EntityFrameworkCore.Testing.Common.Helpers;
using EntityFrameworkCore.Testing.Moq.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.Moq
{
    /// <summary>
    ///     Provides builders that create mocked instances.
    /// </summary>
    public class Build
    {
        internal Build() { }

        /// <summary>
        ///     Provides a mocked db context builder.
        /// </summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <returns>A mocked db context builder.</returns>
        public static IMockedDbContextBuilder<TDbContext> MockedDbContextFor<TDbContext>() where TDbContext : DbContext
        {
            return new MockedDbContextBuilder<TDbContext>();
        }
    }
}