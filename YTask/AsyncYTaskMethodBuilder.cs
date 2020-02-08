using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Async
{
    public class AsyncYTaskMethodBuilder
    {
        public static AsyncYTaskMethodBuilder Create() => new AsyncYTaskMethodBuilder();

        public YTask Task => new YTask(completionSource.Task);

        private readonly TaskCompletionSource<int> completionSource = new TaskCompletionSource<int>();
        private CancellationToken cancellationToken;

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
            => stateMachine.MoveNext();

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }

        public void SetResult()
            => this.completionSource.TrySetResult(default);

        public void SetException(Exception e)
            => this.completionSource.TrySetException(e);
        
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            if (awaiter is CancellationTokenInjector cti)
            {
                this.cancellationToken = cti.CancellationToken;
                stateMachine.MoveNext();
                return;
            }

            if (this.cancellationToken.CanBeCanceled)
            {
                var moveNext = new Action(stateMachine.MoveNext);
                awaiter.OnCompleted(() =>
                {
                    if (this.cancellationToken.IsCancellationRequested)
                    {
                        this.completionSource.SetException(new OperationCanceledException(this.cancellationToken));
                        return;
                    }
                    moveNext();
                });
            }
            else
            {
                awaiter.OnCompleted(stateMachine.MoveNext);
            }
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            if (awaiter is CancellationTokenInjector cti)
            {
                this.cancellationToken = cti.CancellationToken;
                stateMachine.MoveNext();
                return;
            }

            if (this.cancellationToken.CanBeCanceled)
            {
                var moveNext = new Action(stateMachine.MoveNext);
                awaiter.UnsafeOnCompleted(() =>
                {
                    if (this.cancellationToken.IsCancellationRequested)
                    {
                        this.completionSource.SetException(new OperationCanceledException(this.cancellationToken));
                        return;
                    }
                    moveNext();
                });
            }
            else
            {
                awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
            }
        }
    }

    public class AsyncYTaskMethodBuilder<T>
    {
        public static AsyncYTaskMethodBuilder<T> Create() => new AsyncYTaskMethodBuilder<T>();

        public YTask<T> Task => new YTask<T>(completionSource.Task);

        private readonly TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
        private CancellationToken cancellationToken;

        public void Start<TStateMachine>(ref TStateMachine stateMachine)
            where TStateMachine : IAsyncStateMachine
            => stateMachine.MoveNext();

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }

        public void SetResult(T result)
            => this.completionSource.TrySetResult(result);

        public void SetException(Exception e)
            => this.completionSource.TrySetException(e);
        
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            if (awaiter is CancellationTokenInjector cti)
            {
                this.cancellationToken = cti.CancellationToken;
                stateMachine.MoveNext();
                return;
            }

            if (this.cancellationToken.CanBeCanceled)
            {
                var moveNext = new Action(stateMachine.MoveNext);
                awaiter.OnCompleted(() =>
                {
                    this.cancellationToken.ThrowIfCancellationRequested();
                    moveNext();
                });
            }
            else
            {
                awaiter.OnCompleted(stateMachine.MoveNext);
            }
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            if (awaiter is CancellationTokenInjector cti)
            {
                this.cancellationToken = cti.CancellationToken;
                stateMachine.MoveNext();
                return;
            }

            if (this.cancellationToken.CanBeCanceled)
            {
                var moveNext = new Action(stateMachine.MoveNext);
                awaiter.UnsafeOnCompleted(() =>
                {
                    this.cancellationToken.ThrowIfCancellationRequested();
                    moveNext();
                });
            }
            else
            {
                awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
            }
        }
    }
}
