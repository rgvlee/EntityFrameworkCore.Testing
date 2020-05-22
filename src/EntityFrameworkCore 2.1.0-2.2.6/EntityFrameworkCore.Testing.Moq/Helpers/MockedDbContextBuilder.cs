﻿using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.Moq.Helpers
{
    /// <summary>
    ///     The mocked db context builder.
    /// </summary>
    /// <typeparam name="TDbContext">The db context type.</typeparam>
    public class MockedDbContextBuilder<TDbContext> : BaseMockedDbContextBuilder<TDbContext> where TDbContext : DbContext
    {
        internal MockedDbContextBuilder() { }

        /// <summary>
        ///     Creates the mocked db context.
        /// </summary>
        /// <returns>A mocked db context.</returns>
        public override TDbContext Create()
        {
            var factory = new MockedDbContextFactory<TDbContext>(Options);
            return factory.Create();
        }
    }
}