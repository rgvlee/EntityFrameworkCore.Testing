#pragma warning disable EF1001 // Internal EF Core API usage.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using EntityFrameworkCore.Testing.Common.Helpers;
using EntityFrameworkCore.Testing.NSubstitute.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.Extensions;

namespace EntityFrameworkCore.Testing.NSubstitute.Helpers
{
    public class MockedDbContextFactory<TDbContext> : MockedDbContextFactoryBase<TDbContext> where TDbContext : DbContext
    {
        public MockedDbContextFactory(params object[] constructorParameters) : base(constructorParameters) { }

        /// <summary>Creates and sets up a mocked db context.</summary>
        /// <returns>A mocked db context.</returns>
        public override (TDbContext MockedDbContext, TDbContext DbContext) Create()
        {
            var dbContext = (TDbContext) Activator.CreateInstance(typeof(TDbContext), ConstructorParametersProvided ? ConstructorParameters : DefaultConstructorParameters);
            var mockedDbContext = (TDbContext)
                Substitute.For(new[] {
                        typeof(TDbContext),
                        typeof(IDbContextDependencies),
                        typeof(IDbSetCache),
                        typeof(IInfrastructure<IServiceProvider>),
                        typeof(IDbContextPoolable)
                    },
                    ConstructorParametersProvided ? ConstructorParameters : DefaultConstructorParameters
                );

            mockedDbContext.Add(Arg.Any<object>()).Returns(callInfo => dbContext.Add(callInfo.Arg<object>()));
            mockedDbContext.AddAsync(Arg.Any<object>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbContext.AddAsync(callInfo.Arg<object>(), callInfo.Arg<CancellationToken>()));
            mockedDbContext.When(x => x.AddRange(Arg.Any<object[]>())).Do(callInfo => dbContext.AddRange(callInfo.Arg<object[]>()));
            mockedDbContext.When(x => x.AddRange(Arg.Any<IEnumerable<object>>())).Do(callInfo => dbContext.AddRange(callInfo.Arg<IEnumerable<object>>()));
            mockedDbContext.AddRangeAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbContext.AddRangeAsync(callInfo.Arg<object[]>(), callInfo.Arg<CancellationToken>()));
            mockedDbContext.AddRangeAsync(Arg.Any<IEnumerable<object>>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbContext.AddRangeAsync(callInfo.Arg<object>(), callInfo.Arg<CancellationToken>()));

            mockedDbContext.Attach(Arg.Any<object>()).Returns(callInfo => dbContext.Attach(callInfo.Arg<object>()));
            mockedDbContext.When(x => x.AttachRange(Arg.Any<object[]>())).Do(callInfo => dbContext.AttachRange(callInfo.Arg<object[]>()));
            mockedDbContext.When(x => x.AttachRange(Arg.Any<IEnumerable<object>>())).Do(callInfo => dbContext.AttachRange(callInfo.Arg<IEnumerable<object>>()));

            ((IDbContextDependencies) mockedDbContext).ChangeDetector.Returns(callInfo => ((IDbContextDependencies) dbContext).ChangeDetector);
            mockedDbContext.ChangeTracker.Returns(callInfo => dbContext.ChangeTracker);
            mockedDbContext.ContextId.Returns(callInfo => dbContext.ContextId);
            mockedDbContext.Database.Returns(callInfo => dbContext.Database);
            mockedDbContext.When(x => x.Dispose()).Do(callInfo => dbContext.Dispose());
            mockedDbContext.DisposeAsync().Returns(callInfo => dbContext.DisposeAsync());
            ((IDbContextDependencies) mockedDbContext).EntityFinderFactory.Returns(callInfo => ((IDbContextDependencies) dbContext).EntityFinderFactory);
            ((IDbContextDependencies) mockedDbContext).EntityGraphAttacher.Returns(callInfo => ((IDbContextDependencies) dbContext).EntityGraphAttacher);
            mockedDbContext.Entry(Arg.Any<object>()).Returns(callInfo => dbContext.Entry(callInfo.Arg<object>()));

            mockedDbContext.Find(Arg.Any<Type>(), Arg.Any<object[]>()).Returns(callInfo => dbContext.Find(callInfo.Arg<Type>(), callInfo.Arg<object[]>()));
            mockedDbContext.FindAsync(Arg.Any<Type>(), Arg.Any<object[]>()).Returns(callInfo => dbContext.FindAsync(callInfo.Arg<Type>(), callInfo.Arg<object[]>()));
            mockedDbContext.FindAsync(Arg.Any<Type>(), Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbContext.FindAsync(callInfo.Arg<Type>(), callInfo.Arg<object[]>(), callInfo.Arg<CancellationToken>()));

