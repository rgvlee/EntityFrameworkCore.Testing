using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Testing.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NSubstitute;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    /// <summary>Extensions for the db query type.</summary>
    public static class DbQueryExtensions
    {
        /// <summary>Creates and sets up a mocked db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQuery">The db query to mock.</param>
        /// <returns>A mocked db query.</returns>
        public static DbQuery<TQuery> CreateDbQuerySubstitute<TQuery>(this DbQuery<TQuery> dbQuery) 
            where TQuery : class
        {
            EnsureArgument.IsNotNull(dbQuery, nameof(dbQuery));

            var mockedDbQuery = (DbQuery<TQuery>)
                Substitute.For(new[] {
                        typeof(DbQuery<TQuery>),
                        typeof(IAsyncEnumerableAccessor<TQuery>),
                        typeof(IEnumerable),
                        typeof(IEnumerable<TQuery>),
                        typeof(IInfrastructure<IServiceProvider>),
                        typeof(IQueryable<TQuery>)
                    },
                    new object[] { }
                );

            var queryable = new List<TQuery>().AsQueryable();

            ((IAsyncEnumerableAccessor<TQuery>) mockedDbQuery).AsyncEnumerable.Returns(queryable.ToAsyncEnumerable());
            ((IQueryable<TQuery>) mockedDbQuery).ElementType.Returns(queryable.ElementType);
            ((IQueryable<TQuery>) mockedDbQuery).Expression.Returns(queryable.Expression);
            ((IEnumerable) mockedDbQuery).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());
            ((IEnumerable<TQuery>) mockedDbQuery).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());

            ((IInfrastructure<IServiceProvider>) mockedDbQuery).Instance.Returns(((IInfrastructure<IServiceProvider>) mockedDbQuery).Instance);

            var mockedQueryProvider = ((IQueryable<TQuery>) mockedDbQuery).Provider.CreateQueryProviderSubstitute(new List<TQuery>());
            ((IQueryable<TQuery>) mockedDbQuery).Provider.Returns(mockedQueryProvider);

            return mockedDbQuery;
        }

        /// <summary>Creates and sets up a mocked db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQuery">The db query to mock.</param>
        /// <returns>A mocked db query.</returns>
        [Obsolete("This will be removed in a future version. Use DbQueryExtensions.CreateDbQuerySubstitute instead.")]
        public static DbQuery<TQuery> CreateMock<TQuery>(this DbQuery<TQuery> dbQuery)
            where TQuery : class
        {
            return dbQuery.CreateDbQuerySubstitute();
        }

        /// <summary>Adds an item to the end of the mocked db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        /// <param name="item">The item to be added to the end of the mocked db query source.</param>
        [Obsolete(
            "This has been replaced by DbQuery<TQuery>.AddToReadOnlySource(TQuery item) to avoid conflicts with the " +
            "EntityFrameworkCore 3.0.0 read only set Add(TEntity entity) method.")
        ]
        public static void Add<TQuery>(this DbQuery<TQuery> mockedDbQuery, TQuery item) 
            where TQuery : class
        {
            mockedDbQuery.AddToReadOnlySource(item);
        }

        /// <summary>Adds an item to the end of the mocked db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        /// <param name="item">The item to be added to the end of the mocked db query source.</param>
        public static void AddToReadOnlySource<TQuery>(this DbQuery<TQuery> mockedDbQuery, TQuery item) 
            where TQuery : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));
            EnsureArgument.IsNotNull(item, nameof(item));

            var list = mockedDbQuery.ToList();
            list.Add(item);
            var queryable = list.AsQueryable();

            mockedDbQuery.SetSource(queryable);
        }

        /// <summary>Adds the items of the specified sequence to the end of the mocked db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        /// <param name="items">The sequence whose items should be added to the end of the mocked db query source.</param>
        [Obsolete(
            "This has been replaced by DbQuery<TQuery>.AddRangeToReadOnlySource(IEnumerable<TQuery> items) to avoid conflicts with the " +
            "EntityFrameworkCore 3.0.0 read only set AddRange(IEnumerable<TEntity> entities) method.")
        ]
        public static void AddRange<TQuery>(this DbQuery<TQuery> mockedDbQuery, IEnumerable<TQuery> items) 
            where TQuery : class
        {
            mockedDbQuery.AddRangeToReadOnlySource(items);
        }

        /// <summary>Adds the items of the specified sequence to the end of the mocked db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        /// <param name="items">The sequence whose items should be added to the end of the mocked db query source.</param>
        public static void AddRangeToReadOnlySource<TQuery>(this DbQuery<TQuery> mockedDbQuery, IEnumerable<TQuery> items) 
            where TQuery : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));
            EnsureArgument.IsNotNull(items, nameof(items));
            EnsureArgument.IsNotEmpty(items, nameof(items));

            var list = mockedDbQuery.ToList();
            list.AddRange(items);
            var queryable = list.AsQueryable();

            mockedDbQuery.SetSource(queryable);
        }

        /// <summary>Removes all items from the mocked db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        [Obsolete("This has been replaced by DbQuery<TQuery>.ClearReadOnlySource().")]
        public static void Clear<TQuery>(this DbQuery<TQuery> mockedDbQuery) 
            where TQuery : class
        {
            mockedDbQuery.ClearReadOnlySource();
        }

        /// <summary>Removes all items from the mocked db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        public static void ClearReadOnlySource<TQuery>(this DbQuery<TQuery> mockedDbQuery) 
            where TQuery : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));

            mockedDbQuery.SetSource(new List<TQuery>());
        }

        internal static void SetSource<TQuery>(this DbQuery<TQuery> mockedDbQuery, IEnumerable<TQuery> source) 
            where TQuery : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));
            EnsureArgument.IsNotNull(source, nameof(source));

            var queryable = source.AsQueryable();

            ((IAsyncEnumerableAccessor<TQuery>) mockedDbQuery).AsyncEnumerable.Returns(queryable.ToAsyncEnumerable());
            ((IQueryable<TQuery>) mockedDbQuery).ElementType.Returns(queryable.ElementType);
            ((IQueryable<TQuery>) mockedDbQuery).Expression.Returns(queryable.Expression);
            ((IEnumerable) mockedDbQuery).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());
            ((IEnumerable<TQuery>) mockedDbQuery).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());

            var provider = ((IQueryable<TQuery>) mockedDbQuery).Provider;
            ((AsyncQueryProvider<TQuery>) provider).SetSource(queryable);
        }
    }
}