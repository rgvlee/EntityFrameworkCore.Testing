using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace EntityFrameworkCore.Testing.Common {
    /// <summary>
    /// Provides an asynchronous query provider for an enumerable sequence.
    /// </summary>
    /// <typeparam name="T">The enumerable sequence element type.</typeparam>
    public class AsyncQueryProvider<T> : IAsyncQueryProvider {
        private readonly IQueryable _sequence;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sequence">The sequence to create an asynchronous query provider for.</param>
        public AsyncQueryProvider(IEnumerable<T> sequence) {
            _sequence = sequence.AsQueryable();
        }

        /// <inheritdoc />
        public IQueryable CreateQuery(Expression expression) {
            return new AsyncEnumerable<T>(expression);
        }

        /// <inheritdoc />
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) {
            return new AsyncEnumerable<TElement>(expression);
        }

        /// <inheritdoc />
        public object Execute(Expression expression) {
            return _sequence.Provider.Execute(expression);
        }

        /// <inheritdoc />
        public TResult Execute<TResult>(Expression expression) {
            return _sequence.Provider.Execute<TResult>(expression);
        }

        /// <inheritdoc />
        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression) {
            return Task.FromResult(Execute<TResult>(expression)).ToAsyncEnumerable();
        }

        /// <inheritdoc />
        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) {
            return Task.FromResult(Execute<TResult>(expression));
        }
    }
}
