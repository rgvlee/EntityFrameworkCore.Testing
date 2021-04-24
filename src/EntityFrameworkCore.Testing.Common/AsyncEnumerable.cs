using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore.Query;

namespace EntityFrameworkCore.Testing.Common
{
    public class AsyncEnumerable<T> : IAsyncEnumerable<T>, IOrderedQueryable<T>
    {
        private readonly IQueryable<T> _source;

        public AsyncEnumerable(IEnumerable<T> enumerable)
        {
            _source = enumerable.AsQueryable();

            Provider = new AsyncQueryProvider<T>(_source);

            Expression = _source.Expression;
        }

        public AsyncEnumerable(IEnumerable<T> enumerable, QueryRootExpression expression) : this(enumerable)
        {
            Expression = expression;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return new AsyncEnumerator<T>(_source);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        public Type ElementType => typeof(T);

        public Expression Expression { get; }

        public IQueryProvider Provider { get; }
    }
}