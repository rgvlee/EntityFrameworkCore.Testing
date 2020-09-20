using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Testing.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Moq;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.Moq.Extensions
{
    public static partial class DbQueryExtensions
    {
        internal static DbQuery<TQuery> CreateMockedDbQuery<TQuery>(this DbQuery<TQuery> dbQuery) where TQuery : class
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

            var mockedQueryProvider = ((IQueryable<TQuery>) dbQuery).Provider.CreateMockedQueryProvider(new List<TQuery>());
            dbQueryMock.As<IQueryable<TQuery>>().Setup(m => m.Provider).Returns(mockedQueryProvider);

            return dbQueryMock.Object;
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