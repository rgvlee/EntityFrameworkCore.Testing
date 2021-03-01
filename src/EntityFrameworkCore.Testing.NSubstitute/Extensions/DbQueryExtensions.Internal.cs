using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Testing.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NSubstitute;
using rgvlee.Core.Common.Helpers;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    public static partial class DbQueryExtensions
    {
        internal static DbQuery<TEntity> CreateMockedDbQuery<TEntity>(this DbQuery<TEntity> dbQuery) where TEntity : class
        {
            EnsureArgument.IsNotNull(dbQuery, nameof(dbQuery));

            var mockedDbQuery = (DbQuery<TEntity>) Substitute.For(new[] {
                    typeof(DbQuery<TEntity>),
                    typeof(IAsyncEnumerableAccessor<TEntity>),
                    typeof(IEnumerable),
                    typeof(IEnumerable<TEntity>),
                    typeof(IInfrastructure<IServiceProvider>),
                    typeof(IQueryable<TEntity>)
                },
                new object[] { });

            var asyncEnumerable = new AsyncEnumerable<TEntity>(new List<TEntity>());
            var mockedQueryProvider = ((IQueryable<TEntity>) dbQuery).Provider.CreateMockedQueryProvider(asyncEnumerable);

            ((IAsyncEnumerableAccessor<TEntity>) mockedDbQuery).AsyncEnumerable.Returns(callInfo => asyncEnumerable);
            ((IQueryable<TEntity>) mockedDbQuery).ElementType.Returns(callInfo => asyncEnumerable.ElementType);
            ((IQueryable<TEntity>) mockedDbQuery).Expression.Returns(callInfo => asyncEnumerable.Expression);
            ((IEnumerable) mockedDbQuery).GetEnumerator().Returns(callInfo => ((IEnumerable) asyncEnumerable).GetEnumerator());
            ((IEnumerable<TEntity>) mockedDbQuery).GetEnumerator().Returns(callInfo => ((IEnumerable<TEntity>) asyncEnumerable).GetEnumerator());

            ((IInfrastructure<IServiceProvider>) mockedDbQuery).Instance.Returns(callInfo => ((IInfrastructure<IServiceProvider>) mockedDbQuery).Instance);

            ((IQueryable<TEntity>) mockedDbQuery).Provider.Returns(callInfo => mockedQueryProvider);

            return mockedDbQuery;
        }

        internal static void SetSource<TEntity>(this DbQuery<TEntity> mockedDbQuery, IEnumerable<TEntity> source) where TEntity : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));
            EnsureArgument.IsNotNull(source, nameof(source));

            var asyncEnumerable = new AsyncEnumerable<TEntity>(source);
            var mockedQueryProvider = ((IQueryable<TEntity>) mockedDbQuery).Provider;

            ((IAsyncEnumerableAccessor<TEntity>) mockedDbQuery).AsyncEnumerable.Returns(callInfo => asyncEnumerable);
            ((IQueryable<TEntity>) mockedDbQuery).Expression.Returns(callInfo => asyncEnumerable.Expression);
            ((IEnumerable) mockedDbQuery).GetEnumerator().Returns(callInfo => ((IEnumerable) asyncEnumerable).GetEnumerator());
            ((IEnumerable<TEntity>) mockedDbQuery).GetEnumerator().Returns(callInfo => ((IEnumerable<TEntity>) asyncEnumerable).GetEnumerator());

            ((AsyncQueryProvider<TEntity>) mockedQueryProvider).SetSource(asyncEnumerable);
        }
    }
}