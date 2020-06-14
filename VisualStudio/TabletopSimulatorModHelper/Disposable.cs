using System;

namespace TabletopSimulatorModHelper
{
    public abstract class Disposable: IDisposable
    {
        private bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                Dispose(true);
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose of native resources.
        /// Dispose of managed resources only if <paramref name="disposing"/> is <c>true</c>.
        /// </summary>
        /// <param name="disposing">Whether to dispose of managed resources</param>
        protected abstract void Dispose(bool disposing);

        ~Disposable()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                Dispose(false);
            }
        }
    }
}
