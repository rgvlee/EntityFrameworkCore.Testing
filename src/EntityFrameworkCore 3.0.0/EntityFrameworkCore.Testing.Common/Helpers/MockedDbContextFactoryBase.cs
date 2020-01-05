using System;
using System.Linq;
using EntityFrameworkCore.Testing.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Testing.Common.Helpers
{
    public abstract class MockedDbContextFactoryBase<TDbContext> where TDbContext : DbContext
    {
        protected static readonly ILogger Logger = LoggerHelper.CreateLogger(typeof(MockedDbContextFactoryBase<TDbContext>));

        protected readonly object[] ConstructorParameters;
        protected readonly TDbContext DbContextToMock;
        protected readonly object[] DefaultConstructorParameters;

        /// <summary>Constructor.</summary>
        /// <param name="constructorParameters">The db context constructor parameters.</param>
        protected MockedDbContextFactoryBase(params object[] constructorParameters)
        {
            ConstructorParameters = constructorParameters;

            if (!ConstructorParametersProvided)
            {
                var dbContextType = typeof(TDbContext);

                if (!dbContextType.HasConstructorWithParameterOfType(typeof(DbContextOptions)) &&
                    !dbContextType.HasConstructorWithParameterOfType(typeof(DbContextOptions<TDbContext>)) &&
                    !dbContextType.HasParameterlessConstructor())
                {
                    throw new MissingMethodException("Unable to find a suitable constructor. TDbContext must have a parameterless or DbContextOptions/DbContextOptions<TDbContext> constructor");
                }

                if (!dbContextType.HasConstructorWithParameterOfType(typeof(DbContextOptions<>)))
                {
                    DefaultConstructorParameters = new object[] {new DbContextOptionsBuilder<TDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options};
                }
                else if (!dbContextType.HasConstructorWithParameterOfType(typeof(DbContextOptions)))
                {
                    DefaultConstructorParameters = new object[] {new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options};
                }
            }

            DbContextToMock = (TDbContext) Activator.CreateInstance(typeof(TDbContext), ConstructorParametersProvided ? ConstructorParameters : DefaultConstructorParameters);
        }

        protected bool ConstructorParametersProvided => ConstructorParameters != null && ConstructorParameters.Any();

        /// <summary>Creates and sets up a mocked db context.</summary>
        /// <returns>A mocked db context.</returns>
        public abstract TDbContext Create();
    }
}