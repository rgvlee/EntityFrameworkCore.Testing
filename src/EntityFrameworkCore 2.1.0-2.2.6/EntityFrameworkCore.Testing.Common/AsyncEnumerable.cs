using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityFrameworkCore.Testing.Common
{
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

        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
        {
            return _enumerable.ToAsyncEnumerable().GetEnumerator();
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