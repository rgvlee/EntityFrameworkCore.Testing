using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.Moq.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.Moq.Helpers
{
    /// <summary>Factory for creating mocked instances.</summary>
    public static class Create
    {
        /// <summary>Creates a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextToMock">The db context to mock.</param>
        /// <returns>A mocked db context.</returns>
        public static TDbContext MockedDbContextFor<TDbContext>(TDbContext dbContextToMock) where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(dbContextToMock, nameof(dbContextToMock));

            return dbContextToMock.CreateMock();
        }

        /// <summary>Creates a mocked db set.</summary>
        /// <typeparam name="TEntity">The db set entity type.</typeparam>
        /// <param name="dbSetToMock">The db set to mock.</param>
        /// <returns>A mocked db set.</returns>
        public static DbSet<TEntity> MockedDbSetFor<TEntity>(DbSet<TEntity> dbSetToMock) where TEntity : class
        {
            EnsureArgument.IsNotNull(dbSetToMock, nameof(dbSetToMock));

            return dbSetToMock.CreateMock();
        }

        /// <summary>Creates a mocked db query.</summary>
        /// <typeparam name="TQuery">The db query query type.</typeparam>
        /// <param name="dbQueryToMock">The db query to mock.</param>
        /// <returns>A mocked db query.</returns>
        public static DbQuery<TQuery> MockedDbQueryFor<TQuery>(DbQuery<TQuery> dbQueryToMock) where TQuery : class
        {
            EnsureArgument.IsNotNull(dbQueryToMock, nameof(dbQueryToMock));

            return dbQueryToMock.CreateMock();
        }
    }
}