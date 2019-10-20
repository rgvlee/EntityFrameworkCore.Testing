using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace EntityFrameworkCore.Testing.Common
{
    /// <inheritdoc cref="IAsyncEnumerable{T}" />
    [ExcludeFromCodeCoverage]
    public class AsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="expression">The expression to create an asynchronous enumerable for.</param>
        public AsyncEnumerable(Expression expression) : base(expression) { }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="enumerable">The enumerable to create an asynchronous enumerable for.</param>
        public AsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }

        /// <inheritdoc />
        public IAsyncEnumerator<T> GetEnumerator()
        {
            return new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }
    }
}