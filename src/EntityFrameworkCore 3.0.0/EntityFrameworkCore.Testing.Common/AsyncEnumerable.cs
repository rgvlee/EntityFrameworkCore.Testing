using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace EntityFrameworkCore.Testing.Common
{
    /// <inheritdoc cref="IAsyncEnumerable{T}" />
    [ExcludeFromCodeCoverage]
    public class AsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable, IQueryable<T>
    {
        private readonly IQueryProvider _provider;

        /// <summary>Constructor.</summary>
        /// <param name="expression">The expression to create an asynchronous enumerable for.</param>
        public AsyncEnumerable(Expression expression) : base(expression)
        {
            var queryable = (IQueryable<T>) ((ConstantExpression) ((MethodCallExpression) expression).Arguments[0]).Value;
            _provider = queryable.Provider;
        }

        /// <summary>Constructor.</summary>
        /// <param name="enumerable">The enumerable to create an asynchronous enumerable for.</param>
        public AsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
        {
            var provider = new AsyncQueryProvider<T>();
            provider.Source = enumerable.AsQueryable();
            _provider = provider;
        }

        /// <inheritdoc />
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            return new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => _provider;
    }
}