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
        /// <summary>Creates and sets up a substitute db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQuery">The db query to mock.</param>
        /// <returns>A substitute db query.</returns>
        public static DbQuery<TQuery> CreateSubstituteDbQuery<TQuery>(this DbQuery<TQuery> dbQuery)
            where TQuery : class
        {
            EnsureArgument.IsNotNull(dbQuery, nameof(dbQuery));

            var substituteDbQuery = (DbQuery<TQuery>)
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

            ((IAsyncEnumerableAccessor<TQuery>) substituteDbQuery).AsyncEnumerable.Returns(callInfo => queryable.ToAsyncEnumerable());
            ((IQueryable<TQuery>) substituteDbQuery).ElementType.Returns(callInfo => queryable.ElementType);
            ((IQueryable<TQuery>) substituteDbQuery).Expression.Returns(callInfo => queryable.Expression);
            ((IEnumerable) substituteDbQuery).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());
            ((IEnumerable<TQuery>) substituteDbQuery).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());

            ((IInfrastructure<IServiceProvider>) substituteDbQuery).Instance.Returns(callInfo => ((IInfrastructure<IServiceProvider>) substituteDbQuery).Instance);

            var substituteQueryProvider = ((IQueryable<TQuery>) substituteDbQuery).Provider.CreateSubstituteQueryProvider(new List<TQuery>());
            ((IQueryable<TQuery>) substituteDbQuery).Provider.Returns(callInfo => substituteQueryProvider);

            return substituteDbQuery;
        }

        /// <summary>Creates and sets up a substitute db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQuery">The db query to mock.</param>
        /// <returns>A substitute db query.</returns>
        [Obsolete("This will be removed in a future version. Use DbQueryExtensions.CreateDbQuerySubstitute instead.")]
        public static DbQuery<TQuery> CreateMock<TQuery>(this DbQuery<TQuery> dbQuery)
            where TQuery : class
        {
            return dbQuery.CreateSubstituteDbQuery();
        }

        /// <summary>Adds an item to the end of the substitute db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="substituteDbQuery">The substitute db query.</param>
        /// <param name="item">The item to be added to the end of the substitute db query source.</param>
        [Obsolete(
            "This has been replaced by DbQueryExtensions.AddToReadOnlySource to avoid conflicts with the " +
            "EntityFrameworkCore 3.0.0 read only set Add method.")
        ]
        public static void Add<TQuery>(this DbQuery<TQuery> substituteDbQuery, TQuery item)
            where TQuery : class
        {
            substituteDbQuery.AddToReadOnlySource(item);
        }

        /// <summary>Adds an item to the end of the substitute db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="substituteDbQuery">The substitute db query.</param>
        /// <param name="item">The item to be added to the end of the substitute db query source.</param>
        public static void AddToReadOnlySource<TQuery>(this DbQuery<TQuery> substituteDbQuery, TQuery item)
            where TQuery : class
        {
            EnsureArgument.IsNotNull(substituteDbQuery, nameof(substituteDbQuery));
            EnsureArgument.IsNotNull(item, nameof(item));

            var list = substituteDbQuery.ToList();
            list.Add(item);
            var queryable = list.AsQueryable();

            substituteDbQuery.SetSource(queryable);
        }

        /// <summary>Adds the items of the specified sequence to the end of the substitute db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="substituteDbQuery">The substitute db query.</param>
        /// <param name="items">The sequence whose items should be added to the end of the substitute db query source.</param>
        [Obsolete(
            "This has been replaced by DbQueryExtensions.AddRangeToReadOnlySource to avoid conflicts with the " +
            "EntityFrameworkCore 3.0.0 read only set AddRange method.")
        ]
        public static void AddRange<TQuery>(this DbQuery<TQuery> substituteDbQuery, IEnumerable<TQuery> items)
            where TQuery : class
        {
            substituteDbQuery.AddRangeToReadOnlySource(items);
        }

        /// <summary>Adds the items of the specified sequence to the end of the substitute db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="substituteDbQuery">The substitute db query.</param>
        /// <param name="items">The sequence whose items should be added to the end of the substitute db query source.</param>
        public static void AddRangeToReadOnlySource<TQuery>(this DbQuery<TQuery> substituteDbQuery, IEnumerable<TQuery> items)
            where TQuery : class
        {
            EnsureArgument.IsNotNull(substituteDbQuery, nameof(substituteDbQuery));
            EnsureArgument.IsNotNull(items, nameof(items));
            EnsureArgument.IsNotEmpty(items, nameof(items));

            var list = substituteDbQuery.ToList();
            list.AddRange(items);
            var queryable = list.AsQueryable();

            substituteDbQuery.SetSource(queryable);
        }

        /// <summary>Removes all items from the substitute db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="substituteDbQuery">The substitute db query.</param>
        [Obsolete("This has been replaced by DbQueryExtensions.ClearReadOnlySource.")]
        public static void Clear<TQuery>(this DbQuery<TQuery> substituteDbQuery)
            where TQuery : class
        {
            substituteDbQuery.ClearReadOnlySource();
        }

        /// <summary>Removes all items from the substitute db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="substituteDbQuery">The substitute db query.</param>
        public static void ClearReadOnlySource<TQuery>(this DbQuery<TQuery> substituteDbQuery)
            where TQuery : class
        {
            EnsureArgument.IsNotNull(substituteDbQuery, nameof(substituteDbQuery));

            substituteDbQuery.SetSource(new List<TQuery>());
        }

        internal static void SetSource<TQuery>(this DbQuery<TQuery> substituteDbQuery, IEnumerable<TQuery> source)
            where TQuery : class
        {
            EnsureArgument.IsNotNull(substituteDbQuery, nameof(substituteDbQuery));
            EnsureArgument.IsNotNull(source, nameof(source));

            var queryable = source.AsQueryable();

            ((IAsyncEnumerableAccessor<TQuery>) substituteDbQuery).AsyncEnumerable.Returns(callInfo => queryable.ToAsyncEnumerable());
            ((IQueryable<TQuery>) substituteDbQuery).ElementType.Returns(callInfo => queryable.ElementType);
            ((IQueryable<TQuery>) substituteDbQuery).Expression.Returns(callInfo => queryable.Expression);
            ((IEnumerable) substituteDbQuery).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());
            ((IEnumerable<TQuery>) substituteDbQuery).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());

            var provider = ((IQueryable<TQuery>) substituteDbQuery).Provider;
            ((AsyncQueryProvider<TQuery>) provider).SetSource(queryable);
        }
    }
}