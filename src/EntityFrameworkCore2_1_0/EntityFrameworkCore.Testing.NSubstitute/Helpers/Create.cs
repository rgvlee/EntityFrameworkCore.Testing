using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.NSubstitute.Helpers
{
    /// <summary>Factory for creating mocked instances.</summary>
    public static class Create
    {
        /// <summary>Creates a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextToMock">The db context to mock.</param>
        /// <returns>A mocked db context.</returns>
        public static TDbContext SubstituteFor<TDbContext>(TDbContext dbContextToMock) where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(dbContextToMock, nameof(dbContextToMock));

            return dbContextToMock.CreateMock();
        }

        /// <summary>Creates a mocked db set.</summary>
        /// <typeparam name="TEntity">The db set entity type.</typeparam>
        /// <param name="dbSetToMock">The db set to mock.</param>
        /// <returns>A mocked db set.</returns>
        public static DbSet<TEntity> SubstituteFor<TEntity>(DbSet<TEntity> dbSetToMock) where TEntity : class
        {
            EnsureArgument.IsNotNull(dbSetToMock, nameof(dbSetToMock));

            return dbSetToMock.CreateMock();
        }

        /// <summary>Creates a mocked db query.</summary>
        /// <typeparam name="TQuery">The db query query type.</typeparam>
        /// <param name="dbQueryToMock">The db query to mock.</param>
        /// <returns>A mocked db query.</returns>
        public static DbQuery<TQuery> SubstituteFor<TQuery>(DbQuery<TQuery> dbQueryToMock) where TQuery : class
        {
            EnsureArgument.IsNotNull(dbQueryToMock, nameof(dbQueryToMock));

            return dbQueryToMock.CreateMock();
        }
    }
}