using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Testing.Moq {
    /// <inheritdoc />
    public class AsyncEnumerator<T> : IAsyncEnumerator<T> {
        private bool _disposed;
        private readonly IEnumerator<T> _enumerator;

        /// <inheritdoc />
        public T Current => _enumerator.Current;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="enumerator">The enumerator.</param>
        public AsyncEnumerator(IEnumerator<T> enumerator) {
            _enumerator = enumerator;
        }

        /// <inheritdoc />
        public Task<bool> MoveNext(CancellationToken cancellationToken) => Task.FromResult(_enumerator.MoveNext());

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            //no need for the finalizer to run - suppress it so we don't extend the lifetime at the hands of the finalizer
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">Provides disposal context; true if invoked by the instance, false if invoked by the finalizer.</param>
        protected virtual void Dispose(bool disposing) {
            if (_disposed) return;
            if (disposing) {
                //invoked by user
                _enumerator.Dispose();
            }
            else {
                //invoked by finalizer
            }
            _disposed = true;
        }
    }
}