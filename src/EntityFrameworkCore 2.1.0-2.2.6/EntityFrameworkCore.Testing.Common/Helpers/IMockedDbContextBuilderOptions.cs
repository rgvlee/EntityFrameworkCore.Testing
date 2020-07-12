﻿using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.Common.Helpers
{
    /// <summary>
    ///     The mocked db context builder options.
    /// </summary>
    /// <typeparam name="TDbContext">The db context type.</typeparam>
    public interface IMockedDbContextBuilderOptions<TDbContext> : IMockedDbContextBuilder<TDbContext> where TDbContext : DbContext
    {
        /// <summary>
        ///     The parameters that will be used to create the mocked db context and, if one is not provided,
        ///     the in-memory context that the mocked db context will use for in-memory provider supported operations.
        /// </summary>
        /// <param name="constructorParameters">
        ///     The constructor parameters.
        /// </param>
        /// <returns>The mocked db context builder.</returns>
        IMockedDbContextBuilderFluentAnd<TDbContext> ConstructorWithParameters(params object[] constructorParameters);

        /// <summary>
        ///     The db context instance that the mocked db context will use for in-memory provider supported operations.
        /// </summary>
        IMockedDbContextBuilderFluentAnd<TDbContext> DbContext(TDbContext dbContext);
    }
}