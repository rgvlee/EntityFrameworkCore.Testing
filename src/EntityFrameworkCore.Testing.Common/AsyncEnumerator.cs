using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Testing.Common
{
    public class AsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public AsyncEnumerator(IEnumerable<T> enumerable)
        {
            _enumerator = enumerable.GetEnumerator();
        }

        public ValueTask DisposeAsync()
        {
            return new();
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new(_enumerator.MoveNext());
        }

        public T Current => _enumerator.Current;
    }
}