            ((IDbSetCache) mockedDbContext).GetOrAddSet(Arg.Any<IDbSetSource>(), Arg.Any<Type>()).Returns(callInfo => ((IDbSetCache) dbContext).GetOrAddSet(callInfo.Arg<IDbSetSource>(), callInfo.Arg<Type>()));
            ((IDbContextDependencies) mockedDbContext).InfrastructureLogger.Returns(callInfo => ((IDbContextDependencies) dbContext).InfrastructureLogger);
            ((IInfrastructure<IServiceProvider>) mockedDbContext).Instance.Returns(callInfo => ((IInfrastructure<IServiceProvider>) dbContext).Instance);
            ((IDbContextDependencies) mockedDbContext).Model.Returns(callInfo => ((IDbContextDependencies) dbContext).Model);
            ((IDbContextDependencies) mockedDbContext).QueryProvider.Returns(callInfo => ((IDbContextDependencies) dbContext).QueryProvider);

            mockedDbContext.Remove(Arg.Any<object>()).Returns(callInfo => dbContext.Remove(callInfo.Arg<object>()));
            mockedDbContext.When(x => x.RemoveRange(Arg.Any<object[]>())).Do(callInfo => dbContext.RemoveRange(callInfo.Arg<object[]>()));
            mockedDbContext.When(x => x.RemoveRange(Arg.Any<IEnumerable<object>>())).Do(callInfo => dbContext.RemoveRange(callInfo.Arg<IEnumerable<object>>()));

            ((IDbContextPoolable) mockedDbContext).When(x => x.ResetState()).Do(callInfo => ((IDbContextPoolable) dbContext).ResetState());
            ((IDbContextPoolable) mockedDbContext).When(x => x.ResetStateAsync(Arg.Any<CancellationToken>())).Do(callInfo => ((IDbContextPoolable) dbContext).ResetStateAsync(callInfo.Arg<CancellationToken>()));
            ((IDbContextPoolable) mockedDbContext).When(x => x.Resurrect(Arg.Any<DbContextPoolConfigurationSnapshot>())).Do(callInfo => ((IDbContextPoolable) dbContext).Resurrect(callInfo.Arg<DbContextPoolConfigurationSnapshot>()));

            mockedDbContext.SaveChanges().Returns(callInfo => dbContext.SaveChanges());
            mockedDbContext.SaveChanges(Arg.Any<bool>()).Returns(callInfo => dbContext.SaveChanges(callInfo.Arg<bool>()));
            mockedDbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(callInfo => dbContext.SaveChangesAsync(callInfo.Arg<CancellationToken>()));
            mockedDbContext.SaveChangesAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbContext.SaveChangesAsync(callInfo.Arg<bool>(), callInfo.Arg<CancellationToken>()));

            ((IDbContextPoolable) mockedDbContext).When(x => x.SetPool(Arg.Any<IDbContextPool>())).Do(callInfo => ((IDbContextPoolable) dbContext).SetPool(callInfo.Arg<IDbContextPool>()));
            ((IDbContextDependencies) mockedDbContext).SetSource.Returns(callInfo => ((IDbContextDependencies) dbContext).SetSource);
            ((IDbContextPoolable) mockedDbContext).SnapshotConfiguration().Returns(callInfo => ((IDbContextPoolable) dbContext).SnapshotConfiguration());
            ((IDbContextDependencies) mockedDbContext).StateManager.Returns(callInfo => ((IDbContextDependencies) dbContext).StateManager);

            mockedDbContext.Update(Arg.Any<object>()).Returns(callInfo => dbContext.Update(callInfo.Arg<object>()));

            ((IDbContextDependencies) mockedDbContext).UpdateLogger.Returns(callInfo => ((IDbContextDependencies) dbContext).UpdateLogger);

            mockedDbContext.When(x => x.UpdateRange(Arg.Any<object[]>())).Do(callInfo => dbContext.UpdateRange(callInfo.Arg<object[]>()));
            mockedDbContext.When(x => x.UpdateRange(Arg.Any<IEnumerable<object>>())).Do(callInfo => dbContext.UpdateRange(callInfo.Arg<IEnumerable<object>>()));

