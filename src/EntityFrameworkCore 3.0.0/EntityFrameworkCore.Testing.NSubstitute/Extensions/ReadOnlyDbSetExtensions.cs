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
    public static partial class ReadOnlyDbSetExtensions
    {
        /// <summary>Creates and sets up a substitute readonly db set.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="readOnlyDbSet">The readonly db set to mock.</param>
        /// <returns>A substitute readonly db set.</returns>
        public static DbSet<TEntity> CreateSubstituteReadOnlyDbSet<TEntity>(this DbSet<TEntity> readOnlyDbSet)
            where TEntity : class
        {
            EnsureArgument.IsNotNull(readOnlyDbSet, nameof(readOnlyDbSet));

            //This is deliberate; we cannot cast a Mock<DbSet<>> to a Mock<DbQuery<>> and we still need to support the latter
            var substituteDbQuery = (DbQuery<TEntity>)
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

            substituteDbQuery.Add(Arg.Any<TEntity>()).Throws(callInfo => invalidOperationException);
            substituteDbQuery.AddAsync(Arg.Any<TEntity>(), Arg.Any<CancellationToken>()).Throws(callInfo => invalidOperationException);
            substituteDbQuery.When(x => x.AddRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => throw invalidOperationException);
            substituteDbQuery.When(x => x.AddRange(Arg.Any<TEntity[]>())).Do(callInfo => throw invalidOperationException);
            substituteDbQuery.AddRangeAsync(Arg.Any<IEnumerable<TEntity>>(), Arg.Any<CancellationToken>()).Throws(callInfo => invalidOperationException);
            substituteDbQuery.AddRangeAsync(Arg.Any<TEntity[]>()).Throws(callInfo => invalidOperationException);

            substituteDbQuery.Attach(Arg.Any<TEntity>()).Throws(callInfo => invalidOperationException);
            substituteDbQuery.When(x => x.AttachRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => throw invalidOperationException);
            substituteDbQuery.When(x => x.AttachRange(Arg.Any<TEntity[]>())).Do(callInfo => throw invalidOperationException);

            ((IListSource) substituteDbQuery).ContainsListCollection.Returns(callInfo => false);

            ((IQueryable<TEntity>) substituteDbQuery).ElementType.Returns(callInfo => queryable.ElementType);
            ((IQueryable<TEntity>) substituteDbQuery).Expression.Returns(callInfo => queryable.Expression);

            substituteDbQuery.Find(Arg.Any<object[]>()).Throws(callInfo => new NullReferenceException());
            substituteDbQuery.FindAsync(Arg.Any<object[]>()).Throws(callInfo => new NullReferenceException());
            substituteDbQuery.FindAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Throws(callInfo => new NullReferenceException());

            ((IAsyncEnumerable<TEntity>) substituteDbQuery).GetAsyncEnumerator(Arg.Any<CancellationToken>()).Returns(callInfo => ((IAsyncEnumerable<TEntity>) queryable).GetAsyncEnumerator(callInfo.Arg<CancellationToken>()));

            ((IEnumerable) substituteDbQuery).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());
            ((IEnumerable<TEntity>) substituteDbQuery).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());

            ((IListSource) substituteDbQuery).GetList().Returns(callInfo => queryable.ToList());

            ((IInfrastructure<IServiceProvider>) substituteDbQuery).Instance.Returns(callInfo => ((IInfrastructure<IServiceProvider>) readOnlyDbSet).Instance);

            substituteDbQuery.Local.Throws(callInfo => new InvalidOperationException($"The invoked method is cannot be used for the entity type '{typeof(TEntity).Name}' because it does not have a primary key."));

            substituteDbQuery.Remove(Arg.Any<TEntity>()).Throws(callInfo => invalidOperationException);
            substituteDbQuery.When(x => x.RemoveRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => throw invalidOperationException);
            substituteDbQuery.When(x => x.RemoveRange(Arg.Any<TEntity[]>())).Do(callInfo => throw invalidOperationException);

            substituteDbQuery.Update(Arg.Any<TEntity>()).Throws(callInfo => invalidOperationException);
            substituteDbQuery.When(x => x.UpdateRange(Arg.Any<IEnumerable<TEntity>>())).Do(callInfo => throw invalidOperationException);
            substituteDbQuery.When(x => x.UpdateRange(Arg.Any<TEntity[]>())).Do(callInfo => throw invalidOperationException);

            var substituteQueryProvider = ((IQueryable<TEntity>) readOnlyDbSet).Provider.CreateSubstituteQueryProvider(new List<TEntity>());
            ((IQueryable<TEntity>)substituteDbQuery).Provider.Returns(callInfo => substituteQueryProvider);

            return substituteDbQuery;
        }

        /// <summary>Adds an item to the end of the substitute readonly db set source.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="substituteReadOnlyDbSet">The substitute readonly db set.</param>
        /// <param name="item">The item to be added to the end of the substitute readonly db set source.</param>
        public static void AddToReadOnlySource<TEntity>(this DbQuery<TEntity> substituteReadOnlyDbSet, TEntity item)
            where TEntity : class
        {
            ((DbSet<TEntity>) substituteReadOnlyDbSet).AddToReadOnlySource(item);
        }

        /// <summary>Adds an item to the end of the substitute readonly db set source.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="substituteReadOnlyDbSet">The substitute readonly db set.</param>
        /// <param name="item">The item to be added to the end of the substitute readonly db set source.</param>
        public static void AddToReadOnlySource<TEntity>(this DbSet<TEntity> substituteReadOnlyDbSet, TEntity item)
            where TEntity : class
        {
            EnsureArgument.IsNotNull(substituteReadOnlyDbSet, nameof(substituteReadOnlyDbSet));
            EnsureArgument.IsNotNull(item, nameof(item));

            var list = substituteReadOnlyDbSet.ToList();
            list.Add(item);
            var queryable = list.AsQueryable();

            substituteReadOnlyDbSet.SetSource(queryable);
        }

        /// <summary>Adds the items of the specified sequence to the end of the substitute readonly db set source.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="substituteReadOnlyDbSet">The substitute readonly db set.</param>
        /// <param name="items">The sequence whose items should be added to the end of the substitute readonly db set source.</param>
        public static void AddRangeToReadOnlySource<TEntity>(this DbQuery<TEntity> substituteReadOnlyDbSet, IEnumerable<TEntity> items)
            where TEntity : class
        {
            ((DbSet<TEntity>) substituteReadOnlyDbSet).AddRangeToReadOnlySource(items);
        }

        /// <summary>Adds the items of the specified sequence to the end of the substitute readonly db set source.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="substituteReadOnlyDbSet">The substitute readonly db set.</param>
        /// <param name="items">The sequence whose items should be added to the end of the substitute readonly db set source.</param>
        public static void AddRangeToReadOnlySource<TEntity>(this DbSet<TEntity> substituteReadOnlyDbSet, IEnumerable<TEntity> items)
            where TEntity : class
        {
            EnsureArgument.IsNotNull(substituteReadOnlyDbSet, nameof(substituteReadOnlyDbSet));
            EnsureArgument.IsNotNull(items, nameof(items));
            EnsureArgument.IsNotEmpty(items, nameof(items));

            var list = substituteReadOnlyDbSet.ToList();
            list.AddRange(items);
            var queryable = list.AsQueryable();

            substituteReadOnlyDbSet.SetSource(queryable);
        }

        /// <summary>Removes all items from the substitute readonly db set source.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="substituteReadOnlyDbSet">The substitute readonly db set.</param>
        public static void ClearReadOnlySource<TEntity>(this DbQuery<TEntity> substituteReadOnlyDbSet)
            where TEntity : class
        {
            ((DbSet<TEntity>) substituteReadOnlyDbSet).ClearReadOnlySource();
        }

        /// <summary>Removes all items from the substitute readonly db set source.</summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="substituteReadOnlyDbSet">The substitute readonly db set.</param>
        public static void ClearReadOnlySource<TEntity>(this DbSet<TEntity> substituteReadOnlyDbSet)
            where TEntity : class
        {
            EnsureArgument.IsNotNull(substituteReadOnlyDbSet, nameof(substituteReadOnlyDbSet));

            substituteReadOnlyDbSet.SetSource(new List<TEntity>());
        }

        internal static void SetSource<TEntity>(this DbSet<TEntity> substituteReadOnlyDbSet, IEnumerable<TEntity> source)
            where TEntity : class
        {
            EnsureArgument.IsNotNull(substituteReadOnlyDbSet, nameof(substituteReadOnlyDbSet));
            EnsureArgument.IsNotNull(source, nameof(source));

            var queryable = source.AsQueryable();

            ((IQueryable<TEntity>) substituteReadOnlyDbSet).ElementType.Returns(callInfo => queryable.ElementType);
            ((IQueryable<TEntity>) substituteReadOnlyDbSet).Expression.Returns(callInfo => queryable.Expression);
            ((IAsyncEnumerable<TEntity>) substituteReadOnlyDbSet).GetAsyncEnumerator(Arg.Any<CancellationToken>()).Returns(callInfo => ((IAsyncEnumerable<TEntity>) queryable).GetAsyncEnumerator(callInfo.Arg<CancellationToken>()));
            ((IEnumerable) substituteReadOnlyDbSet).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());
            ((IEnumerable<TEntity>) substituteReadOnlyDbSet).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());

            var provider = ((IQueryable<TEntity>) substituteReadOnlyDbSet).Provider;
            ((AsyncQueryProvider<TEntity>) provider).SetSource(queryable);
        }
    }
}