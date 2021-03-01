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
        internal static DbQuery<TEntity> CreateMockedDbQuery<TEntity>(this DbQuery<TEntity> dbQuery) where TEntity : class
        {
            EnsureArgument.IsNotNull(dbQuery, nameof(dbQuery));

            var dbQueryMock = new Mock<DbQuery<TEntity>>();

            var asyncEnumerable = new AsyncEnumerable<TEntity>(new List<TEntity>());
            var mockedQueryProvider = ((IQueryable<TEntity>) dbQuery).Provider.CreateMockedQueryProvider(asyncEnumerable);

            dbQueryMock.As<IAsyncEnumerableAccessor<TEntity>>().Setup(m => m.AsyncEnumerable).Returns(() => asyncEnumerable);
            dbQueryMock.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(() => asyncEnumerable.ElementType);
            dbQueryMock.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(() => asyncEnumerable.Expression);
            dbQueryMock.As<IEnumerable>().Setup(m => m.GetEnumerator()).Returns(() => ((IEnumerable) asyncEnumerable).GetEnumerator());
            dbQueryMock.As<IEnumerable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => ((IEnumerable<TEntity>) asyncEnumerable).GetEnumerator());

            dbQueryMock.As<IInfrastructure<IServiceProvider>>().Setup(m => m.Instance).Returns(() => ((IInfrastructure<IServiceProvider>) dbQuery).Instance);

            dbQueryMock.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(() => mockedQueryProvider);

            return dbQueryMock.Object;
        }

        internal static void SetSource<TEntity>(this DbQuery<TEntity> mockedDbQuery, IEnumerable<TEntity> source) where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));
            EnsureArgument.IsNotNull(source, nameof(source));

            var dbQueryMock = Mock.Get(mockedDbQuery);

            var asyncEnumerable = new AsyncEnumerable<TEntity>(source);
            var mockedQueryProvider = ((IQueryable<TEntity>) mockedDbQuery).Provider;

            dbQueryMock.As<IAsyncEnumerableAccessor<TEntity>>().Setup(m => m.AsyncEnumerable).Returns(() => asyncEnumerable);
            dbQueryMock.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(() => asyncEnumerable.Expression);
            dbQueryMock.As<IEnumerable>().Setup(m => m.GetEnumerator()).Returns(() => ((IEnumerable) asyncEnumerable).GetEnumerator());
            dbQueryMock.As<IEnumerable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => ((IEnumerable<TEntity>) asyncEnumerable).GetEnumerator());

            ((AsyncQueryProvider<TEntity>) mockedQueryProvider).SetSource(asyncEnumerable);
        }
    }
}