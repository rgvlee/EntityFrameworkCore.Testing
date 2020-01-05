using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Testing.Common
{
    public static class EnumerableExtensions
    {
        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> enumerable)
        {
            foreach (var item in enumerable)
            {
                await Task.Delay(0);
                yield return item;
            }
        }
    }

    public class AsyncEnumerable<T> : IAsyncEnumerable<T>, IOrderedQueryable<T>, IEnumerable<T>, IEnumerable, IOrderedQueryable, IQueryable, IQueryable<T>
    {
        private readonly IEnumerable<T> _enumerable;
        private readonly IQueryable<T> _queryable;

        public AsyncEnumerable(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;

            _queryable = _enumerable.AsQueryable();

            ElementType = _queryable.ElementType;
            Expression = _queryable.Expression;
            Provider = new AsyncQueryProvider<T>(_queryable);
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            return _queryable.ToAsyncEnumerable().GetAsyncEnumerator(cancellationToken);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _enumerable.GetEnumerator();
        }

        public Type ElementType { get; }
        public Expression Expression { get; }
        public IQueryProvider Provider { get; }
    }
}