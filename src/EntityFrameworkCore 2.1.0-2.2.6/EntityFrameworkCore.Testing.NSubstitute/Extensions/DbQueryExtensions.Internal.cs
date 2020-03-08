using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Testing.Common;
using EntityFrameworkCore.Testing.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NSubstitute;

namespace EntityFrameworkCore.Testing.NSubstitute.Extensions
{
    public static partial class DbQueryExtensions
    {
        internal static DbQuery<TQuery> CreateMockedDbQuery<TQuery>(this DbQuery<TQuery> dbQuery) where TQuery : class
        {
            EnsureArgument.IsNotNull(dbQuery, nameof(dbQuery));

            var mockedDbQuery = (DbQuery<TQuery>) Substitute.For(new[] {
                    typeof(DbQuery<TQuery>),
                    typeof(IAsyncEnumerableAccessor<TQuery>),
                    typeof(IEnumerable),
                    typeof(IEnumerable<TQuery>),
                    typeof(IInfrastructure<IServiceProvider>),
                    typeof(IQueryable<TQuery>)
                },
                new object[] { });

            var queryable = new List<TQuery>().AsQueryable();

            ((IAsyncEnumerableAccessor<TQuery>) mockedDbQuery).AsyncEnumerable.Returns(callInfo => queryable.ToAsyncEnumerable());
            ((IQueryable<TQuery>) mockedDbQuery).ElementType.Returns(callInfo => queryable.ElementType);
            ((IQueryable<TQuery>) mockedDbQuery).Expression.Returns(callInfo => queryable.Expression);
            ((IEnumerable) mockedDbQuery).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());
            ((IEnumerable<TQuery>) mockedDbQuery).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());

            ((IInfrastructure<IServiceProvider>) mockedDbQuery).Instance.Returns(callInfo => ((IInfrastructure<IServiceProvider>) mockedDbQuery).Instance);

            var mockedQueryProvider = ((IQueryable<TQuery>) mockedDbQuery).Provider.CreateMockedQueryProvider(new List<TQuery>());
            ((IQueryable<TQuery>) mockedDbQuery).Provider.Returns(callInfo => mockedQueryProvider);

            return mockedDbQuery;
        }

        internal static void SetSource<TQuery>(this DbQuery<TQuery> mockedDbQuery, IEnumerable<TQuery> source) where TQuery : class
        {
            EnsureArgument.IsNotNull(mockedDbQuery, nameof(mockedDbQuery));
            EnsureArgument.IsNotNull(source, nameof(source));

            var queryable = source.AsQueryable();

            ((IAsyncEnumerableAccessor<TQuery>) mockedDbQuery).AsyncEnumerable.Returns(callInfo => queryable.ToAsyncEnumerable());
            ((IQueryable<TQuery>) mockedDbQuery).ElementType.Returns(callInfo => queryable.ElementType);
            ((IQueryable<TQuery>) mockedDbQuery).Expression.Returns(callInfo => queryable.Expression);
            ((IEnumerable) mockedDbQuery).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());
            ((IEnumerable<TQuery>) mockedDbQuery).GetEnumerator().Returns(callInfo => queryable.GetEnumerator());

            var provider = ((IQueryable<TQuery>) mockedDbQuery).Provider;
            ((AsyncQueryProvider<TQuery>) provider).SetSource(queryable);
        }
    }
}