using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityFrameworkCore.Testing.Common
{
    public class AsyncEnumerable<T> : IAsyncEnumerable<T>, IOrderedQueryable<T>
    {
        private readonly IEnumerable<T> _innerEnumerable;

        public AsyncEnumerable(IEnumerable<T> enumerable)
        {
            _innerEnumerable = enumerable;

            Provider = new AsyncQueryProvider<T>(_innerEnumerable);

            if (_innerEnumerable is IQueryable<T> queryable)
            {
                Expression = queryable.Expression;
            }
            else
            {
                Expression = _innerEnumerable.AsQueryable().Expression;
            }
        }

        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
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