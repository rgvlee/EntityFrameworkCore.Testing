﻿using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.Common.Helpers
{
    public interface IMockedDbContextBuilderFluentUsing<TDbContext> : IMockedDbContextBuilder<TDbContext> where TDbContext : DbContext
    {
        IMockedDbContextBuilderOptions<TDbContext> Using { get; }
    }
}