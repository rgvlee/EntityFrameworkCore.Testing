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
    /// <summary>
    ///     Extensions for the db query type.
    /// </summary>
    public static class DbQueryExtensions
    {
        /// <summary>
        ///     Creates and sets up a mocked db query.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQuery">The db query to mock.</param>
        /// <returns>A mocked db query.</returns>
        public static DbQuery<TQuery> CreateMock<TQuery>(this DbQuery<TQuery> dbQuery) where TQuery : class
        {
            EnsureArgument.IsNotNull(dbQuery, nameof(dbQuery));

            var dbQueryMock = new Mock<DbQuery<TQuery>>();
            dbQueryMock.SetUp(new List<TQuery>());
            return dbQueryMock.Object;
        }

        /// <summary>
        ///     Adds an item to the end of the db query source.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        /// <param name="item">The item to be added to the end of the db query source.</param>
        public static void Add<TQuery>(this DbQuery<TQuery> mockedDbQuery, TQuery item) where TQuery : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));
            EnsureArgument.IsNotNull(item, nameof(item));

            var dbQueryMock = Mock.Get(mockedDbQuery);

            var list = mockedDbQuery.ToList();
            list.Add(item);
            var queryable = list.AsQueryable();

            dbQueryMock.SetUp(queryable);
        }

        /// <summary>
        ///     Adds the items of the specified sequence to the end of the db query source.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        /// <param name="enumerable">The sequence whose items should be added to the end of the db query source.</param>
        public static void AddRange<TQuery>(this DbQuery<TQuery> mockedDbQuery, IEnumerable<TQuery> enumerable) where TQuery : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));
            EnsureArgument.IsNotNull(enumerable, nameof(enumerable));
            EnsureArgument.IsNotEmpty(enumerable, nameof(enumerable));

            var dbQueryMock = Mock.Get(mockedDbQuery);

            var list = mockedDbQuery.ToList();
            list.AddRange(enumerable);
            var queryable = list.AsQueryable();

            dbQueryMock.SetUp(queryable);
        }

        /// <summary>
        ///     Removes all items from the db query source.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="mockedDbQuery">The mocked db query.</param>
        public static void Clear<TQuery>(this DbQuery<TQuery> mockedDbQuery) where TQuery : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));

            var dbQueryMock = Mock.Get(mockedDbQuery);
            dbQueryMock.SetUp(new List<TQuery>());
        }

        /// <summary>
        ///     Sets up a db query mock.
        /// </summary>
        /// <typeparam name="TQuery">The query type.</typeparam>
        /// <param name="dbQueryMock">The db query mock to set up.</param>
        /// <param name="enumerable">The db query source.</param>
        internal static void SetUp<TQuery>(this Mock<DbQuery<TQuery>> dbQueryMock, IEnumerable<TQuery> enumerable) where TQuery : class
        {
            EnsureArgument.IsNotNull(dbQueryMock, nameof(dbQueryMock));
            EnsureArgument.IsNotNull(enumerable, nameof(enumerable));

            var queryable = enumerable.AsQueryable();

            dbQueryMock.As<IAsyncEnumerableAccessor<TQuery>>().Setup(m => m.AsyncEnumerable).Returns(queryable.ToAsyncEnumerable());
            dbQueryMock.As<IQueryable<TQuery>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbQueryMock.As<IQueryable<TQuery>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbQueryMock.As<IEnumerable>()
                .Setup(m => m.GetEnumerator())
                .Returns(queryable.GetEnumerator());

            dbQueryMock.As<IEnumerable<TQuery>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            dbQueryMock.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance).Returns(() => ((IInfrastructure<IServiceProvider>) dbQueryMock.Object).Instance);

            dbQueryMock.As<IQueryable<TQuery>>().Setup(m => m.Provider).Returns(queryable.Provider.CreateMock(queryable));
        }
    }
}