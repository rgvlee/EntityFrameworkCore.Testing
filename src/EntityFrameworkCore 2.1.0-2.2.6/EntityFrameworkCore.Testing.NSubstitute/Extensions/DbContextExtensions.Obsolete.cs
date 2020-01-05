using System;
using System.Collections.Generic;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.NSubstitute.Helpers;
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
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.NSubstitute.Create.MockedDbContextFor with the params object[] parameter instead.")]
        public static TDbContext CreateMock<TDbContext>(this TDbContext dbContextToMock)
            where TDbContext : DbContext
        {
            return new MockedDbContextFactory<TDbContext>().Create();
        }

        /// <summary>Creates and sets up a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextToMock">The db context to mock/proxy.</param>
        /// <returns>A mocked db context.</returns>
        /// <remarks>dbContextToMock would typically be an in-memory database instance.</remarks>
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.NSubstitute.Create.MockedDbContextFor with the params object[] parameter instead.")]
        public static TDbContext CreateMockedDbContext<TDbContext>(this TDbContext dbContextToMock)
            where TDbContext : DbContext
        {
            return new MockedDbContextFactory<TDbContext>().Create();
        }

        /// <summary>Creates and sets up a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextToMock">The db context to mock/proxy.</param>
        /// <returns>A mocked db context.</returns>
        /// <remarks>dbContextToMock would typically be an in-memory database instance.</remarks>
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.NSubstitute.Create.MockedDbContextFor with the params object[] parameter instead.")]
        public static TDbContext CreateDbContextSubstitute<TDbContext>(this TDbContext dbContextToMock)
            where TDbContext : DbContext
        {
            return new MockedDbContextFactory<TDbContext>().Create();
        }

        /// <summary>Creates and sets up a mocked db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextToMock">The db context to mock/proxy.</param>
        /// <returns>A mocked db context.</returns>
        /// <remarks>dbContextToMock would typically be an in-memory database instance.</remarks>
        [Obsolete("This will be removed in a future version. Use EntityFrameworkCore.Testing.NSubstitute.Create.MockedDbContextFor with the params object[] parameter instead.")]
        public static TDbContext CreateSubstituteDbContext<TDbContext>(this TDbContext dbContextToMock)
            where TDbContext : DbContext
        {
            return new MockedDbContextFactory<TDbContext>().Create();
        }

        /// <summary>Sets up ExecuteSqlCommand invocations to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        [Obsolete("This will be removed in a future version. Use DbContextExtensions.AddExecuteSqlCommandResult with the Action<string, IEnumerable<object>> parameter instead.")]
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(this TDbContext mockedDbContext, int executeSqlCommandResult, Action callback)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            return mockedDbContext.AddExecuteSqlCommandResult(executeSqlCommandResult, (providedSql, providedParameters) => callback?.Invoke());
        }

        /// <summary>Sets up ExecuteSqlCommand invocations containing a specified sql string to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlCommand sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        [Obsolete("This will be removed in a future version. Use DbContextExtensions.AddExecuteSqlCommandResult with the Action<string, IEnumerable<object>> parameter instead.")]
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(this TDbContext mockedDbContext, string sql, int executeSqlCommandResult, Action callback)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            return mockedDbContext.AddExecuteSqlCommandResult(sql, executeSqlCommandResult, (providedSql, providedParameters) => callback?.Invoke());
        }

        /// <summary>Sets up ExecuteSqlCommand invocations containing a specified sql string and parameters to return a specified result.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="mockedDbContext">The mocked db context.</param>
        /// <param name="sql">The ExecuteSqlCommand sql string. Set up supports case insensitive partial matches.</param>
        /// <param name="parameters">The ExecuteSqlCommand parameters. Set up supports case insensitive partial parameter sequence matching.</param>
        /// <param name="executeSqlCommandResult">The integer to return when ExecuteSqlCommand is invoked.</param>
        /// <param name="callback">Operations to perform after ExecuteSqlCommand is invoked.</param>
        /// <returns>The mocked db context.</returns>
        [Obsolete("This will be removed in a future version. Use DbContextExtensions.AddExecuteSqlCommandResult with the Action<string, IEnumerable<object>> parameter instead.")]
        public static TDbContext AddExecuteSqlCommandResult<TDbContext>(this TDbContext mockedDbContext, string sql, IEnumerable<object> parameters, int executeSqlCommandResult, Action callback)
            where TDbContext : DbContext
        {
            EnsureArgument.IsNotNull(mockedDbContext, nameof(mockedDbContext));
            return mockedDbContext.AddExecuteSqlCommandResult(sql, parameters, executeSqlCommandResult, (providedSql, providedParameters) => callback?.Invoke());
        }
    }
}