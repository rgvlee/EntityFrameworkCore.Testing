using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using EntityFrameworkCore.Testing.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    public static partial class ReadOnlyDbSetExtensions
    {
        internal static DbQuery<TEntity> CreateMockedReadOnlyDbSet<TEntity>(this DbSet<TEntity> readOnlyDbSet) where TEntity : class
        {
            EnsureArgument.IsNotNull(readOnlyDbSet, nameof(readOnlyDbSet));

            //This is deliberate; we cannot cast a Mock<DbSet<>> to a Mock<DbQuery<>> and we still need to support the latter
            var mockedDbQuery = (DbQuery<TEntity>) Substitute.For(new[] {
                    typeof(DbQuery<TEntity>),
                    typeof(IAsyncEnumerable<TEntity>),
                    typeof(IEnumerable),
                    typeof(IEnumerable<TEntity>),
                    typeof(IInfrastructure<IServiceProvider>),
                    typeof(IListSource),
                    typeof(IQueryable<TEntity>)
                },
                new object[] { });

            var asyncEnumerable = new AsyncEnumerable<TEntity>(new List<TEntity>());
            var mockedQueryProvider = ((IQueryable<TEntity>) readOnlyDbSet).Provider.CreateMockedQueryProvider(asyncEnumerable);

            var invalidOperationException = new InvalidOperationException(
                $"Unable to track an instance of type '{typeof(TEntity).Name}' because it does not have a primary key. Only entity types with primary keys may be tracked.");

            mockedDbQuery.Add(Arg.Any<TEntity>()).Throws(callInfo => invalidOperationException);
            mockedDbQuery.AddAsync(Arg.Any<TEntity>(), Arg.Any<CancellationToken>()).Throws(callInfo => invalidOperationException);
            mockedDbQuery.When(x => x.AddRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => throw invalidOperationException);
            mockedDbQuery.When(x => x.AddRange(Arg.Any<TEntity[]>())).Do(callInfo => throw invalidOperationException);
            mockedDbQuery.AddRangeAsync(Arg.Any<IEnumerable<TEntity>>(), Arg.Any<CancellationToken>()).Throws(callInfo => invalidOperationException);
            mockedDbQuery.AddRangeAsync(Arg.Any<TEntity[]>()).Throws(callInfo => invalidOperationException);

            mockedDbQuery.Attach(Arg.Any<TEntity>()).Throws(callInfo => invalidOperationException);
            mockedDbQuery.When(x => x.AttachRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => throw invalidOperationException);
            mockedDbQuery.When(x => x.AttachRange(Arg.Any<TEntity[]>())).Do(callInfo => throw invalidOperationException);

            ((IListSource) mockedDbQuery).ContainsListCollection.Returns(callInfo => false);

            ((IQueryable<TEntity>) mockedDbQuery).ElementType.Returns(callInfo => asyncEnumerable.ElementType);
            ((IQueryable<TEntity>) mockedDbQuery).Expression.Returns(callInfo => asyncEnumerable.Expression);

            mockedDbQuery.Find(Arg.Any<object[]>()).Throws(callInfo => new NullReferenceException());
            mockedDbQuery.FindAsync(Arg.Any<object[]>()).Throws(callInfo => new NullReferenceException());
            mockedDbQuery.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Throws(callInfo => new NullReferenceException());

            ((IAsyncEnumerable<TEntity>) mockedDbQuery).GetAsyncEnumerator(Arg.Any<CancellationToken>())
                .Returns(callInfo => asyncEnumerable.GetAsyncEnumerator(callInfo.Arg<CancellationToken>()));

            ((IEnumerable) mockedDbQuery).GetEnumerator().Returns(callInfo => ((IEnumerable) asyncEnumerable).GetEnumerator());
            ((IEnumerable<TEntity>) mockedDbQuery).GetEnumerator().Returns(callInfo => ((IEnumerable<TEntity>) asyncEnumerable).GetEnumerator());

            ((IListSource) mockedDbQuery).GetList().Returns(callInfo => asyncEnumerable.ToList());

            ((IInfrastructure<IServiceProvider>) mockedDbQuery).Instance.Returns(callInfo => ((IInfrastructure<IServiceProvider>) readOnlyDbSet).Instance);

            mockedDbQuery.Local.Throws(callInfo =>
                new InvalidOperationException($"The invoked method is cannot be used for the entity type '{typeof(TEntity).Name}' because it does not have a primary key."));

            mockedDbQuery.Remove(Arg.Any<TEntity>()).Throws(callInfo => invalidOperationException);
            mockedDbQuery.When(x => x.RemoveRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => throw invalidOperationException);
            mockedDbQuery.When(x => x.RemoveRange(Arg.Any<TEntity[]>())).Do(callInfo => throw invalidOperationException);

            mockedDbQuery.Update(Arg.Any<TEntity>()).Throws(callInfo => invalidOperationException);
            mockedDbQuery.When(x => x.UpdateRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => throw invalidOperationException);
            mockedDbQuery.When(x => x.UpdateRange(Arg.Any<TEntity[]>())).Do(callInfo => throw invalidOperationException);

            ((IQueryable<TEntity>) mockedDbQuery).Provider.Returns(callInfo => mockedQueryProvider);

            //Backwards compatibility implementation for EFCore 3.0.0
            var asyncEnumerableMethod = typeof(DbSet<TEntity>).GetMethod("AsAsyncEnumerable");
            if (asyncEnumerableMethod != null)
            {
                asyncEnumerableMethod.Invoke(mockedDbQuery.Configure(), null).Returns(callInfo => asyncEnumerable);
            }

            var queryableMethod = typeof(DbSet<TEntity>).GetMethod("AsQueryable");
            if (queryableMethod != null)
            {
                queryableMethod.Invoke(mockedDbQuery.Configure(), null).Returns(callInfo => asyncEnumerable);
            }

            return mockedDbQuery;
        }

        internal static void SetSource<TEntity>(this DbSet<TEntity> mockedReadOnlyDbSet, IEnumerable<TEntity> source) where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));
            EnsureArgument.IsNotNull(source, nameof(source));

            var asyncEnumerable = new AsyncEnumerable<TEntity>(source);
            var mockedQueryProvider = ((IQueryable<TEntity>) mockedReadOnlyDbSet).Provider;

            ((IQueryable<TEntity>) mockedReadOnlyDbSet).Expression.Returns(callInfo => asyncEnumerable.Expression);

            ((IAsyncEnumerable<TEntity>) mockedReadOnlyDbSet).GetAsyncEnumerator(Arg.Any<CancellationToken>())
                .Returns(callInfo => asyncEnumerable.GetAsyncEnumerator(callInfo.Arg<CancellationToken>()));

            ((IEnumerable) mockedReadOnlyDbSet).GetEnumerator().Returns(callInfo => ((IEnumerable) asyncEnumerable).GetEnumerator());
            ((IEnumerable<TEntity>) mockedReadOnlyDbSet).GetEnumerator().Returns(callInfo => ((IEnumerable<TEntity>) asyncEnumerable).GetEnumerator());

            ((IListSource) mockedReadOnlyDbSet).GetList().Returns(callInfo => asyncEnumerable.ToList());
            
            //Backwards compatibility implementation for EFCore 3.0.0
            var asyncEnumerableMethod = typeof(DbSet<TEntity>).GetMethod("AsAsyncEnumerable");
            if (asyncEnumerableMethod != null)
            {
                asyncEnumerableMethod.Invoke(mockedReadOnlyDbSet.Configure(), null).Returns(callInfo => asyncEnumerable);
            }

            var queryableMethod = typeof(DbSet<TEntity>).GetMethod("AsQueryable");
            if (queryableMethod != null)
            {
                queryableMethod.Invoke(mockedReadOnlyDbSet.Configure(), null).Returns(callInfo => asyncEnumerable);
            }

            ((AsyncQueryProvider<TEntity>) mockedQueryProvider).SetSource(asyncEnumerable);
        }
    }
}