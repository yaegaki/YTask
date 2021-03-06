using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Async
{
    [AsyncMethodBuilder(typeof(AsyncYTaskMethodBuilder))]
    public struct YTask
    {
        public static CancellationTokenInjector Inject(CancellationToken cancellationToken)
            => new CancellationTokenInjector(cancellationToken, false);

        private static CancellationTokenInjector staticCancellationTokenInjector;

        public static CancellationTokenInjector InjectToStatic(CancellationToken cancellationToken)
        {
            staticCancellationTokenInjector = new CancellationTokenInjector(cancellationToken, true);
            return staticCancellationTokenInjector;
        }

        public static CancellationTokenInjector InjectFromStatic()
            => staticCancellationTokenInjector;

        private readonly Task task;

        public YTask(Task task)
            => this.task = task;
        

        public TaskAwaiter GetAwaiter()
            => this.task.GetAwaiter();
    }

    [AsyncMethodBuilder(typeof(AsyncYTaskMethodBuilder<>))]
    public struct YTask<T>
    {
        private readonly Task<T> task;

        public YTask(Task<T> task)
            => this.task = task;
        
        public TaskAwaiter<T> GetAwaiter()
            => this.task.GetAwaiter();
    }
}