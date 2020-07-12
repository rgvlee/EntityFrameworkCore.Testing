using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.Common.Helpers
{
    /// <summary>
    ///     The mocked db context builder fluent and joiner.
    /// </summary>
    /// <typeparam name="TDbContext">The db context type.</typeparam>
    public interface IMockedDbContextBuilderFluentAnd<TDbContext> : IMockedDbContextBuilder<TDbContext> where TDbContext : DbContext
    {
        IMockedDbContextBuilderOptions<TDbContext> And { get; }
    }
}