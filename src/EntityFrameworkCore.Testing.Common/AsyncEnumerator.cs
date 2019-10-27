using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Testing.Common
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class AsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;
        private bool _disposed;

        /// <summary>Constructor.</summary>
        /// <param name="enumerator">The enumerator to create an asynchronous enumerator for.</param>
        public AsyncEnumerator(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        /// <inheritdoc />
        public T Current => _enumerator.Current;

        /// <inheritdoc />
        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(_enumerator.MoveNext());
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Dispose(true);
            //no need for the finalizer to run - suppress it so we don't extend the lifetime at the hands of the finalizer
            GC.SuppressFinalize(this);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <param name="disposing">Provides disposal context; true if invoked by the instance, false if invoked by the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) _enumerator.Dispose();

            _disposed = true;
        }
    }
}