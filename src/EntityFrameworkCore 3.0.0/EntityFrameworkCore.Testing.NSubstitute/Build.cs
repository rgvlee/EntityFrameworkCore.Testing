using EntityFrameworkCore.Testing.Common.Helpers;
using EntityFrameworkCore.Testing.NSubstitute.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.NSubstitute
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