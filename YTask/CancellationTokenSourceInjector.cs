using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Async
{
    public struct CancellationTokenInjector : INotifyCompletion
    {
        public bool IsCompleted => false;
        public readonly CancellationToken CancellationToken;

        public CancellationTokenInjector(CancellationToken cancellationToken)
            => this.CancellationToken = cancellationToken;

        public CancellationTokenInjector GetAwaiter()
            => this;

        public void GetResult()
        {
        }

        public void OnCompleted(Action continuation)
            => continuation();
    }
}