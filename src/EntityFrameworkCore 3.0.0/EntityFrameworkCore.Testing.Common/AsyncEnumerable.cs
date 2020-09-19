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
        private readonly IEnumerable<T> _enumerable;

        public AsyncEnumerable(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;
            Expression = enumerable.AsQueryable().Expression;
            Provider = new AsyncQueryProvider<T>(_enumerable);
        }

        public AsyncEnumerable(Expression expression)
        {
            _enumerable = Expression.Lambda<Func<IEnumerable<T>>>(expression, null).Compile()();
            Expression = expression;
            Provider = new AsyncQueryProvider<T>(_enumerable);
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            return new AsyncEnumerator<T>(_enumerable);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _enumerable.GetEnumerator();
        }

        public Type ElementType => typeof(T);
        public Expression Expression { get; }
        public IQueryProvider Provider { get; }
    }
}