﻿using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>Extensions for the db set type.</summary>
    public static partial class DbSetExtensions
    {
        /// <summary>Creates and sets up a substitute db set.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbSet">The db set to mock/proxy.</param>
        /// <returns>A substitute db set.</returns>
        [Obsolete("This will be removed in a future version. Use DbSetExtensions.CreateDbSetSubstitute instead.")]
        public static DbSet<TEntity> CreateMock<TEntity>(this DbSet<TEntity> dbSet)
            where TEntity : class
        {
            return dbSet.CreateSubstituteDbSet();
        }
    }
}