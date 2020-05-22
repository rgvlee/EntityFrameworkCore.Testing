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
using NSubstitute.Core;
using NSubstitute.Extensions;

namespace EntityFrameworkCore.Testing.NSubstitute.Helpers
{
    internal class MockedDbContextFactory<TDbContext> : BaseMockedDbContextFactory<TDbContext> where TDbContext : DbContext
    {
        public MockedDbContextFactory(MockedDbContextFactoryOptions<TDbContext> options) : base(options) { }

        public override TDbContext Create()
        {
            var mockedDbContext = (TDbContext) Substitute.For(new[] {
                    typeof(TDbContext),
                    typeof(IEnumerable<object>),
                    typeof(IDbContextDependencies),
                    typeof(IDbQueryCache),
                    typeof(IDbSetCache),
                    typeof(IInfrastructure<IServiceProvider>),
                    typeof(IDbContextPoolable)
                },
                ConstructorParameters.ToArray());

            var router = SubstitutionContext.Current.GetCallRouterFor(mockedDbContext);
            router.RegisterCustomCallHandlerFactory(state => new NoSetUpHandler());

            mockedDbContext.Add(Arg.Any<object>()).Returns(callInfo => DbContext.Add(callInfo.Arg<object>()));
            mockedDbContext.AddAsync(Arg.Any<object>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => DbContext.AddAsync(callInfo.Arg<object>(), callInfo.Arg<CancellationToken>()));
            mockedDbContext.When(x => x.AddRange(Arg.Any<object[]>())).Do(callInfo => DbContext.AddRange(callInfo.Arg<object[]>()));
            mockedDbContext.When(x => x.AddRange(Arg.Any<IEnumerable<object>>())).Do(callInfo => DbContext.AddRange(callInfo.Arg<IEnumerable<object>>()));
            mockedDbContext.AddRangeAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => DbContext.AddRangeAsync(callInfo.Arg<object[]>(), callInfo.Arg<CancellationToken>()));
            mockedDbContext.AddRangeAsync(Arg.Any<IEnumerable<object>>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => DbContext.AddRangeAsync(callInfo.Arg<IEnumerable<object>>(), callInfo.Arg<CancellationToken>()));

            mockedDbContext.Attach(Arg.Any<object>()).Returns(callInfo => DbContext.Attach(callInfo.Arg<object>()));
            mockedDbContext.When(x => x.AttachRange(Arg.Any<object[]>())).Do(callInfo => DbContext.AttachRange(callInfo.Arg<object[]>()));
            mockedDbContext.When(x => x.AttachRange(Arg.Any<IEnumerable<object>>())).Do(callInfo => DbContext.AttachRange(callInfo.Arg<IEnumerable<object>>()));

            ((IDbContextDependencies) mockedDbContext).ChangeDetector.Returns(callInfo => ((IDbContextDependencies) DbContext).ChangeDetector);
            mockedDbContext.ChangeTracker.Returns(callInfo => DbContext.ChangeTracker);
            mockedDbContext.Database.Returns(callInfo => DbContext.Database);
            mockedDbContext.When(x => x.Dispose()).Do(callInfo => DbContext.Dispose());
            ((IDbContextDependencies) mockedDbContext).EntityFinderFactory.Returns(callInfo => ((IDbContextDependencies) DbContext).EntityFinderFactory);
            ((IDbContextDependencies) mockedDbContext).EntityGraphAttacher.Returns(callInfo => ((IDbContextDependencies) DbContext).EntityGraphAttacher);
            mockedDbContext.Entry(Arg.Any<object>()).Returns(callInfo => DbContext.Entry(callInfo.Arg<object>()));

            mockedDbContext.FindAsync(Arg.Any<Type>(), Arg.Any<object[]>()).Returns(callInfo => DbContext.FindAsync(callInfo.Arg<Type>(), callInfo.Arg<object[]>()));
            mockedDbContext.FindAsync(Arg.Any<Type>(), Arg.Any<object[]>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => DbContext.FindAsync(callInfo.Arg<Type>(), callInfo.Arg<object[]>(), callInfo.Arg<CancellationToken>()));

