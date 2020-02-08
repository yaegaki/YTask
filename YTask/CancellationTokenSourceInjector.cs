using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Async
{
    public struct CancellationTokenInjector : INotifyCompletion
    {
        public bool IsCompleted => false;
        public readonly CancellationToken CancellationToken;
        public readonly bool RestoreInjection;

        public CancellationTokenInjector(CancellationToken cancellationToken, bool restoreInjection)
            => (this.CancellationToken, this.RestoreInjection) = (cancellationToken, restoreInjection);

        public CancellationTokenInjector GetAwaiter()
            => this;

        public void GetResult()
        {
        }

        public void OnCompleted(Action continuation)
            => continuation();
    }
}