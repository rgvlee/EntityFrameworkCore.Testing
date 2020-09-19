using System.Collections.Generic;
using System.Threading;
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

        public void Dispose() { }

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.Run(() => _enumerator.MoveNext());
        }

        public T Current => _enumerator.Current;
    }
}