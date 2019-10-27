using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Testing.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Moq;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    /// <summary>Extensions for the db query type.</summary>
    public static class DbQueryExtensions
    {
        /// <summary>Creates and sets up a mocked db query.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQuery">The db query to mock.</param>
        /// <returns>A mocked db query.</returns>
        public static DbQuery<TQuery> CreateMock<TQuery>(this DbQuery<TQuery> dbQuery) where TQuery : class
        {
            EnsureArgument.IsNotNull(dbQuery, nameof(dbQuery));

            var dbQueryMock = new Mock<DbQuery<TQuery>>();

            var queryable = new List<TQuery>().AsQueryable();

            dbQueryMock.As<IAsyncEnumerableAccessor<TQuery>>().Setup(m => m.AsyncEnumerable).Returns(queryable.ToAsyncEnumerable());
            dbQueryMock.As<IQueryable<TQuery>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbQueryMock.As<IQueryable<TQuery>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbQueryMock.As<IEnumerable>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            dbQueryMock.As<IEnumerable<TQuery>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            dbQueryMock.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance).Returns(((IInfrastructure<IServiceProvider>) dbQuery).Instance);

            var mockedQueryProvider = ((IQueryable<TQuery>) dbQuery).Provider.CreateMock(new List<TQuery>());
            dbQueryMock.As<IQueryable<TQuery>>().Setup(m => m.Provider).Returns(mockedQueryProvider);

            return dbQueryMock.Object;
        }

        /// <summary>Adds an item to the end of the mocked db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        /// <param name="item">The item to be added to the end of the mocked db query source.</param>
        public static void Add<TQuery>(this DbQuery<TQuery> mockedDbQuery, TQuery item) where TQuery : class
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
        /// <param name="enumerable">The sequence whose items should be added to the end of the mocked db query source.</param>
        public static void AddRange<TQuery>(this DbQuery<TQuery> mockedDbQuery, IEnumerable<TQuery> enumerable) where TQuery : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));
            EnsureArgument.IsNotNull(enumerable, nameof(enumerable));
            EnsureArgument.IsNotEmpty(enumerable, nameof(enumerable));

            var list = mockedDbQuery.ToList();
            list.AddRange(enumerable);
            var queryable = list.AsQueryable();

            mockedDbQuery.SetSource(queryable);
        }

        /// <summary>Removes all items from the mocked db query source.</summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        public static void Clear<TQuery>(this DbQuery<TQuery> mockedDbQuery) where TQuery : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));

            mockedDbQuery.SetSource(new List<TQuery>());
        }

        internal static void SetSource<TQuery>(this DbQuery<TQuery> mockedDbQuery, IEnumerable<TQuery> source) where TQuery : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));
            EnsureArgument.IsNotNull(source, nameof(source));

            var dbQueryMock = Mock.Get(mockedDbQuery);

            var queryable = source.AsQueryable();

            dbQueryMock.As<IAsyncEnumerableAccessor<TQuery>>().Setup(m => m.AsyncEnumerable).Returns(queryable.ToAsyncEnumerable());
            dbQueryMock.As<IQueryable<TQuery>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbQueryMock.As<IQueryable<TQuery>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbQueryMock.As<IEnumerable>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            dbQueryMock.As<IEnumerable<TQuery>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            var provider = ((IQueryable<TQuery>) mockedDbQuery).Provider;
            ((AsyncQueryProvider<TQuery>) provider).SetSource(queryable);
        }
    }
}