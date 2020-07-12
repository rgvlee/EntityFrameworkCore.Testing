﻿using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.Common.Helpers
{
    public interface IMockedDbContextBuilderFluentAnd<TDbContext> : IMockedDbContextBuilder<TDbContext> where TDbContext : DbContext
    {
        IMockedDbContextBuilderOptions<TDbContext> And { get; }
    }
}