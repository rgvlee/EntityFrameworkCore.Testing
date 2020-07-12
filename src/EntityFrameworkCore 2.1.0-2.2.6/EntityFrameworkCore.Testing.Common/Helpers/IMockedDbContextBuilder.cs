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
        ///     Creates the mocked db context.
        /// </summary>
        /// <returns>A mocked db context.</returns>
        TDbContext Build();
    }
}