            ((IDbQueryCache) mockedDbContext).GetOrAddQuery(Arg.Any<IDbQuerySource>(), Arg.Any<Type>())
                .Returns(callInfo => ((IDbQueryCache) DbContext).GetOrAddQuery(callInfo.Arg<IDbQuerySource>(), callInfo.Arg<Type>()));
            ((IDbSetCache) mockedDbContext).GetOrAddSet(Arg.Any<IDbSetSource>(), Arg.Any<Type>())
                .Returns(callInfo => ((IDbSetCache) DbContext).GetOrAddSet(callInfo.Arg<IDbSetSource>(), callInfo.Arg<Type>()));
            ((IDbContextDependencies) mockedDbContext).InfrastructureLogger.Returns(callInfo => ((IDbContextDependencies) DbContext).InfrastructureLogger);
            ((IInfrastructure<IServiceProvider>) mockedDbContext).Instance.Returns(callInfo => ((IInfrastructure<IServiceProvider>) DbContext).Instance);
            ((IDbContextDependencies) mockedDbContext).Model.Returns(callInfo => ((IDbContextDependencies) DbContext).Model);
            ((IDbContextDependencies) mockedDbContext).QueryProvider.Returns(callInfo => ((IDbContextDependencies) DbContext).QueryProvider);
            ((IDbContextDependencies) mockedDbContext).QuerySource.Returns(callInfo => ((IDbContextDependencies) DbContext).QuerySource);

            mockedDbContext.Remove(Arg.Any<object>()).Returns(callInfo => DbContext.Remove(callInfo.Arg<object>()));
            mockedDbContext.When(x => x.RemoveRange(Arg.Any<object[]>())).Do(callInfo => DbContext.RemoveRange(callInfo.Arg<object[]>()));
            mockedDbContext.When(x => x.RemoveRange(Arg.Any<IEnumerable<object>>())).Do(callInfo => DbContext.RemoveRange(callInfo.Arg<IEnumerable<object>>()));

            ((IDbContextPoolable) mockedDbContext).When(x => x.ResetState()).Do(callInfo => ((IDbContextPoolable) DbContext).ResetState());
            ((IDbContextPoolable) mockedDbContext).When(x => x.Resurrect(Arg.Any<DbContextPoolConfigurationSnapshot>()))
                .Do(callInfo => ((IDbContextPoolable) DbContext).Resurrect(callInfo.Arg<DbContextPoolConfigurationSnapshot>()));

            mockedDbContext.SaveChanges().Returns(callInfo => DbContext.SaveChanges());
            mockedDbContext.SaveChanges(Arg.Any<bool>()).Returns(callInfo => DbContext.SaveChanges(callInfo.Arg<bool>()));
            mockedDbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(callInfo => DbContext.SaveChangesAsync(callInfo.Arg<CancellationToken>()));
            mockedDbContext.SaveChangesAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => DbContext.SaveChangesAsync(callInfo.Arg<bool>(), callInfo.Arg<CancellationToken>()));

            ((IDbContextPoolable) mockedDbContext).When(x => x.SetPool(Arg.Any<IDbContextPool>()))
                .Do(callInfo => ((IDbContextPoolable) DbContext).SetPool(callInfo.Arg<IDbContextPool>()));
            ((IDbContextDependencies) mockedDbContext).SetSource.Returns(callInfo => ((IDbContextDependencies) DbContext).SetSource);
            ((IDbContextPoolable) mockedDbContext).SnapshotConfiguration().Returns(callInfo => ((IDbContextPoolable) DbContext).SnapshotConfiguration());
            ((IDbContextDependencies) mockedDbContext).StateManager.Returns(callInfo => ((IDbContextDependencies) DbContext).StateManager);

            mockedDbContext.Update(Arg.Any<object>()).Returns(callInfo => DbContext.Update(callInfo.Arg<object>()));

            ((IDbContextDependencies) mockedDbContext).UpdateLogger.Returns(callInfo => ((IDbContextDependencies) DbContext).UpdateLogger);

