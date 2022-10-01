using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using rgvlee.Core.Common.Extensions;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.Common.Helpers
{
    /// <summary>
    ///     The base mocked db context factory.
    /// </summary>
    /// <typeparam name="TDbContext">The db context type.</typeparam>
    public abstract class BaseMockedDbContextFactory<TDbContext> where TDbContext : DbContext
    {
        /// <summary>
        ///     The logger instance.
        /// </summary>
        protected static readonly ILogger Logger = LoggingHelper.CreateLogger<BaseMockedDbContextFactory<TDbContext>>();

        /// <summary>
        ///     The parameters that will be used to create the mocked db context and, if one is not provided,
        ///     the in-memory context that the mocked db context will use for in-memory provider supported operations.
        /// </summary>
        protected readonly List<object> ConstructorParameters;

        /// <summary>
        ///     The db context instance that the mocked db context will use for in-memory provider supported operations.
        /// </summary>
        protected readonly TDbContext DbContext;

        /// <summary>
        ///     The db connection instance that the mocked db context will use in direct commands.
        /// </summary>
        protected readonly DbConnection DbConnection;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="options">The mocked db context factory options.</param>
        protected BaseMockedDbContextFactory(MockedDbContextFactoryOptions<TDbContext> options)
        {
            DbContext = options.DbContext;

            ConstructorParameters = options.ConstructorParameters?.ToList();

            if (ConstructorParameters == null || !ConstructorParameters.Any())
            {
                var dbContextType = typeof(TDbContext);

                if (!dbContextType.HasConstructor(typeof(DbContextOptions)) &&
                    !dbContextType.HasConstructor(typeof(DbContextOptions<TDbContext>)) &&
                    !dbContextType.HasParameterlessConstructor())
                {
                    throw new MissingMethodException(ExceptionMessages.UnableToFindSuitableDbContextConstructor);
                }

                if (DbContext != null && dbContextType.HasParameterlessConstructor())
                {
                    ConstructorParameters = new List<object>();
                }
                else if (!dbContextType.HasConstructor(typeof(DbContextOptions<>)))
                {
                    ConstructorParameters = new List<object> { new DbContextOptionsBuilder<TDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options };
                }
                else if (!dbContextType.HasConstructor(typeof(DbContextOptions)))
                {
                    ConstructorParameters = new List<object> { new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options };
                }
            }

            if (DbContext == null)
            {
                DbContext = (TDbContext) Activator.CreateInstance(typeof(TDbContext), ConstructorParameters?.ToArray());
            }

            if (options.DbConnection != null)
            {
                DbConnection = options.DbConnection;
            }
        }

        /// <summary>
        ///     Creates and sets up a mocked db context.
        /// </summary>
        /// <returns>A mocked db context.</returns>
        public abstract TDbContext Create();
    }
}