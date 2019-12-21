using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.Common.Extensions;
using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>Extensions for the db context type.</summary>
    public static partial class DbContextExtensions
    {
        /// <summary>Creates and sets up a substitute db context.</summary>
        /// <typeparam name="TDbContext">The db context type.</typeparam>
        /// <param name="dbContextToMock">The db context to mock/proxy.</param>
        /// <returns>A substitute db context.</returns>
        /// <remarks>dbContextToMock would typically be an in-memory database instance.</remarks>
        [Obsolete("This will be removed in a future version. Use DbContextExtensions.CreateDbContextSubstitute instead.")]
        public static TDbContext CreateMock<TDbContext>(this TDbContext dbContextToMock)
            where TDbContext : DbContext
        {
            return dbContextToMock.CreateSubstituteDbContext();
        }
    }
}