            mockedDbContext.When(x => x.UpdateRange(Arg.Any<object[]>())).Do(callInfo => DbContext.UpdateRange(callInfo.Arg<object[]>()));
            mockedDbContext.When(x => x.UpdateRange(Arg.Any<IEnumerable<object>>())).Do(callInfo => DbContext.UpdateRange(callInfo.Arg<IEnumerable<object>>()));

            foreach (var entity in DbContext.Model.GetEntityTypes().Where(x => !x.IsQueryType))
            {
                typeof(MockedDbContextFactory<TDbContext>).GetMethod(nameof(SetUpDbSetFor), BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(entity.ClrType)
                    .Invoke(this, new object[] { mockedDbContext });
            }

            foreach (var entity in DbContext.Model.GetEntityTypes().Where(x => x.IsQueryType))
            {
                typeof(MockedDbContextFactory<TDbContext>).GetMethod(nameof(SetUpDbQueryFor), BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(entity.ClrType)
                    .Invoke(this, new object[] { mockedDbContext });
            }

            return mockedDbContext;
        }

        private void SetUpDbSetFor<TEntity>(TDbContext mockedDbContext) where TEntity : class
        {
            var mockedDbSet = DbContext.Set<TEntity>().CreateMockedDbSet();

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

            mockedDbContext.Add(Arg.Any<TEntity>()).Returns(callInfo => DbContext.Add(callInfo.Arg<TEntity>()));
            mockedDbContext.AddAsync(Arg.Any<TEntity>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => DbContext.AddAsync(callInfo.Arg<TEntity>(), callInfo.Arg<CancellationToken>()));

            mockedDbContext.Attach(Arg.Any<TEntity>()).Returns(callInfo => DbContext.Attach(callInfo.Arg<TEntity>()));
            mockedDbContext.When(x => x.AttachRange(Arg.Any<object[]>())).Do(callInfo => DbContext.AttachRange(callInfo.Arg<object[]>()));
            mockedDbContext.When(x => x.AttachRange(Arg.Any<IEnumerable<object>>())).Do(callInfo => DbContext.AttachRange(callInfo.Arg<IEnumerable<object>>()));

            mockedDbContext.Entry(Arg.Any<TEntity>()).Returns(callInfo => DbContext.Entry(callInfo.Arg<TEntity>()));

            mockedDbContext.Find<TEntity>(Arg.Any<object[]>()).Returns(callInfo => DbContext.Find<TEntity>(callInfo.Arg<object[]>()));
            mockedDbContext.Find(typeof(TEntity), Arg.Any<object[]>()).Returns(callInfo => DbContext.Find(callInfo.Arg<Type>(), callInfo.Arg<object[]>()));
            mockedDbContext.FindAsync<TEntity>(Arg.Any<object[]>()).Returns(callInfo => DbContext.FindAsync<TEntity>(callInfo.Arg<object[]>()));
            mockedDbContext.FindAsync<TEntity>(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => DbContext.FindAsync<TEntity>(callInfo.Arg<object[]>(), callInfo.Arg<CancellationToken>()));

            mockedDbContext.Remove(Arg.Any<TEntity>()).Returns(callInfo => DbContext.Remove(callInfo.Arg<TEntity>()));

            mockedDbContext.Update(Arg.Any<TEntity>()).Returns(callInfo => DbContext.Update(callInfo.Arg<TEntity>()));
        }

        private void SetUpDbQueryFor<TQuery>(TDbContext mockedDbContext) where TQuery : class
        {
            var mockedDbQuery = DbContext.Query<TQuery>().CreateMockedDbQuery();

            var property = typeof(TDbContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof(DbQuery<TQuery>));

            if (property != null)
            {
                property.GetValue(mockedDbContext.Configure()).Returns(mockedDbQuery);
            }
            else
            {
                Logger.LogDebug($"Could not find a DbContext property for type '{typeof(TQuery)}'");
            }

            mockedDbContext.Configure().Query<TQuery>().Returns(callInfo => mockedDbQuery);
        }
    }
}