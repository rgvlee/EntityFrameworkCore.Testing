using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>Extensions for the db context type.</summary>
    public static partial class DbContextExtensions
    {
        /// <summary>Creates and sets up a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextToMock">The db context to mock/proxy.</param>
        /// <returns>A mocked db context.</returns>
        /// <remarks>dbContextToMock would typically be an in-memory database instance.</remarks>
        [Obsolete("This will be removed in a future version. Use DbContextExtensions.CreateMockedDbContext instead.")]
        public static TDbContext CreateMock<TDbContext>(this TDbContext dbContextToMock)
            where TDbContext : DbContext
        {
            return dbContextToMock.CreateMockedDbContext();
        }

        /// <summary>Creates and sets up a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextToMock">The db context to mock/proxy.</param>
        /// <returns>A mocked db context.</returns>
        /// <remarks>dbContextToMock would typically be an in-memory database instance.</remarks>
        [Obsolete("This will be removed in a future version. Use DbContextExtensions.CreateMockedDbContext instead.")]
        public static TDbContext CreateDbContextSubstitute<TDbContext>(this TDbContext dbContextToMock)
            where TDbContext : DbContext
        {
            return dbContextToMock.CreateMockedDbContext();
        }

        /// <summary>Creates and sets up a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextToMock">The db context to mock/proxy.</param>
        /// <returns>A mocked db context.</returns>
        /// <remarks>dbContextToMock would typically be an in-memory database instance.</remarks>
        [Obsolete("This will be removed in a future version. Use DbContextExtensions.CreateMockedDbContext instead.")]
        public static TDbContext CreateSubstituteDbContext<TDbContext>(this TDbContext dbContextToMock)
            where TDbContext : DbContext
        {
            return dbContextToMock.CreateMockedDbContext();
        }
    }
}