            foreach (var entity in dbContext.Model.GetEntityTypes().Where(x => x.FindPrimaryKey() != null))
            {
                typeof(MockedDbContextFactory<TDbContext>)
                    .GetMethod(nameof(SetUpDbSetFor), BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(entity.ClrType).Invoke(this, new object[] {mockedDbContext, dbContext});
            }

            foreach (var entity in dbContext.Model.GetEntityTypes().Where(x => x.FindPrimaryKey() == null))
            {
                typeof(MockedDbContextFactory<TDbContext>)
                    .GetMethod(nameof(SetUpReadOnlyDbSetFor), BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(entity.ClrType).Invoke(this, new object[] {mockedDbContext, dbContext});
            }

            return (MockedDbContext: mockedDbContext, DbContext: dbContext);
        }

        private void SetUpDbSetFor<TEntity>(TDbContext mockedDbContext, TDbContext dbContext)
            where TEntity : class
        {
            var mockedDbSet = dbContext.Set<TEntity>().CreateMockedDbSet();

            var property = typeof(TDbContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof(DbSet<TEntity>));

            if (property != null)
            {
                property.GetValue(mockedDbContext.Configure()).Returns(mockedDbSet);
            }
            else
            {
                Logger.LogDebug($"Could not find a DbContext property for type '{typeof(TEntity)}'");
            }

            mockedDbContext.Configure().Set<TEntity>().Returns(callInfo => mockedDbSet);

            mockedDbContext.Add(Arg.Any<TEntity>()).Returns(callInfo => dbContext.Add(callInfo.Arg<TEntity>()));
            mockedDbContext.AddAsync(Arg.Any<TEntity>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbContext.AddAsync(callInfo.Arg<TEntity>(), callInfo.Arg<CancellationToken>()));

            mockedDbContext.Attach(Arg.Any<TEntity>()).Returns(callInfo => dbContext.Attach(callInfo.Arg<TEntity>()));
            mockedDbContext.When(x => x.AttachRange(Arg.Any<object[]>())).Do(callInfo => dbContext.AttachRange(callInfo.Arg<object[]>()));
            mockedDbContext.When(x => x.AttachRange(Arg.Any<IEnumerable<object>>())).Do(callInfo => dbContext.AttachRange(callInfo.Arg<IEnumerable<object>>()));

            mockedDbContext.Entry(Arg.Any<TEntity>()).Returns(callInfo => dbContext.Entry(callInfo.Arg<TEntity>()));

            mockedDbContext.Find<TEntity>(Arg.Any<object[]>()).Returns(callInfo => dbContext.Find<TEntity>(callInfo.Arg<object[]>()));
            mockedDbContext.Find(typeof(TEntity), Arg.Any<object[]>()).Returns(callInfo => dbContext.Find(callInfo.Arg<Type>(), callInfo.Arg<object[]>()));
            mockedDbContext.FindAsync<TEntity>(Arg.Any<object[]>()).Returns(callInfo => dbContext.FindAsync<TEntity>(callInfo.Arg<object[]>()));
            mockedDbContext.FindAsync<TEntity>(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(callInfo => dbContext.FindAsync<TEntity>(callInfo.Arg<object[]>(), callInfo.Arg<CancellationToken>()));

            mockedDbContext.Remove(Arg.Any<TEntity>()).Returns(callInfo => dbContext.Remove(callInfo.Arg<TEntity>()));

            mockedDbContext.Update(Arg.Any<TEntity>()).Returns(callInfo => dbContext.Update(callInfo.Arg<TEntity>()));
        }

        private void SetUpReadOnlyDbSetFor<TEntity>(TDbContext mockedDbContext, TDbContext dbContext)
            where TEntity : class
        {
            var mockedReadOnlyDbSet = dbContext.Set<TEntity>().CreateMockedReadOnlyDbSet();

            var property = typeof(TDbContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof(DbSet<TEntity>) || p.PropertyType == typeof(DbQuery<TEntity>));

            if (property != null)
            {
                property.GetValue(mockedDbContext.Configure()).Returns(mockedReadOnlyDbSet);
            }
            else
            {
                Logger.LogDebug($"Could not find a DbContext property for type '{typeof(TEntity)}'");
            }

            mockedDbContext.Configure().Set<TEntity>().Returns(callInfo => mockedReadOnlyDbSet);
            mockedDbContext.Configure().Query<TEntity>().Returns(callInfo => mockedReadOnlyDbSet);
        }
    }
}