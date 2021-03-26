using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace EntityFrameworkCore.Testing.Common
{
    public class AsyncEnumerable<T> : IAsyncEnumerable<T>, IOrderedQueryable<T>
    {
        private readonly IEnumerable<T> _innerEnumerable;

        public AsyncEnumerable(IEnumerable<T> enumerable)
        {
            _innerEnumerable = enumerable;

            var provider = new AsyncQueryProvider<T>(_innerEnumerable);
            Provider = provider;

            if (_innerEnumerable is IQueryable<T> queryable && queryable.Expression is Microsoft.EntityFrameworkCore.Query.QueryRootExpression)
            {
                Expression = queryable.Expression;
            }
            else
            {
                Expression = new QueryRootExpression(provider, new FakeEntityType(typeof(T)));
            }
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            return new AsyncEnumerator<T>(_innerEnumerable);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _innerEnumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _innerEnumerable.GetEnumerator();
        }

        public Type ElementType => typeof(T);

        public Expression Expression { get; }

        public IQueryProvider Provider { get; }
    }
}