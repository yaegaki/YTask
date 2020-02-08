using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Async
{
    public class AsyncYTaskMethodBuilder
    {
        public static AsyncYTaskMethodBuilder Create() => new AsyncYTaskMethodBuilder();

        public YTask Task => new YTask(completionSource.Task);

        private readonly TaskCompletionSource<int> completionSource = new TaskCompletionSource<int>();
        private CancellationTokenInjector cancellationTokenInjector;

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
                this.cancellationTokenInjector = cti;
                stateMachine.MoveNext();
                return;
            }

            if (this.cancellationTokenInjector.CancellationToken.CanBeCanceled)
            {
                var moveNext = new Action(stateMachine.MoveNext);
                awaiter.OnCompleted(() =>
                {
                    var ct = this.cancellationTokenInjector.CancellationToken;
                    if (ct.IsCancellationRequested)
                    {
                        this.completionSource.SetException(new OperationCanceledException(ct));
                        return;
                    }
                    if (this.cancellationTokenInjector.RestoreInjection)
                    {
                        _ = YTask.InjectToStatic(ct);
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
                this.cancellationTokenInjector = cti;
                stateMachine.MoveNext();
                return;
            }

            if (this.cancellationTokenInjector.CancellationToken.CanBeCanceled)
            {
                var moveNext = new Action(stateMachine.MoveNext);
                awaiter.UnsafeOnCompleted(() =>
                {
                    var ct = this.cancellationTokenInjector.CancellationToken;
                    if (ct.IsCancellationRequested)
                    {
                        this.completionSource.SetException(new OperationCanceledException(ct));
                        return;
                    }
                    if (this.cancellationTokenInjector.RestoreInjection)
                    {
                        _ = YTask.InjectToStatic(ct);
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
        private CancellationTokenInjector cancellationTokenInjector;

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
                this.cancellationTokenInjector = cti;
                stateMachine.MoveNext();
                return;
            }

            if (this.cancellationTokenInjector.CancellationToken.CanBeCanceled)
            {
                var moveNext = new Action(stateMachine.MoveNext);
                awaiter.OnCompleted(() =>
                {
                    var ct = this.cancellationTokenInjector.CancellationToken;
                    if (ct.IsCancellationRequested)
                    {
                        this.completionSource.SetException(new OperationCanceledException(ct));
                        return;
                    }
                    if (this.cancellationTokenInjector.RestoreInjection)
                    {
                        _ = YTask.InjectToStatic(ct);
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
                this.cancellationTokenInjector = cti;
                stateMachine.MoveNext();
                return;
            }

            if (this.cancellationTokenInjector.CancellationToken.CanBeCanceled)
            {
                var moveNext = new Action(stateMachine.MoveNext);
                awaiter.UnsafeOnCompleted(() =>
                {
                    var ct = this.cancellationTokenInjector.CancellationToken;
                    if (ct.IsCancellationRequested)
                    {
                        this.completionSource.SetException(new OperationCanceledException(ct));
                        return;
                    }
                    if (this.cancellationTokenInjector.RestoreInjection)
                    {
                        _ = YTask.InjectToStatic(ct);
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
}
