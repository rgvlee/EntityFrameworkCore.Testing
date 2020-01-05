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

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>Extensions for read-only db sets.</summary>
    public static class ReadOnlyDbSetExtensions
    {
        /// <summary>Creates and sets up a mocked readonly db set.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="readOnlyDbSet">The readonly db set to mock.</param>
        /// <returns>A mocked readonly db set.</returns>
        internal static DbSet<TEntity> CreateMockedReadOnlyDbSet<TEntity>(this DbSet<TEntity> readOnlyDbSet)
            where TEntity : class
        {
            EnsureArgument.IsNotNull(readOnlyDbSet, nameof(readOnlyDbSet));

            //This is deliberate; we cannot cast a Mock<DbSet<>> to a Mock<DbQuery<>> and we still need to support the latter
            var mockedDbQuery = (DbQuery<TEntity>)
                Substitute.For(
                    new[] {
                        typeof(DbQuery<TEntity>),
                        typeof(IAsyncEnumerable<TEntity>),
                        typeof(IEnumerable),
                        typeof(IEnumerable<TEntity>),
                        typeof(IInfrastructure<IServiceProvider>),
                        typeof(IListSource),
                        typeof(IQueryable<TEntity>)
                    },
                    new object[] { }
                );

            var queryable = new List<TEntity>().AsQueryable();

            var invalidOperationException = new InvalidOperationException($"Unable to track an instance of type '{typeof(TEntity).Name}' because it does not have a primary key. Only entity types with primary keys may be tracked.");

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

            ((IQueryable<TEntity>) mockedDbQuery).ElementType.Returns(callInfo => queryable.ElementType);
            ((IQueryable<TEntity>) mockedDbQuery).Expression.Returns(callInfo => queryable.Expression);

            mockedDbQuery.Find(Arg.Any<object[]>()).Throws(callInfo => new NullReferenceException());
            mockedDbQuery.FindAsync(Arg.Any<object[]>()).Throws(callInfo => new NullReferenceException());
            mockedDbQuery.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Throws(callInfo => new NullReferenceException());

            ((IAsyncEnumerable<TEntity>) mockedDbQuery).GetAsyncEnumerator(Arg.Any<CancellationToken>())
                .Returns(callInfo =>
                    {
                        return new AsyncEnumerable<TEntity>(queryable).GetAsyncEnumerator(callInfo.Arg<CancellationToken>());
                        //return ((IAsyncEnumerable<TEntity>)queryable).GetAsyncEnumerator(callInfo.Arg<CancellationToken>());
                    }
                );

            ((IEnumerable) mockedDbQuery).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());
            ((IEnumerable<TEntity>) mockedDbQuery).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());

            ((IListSource) mockedDbQuery).GetList().Returns(callInfo => queryable.ToList());

            ((IInfrastructure<IServiceProvider>) mockedDbQuery).Instance.Returns(callInfo => ((IInfrastructure<IServiceProvider>) readOnlyDbSet).Instance);

            mockedDbQuery.Local.Throws(callInfo => new InvalidOperationException($"The invoked method is cannot be used for the entity type '{typeof(TEntity).Name}' because it does not have a primary key."));

            mockedDbQuery.Remove(Arg.Any<TEntity>()).Throws(callInfo => invalidOperationException);
            mockedDbQuery.When(x => x.RemoveRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => throw invalidOperationException);
            mockedDbQuery.When(x => x.RemoveRange(Arg.Any<TEntity[]>())).Do(callInfo => throw invalidOperationException);

            mockedDbQuery.Update(Arg.Any<TEntity>()).Throws(callInfo => invalidOperationException);
            mockedDbQuery.When(x => x.UpdateRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => throw invalidOperationException);
            mockedDbQuery.When(x => x.UpdateRange(Arg.Any<TEntity[]>())).Do(callInfo => throw invalidOperationException);

            var mockedQueryProvider = ((IQueryable<TEntity>) readOnlyDbSet).Provider.CreateMockedQueryProvider(new List<TEntity>());
            ((IQueryable<TEntity>) mockedDbQuery).Provider.Returns(callInfo => mockedQueryProvider);

            return mockedDbQuery;
        }

        internal static void SetSource<TEntity>(this DbSet<TEntity> mockedReadOnlyDbSet, IEnumerable<TEntity> source)
            where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));
            EnsureArgument.IsNotNull(source, nameof(source));

            var queryable = source.AsQueryable();

            ((IQueryable<TEntity>) mockedReadOnlyDbSet).ElementType.Returns(callInfo => queryable.ElementType);
            ((IQueryable<TEntity>) mockedReadOnlyDbSet).Expression.Returns(callInfo => queryable.Expression);

            ((IAsyncEnumerable<TEntity>) mockedReadOnlyDbSet).GetAsyncEnumerator(Arg.Any<CancellationToken>())
                .Returns(callInfo =>
                    {
                        return new AsyncEnumerable<TEntity>(queryable).GetAsyncEnumerator(callInfo.Arg<CancellationToken>());
                        //return ((IAsyncEnumerable<TEntity>)queryable).GetAsyncEnumerator(callInfo.Arg<CancellationToken>());
                    }
                );

            ((IEnumerable) mockedReadOnlyDbSet).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());
            ((IEnumerable<TEntity>) mockedReadOnlyDbSet).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());

            var provider = ((IQueryable<TEntity>) mockedReadOnlyDbSet).Provider;
            ((AsyncQueryProvider<TEntity>) provider).SetSource(queryable);
        }

        /// <summary>Adds an item to the end of the mocked readonly db set source.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedReadOnlyDbSet">The mocked readonly db set.</param>
        /// <param name="item">The item to be added to the end of the mocked readonly db set source.</param>
        public static void AddToReadOnlySource<TEntity>(this DbQuery<TEntity> mockedReadOnlyDbSet, TEntity item)
            where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));
            ((DbSet<TEntity>) mockedReadOnlyDbSet).AddToReadOnlySource(item);
        }

        /// <summary>Adds an item to the end of the mocked readonly db set source.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedReadOnlyDbSet">The mocked readonly db set.</param>
        /// <param name="item">The item to be added to the end of the mocked readonly db set source.</param>
        public static void AddToReadOnlySource<TEntity>(this DbSet<TEntity> mockedReadOnlyDbSet, TEntity item)
            where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));
            EnsureArgument.IsNotNull(item, nameof(item));

            var list = mockedReadOnlyDbSet.ToList();
            list.Add(item);
            var queryable = list.AsQueryable();

            mockedReadOnlyDbSet.SetSource(queryable);
        }

        /// <summary>Adds the items of the specified sequence to the end of the mocked readonly db set source.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedReadOnlyDbSet">The mocked readonly db set.</param>
        /// <param name="items">The sequence whose items should be added to the end of the mocked readonly db set source.</param>
        public static void AddRangeToReadOnlySource<TEntity>(this DbQuery<TEntity> mockedReadOnlyDbSet, IEnumerable<TEntity> items)
            where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));
            ((DbSet<TEntity>) mockedReadOnlyDbSet).AddRangeToReadOnlySource(items);
        }

        /// <summary>Adds the items of the specified sequence to the end of the mocked readonly db set source.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedReadOnlyDbSet">The mocked readonly db set.</param>
        /// <param name="items">The sequence whose items should be added to the end of the mocked readonly db set source.</param>
        public static void AddRangeToReadOnlySource<TEntity>(this DbSet<TEntity> mockedReadOnlyDbSet, IEnumerable<TEntity> items)
            where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));
            EnsureArgument.IsNotEmpty(items, nameof(items));

            var list = mockedReadOnlyDbSet.ToList();
            list.AddRange(items);
            var queryable = list.AsQueryable();

            mockedReadOnlyDbSet.SetSource(queryable);
        }

        /// <summary>Removes all items from the mocked readonly db set source.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedReadOnlyDbSet">The mocked readonly db set.</param>
        public static void ClearReadOnlySource<TEntity>(this DbQuery<TEntity> mockedReadOnlyDbSet)
            where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));
            ((DbSet<TEntity>) mockedReadOnlyDbSet).ClearReadOnlySource();
        }

        /// <summary>Removes all items from the mocked readonly db set source.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="mockedReadOnlyDbSet">The mocked readonly db set.</param>
        public static void ClearReadOnlySource<TEntity>(this DbSet<TEntity> mockedReadOnlyDbSet)
            where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedReadOnlyDbSet, nameof(mockedReadOnlyDbSet));
            mockedReadOnlyDbSet.SetSource(new List<TEntity>());
        }
    }
}