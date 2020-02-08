using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Async.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            _ = Task.Run(async () =>
            {
                await Task.Delay(5000);
                cts.Cancel();
            });

            try
            {
                await HogeAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("catch OperationCanceledException");
            }

            var finish = false;
            var sync = new object();
            var queue = new Queue<Action>();
            var loop = Task.Run(async () =>
            {
                var queue2 = new Queue<Action>();
                while (!finish)
                {
                    await Task.Delay(1);
                    lock (sync)
                    {
                        if (queue.Count == 0) continue;
                        (queue2, queue) = (queue, queue2);
                    }

                    while (queue2.Count > 0)
                    {
                        queue2.Dequeue().Invoke();
                    }
                }
            });
            SynchronizationContext.SetSynchronizationContext(new CustomSynchronizationContext((p, s) =>
            {
                lock (sync)
                {
                    queue.Enqueue(() => p(s));
                }
            }));
            Console.WriteLine("Nested AsyncMethod");
            await Task.WhenAll(
                NestedAsyncMethodTest("A", 3000),
                NestedAsyncMethodTest("B", 6000)
            );
            finish = true;
            await loop;
        }

        static async Task NestedAsyncMethodTest(string name, int delay)
        {
            var cts = new CancellationTokenSource();
            _ = Task.Run(async () =>
            {
                await Task.Delay(delay);
                cts.Cancel();
            });
            try
            {
                await FugaAsync(name, cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"{name}: catch OperationCanceledException");
            }
        }

        static async YTask HogeAsync(CancellationToken token)
        {
            await YTask.Inject(token);

            for (var i = 0; ; i++)
            {
                Console.WriteLine(i);
                await Task.Delay(1000);
            }
        }

        static async YTask FugaAsync(string name, CancellationToken token)
        {
            await YTask.InjectToStatic(token);

            await Task.Delay(300);

            await PiyoAsync(name);
        }

        static async YTask PiyoAsync(string name)
        {
            await YTask.InjectFromStatic();

            for (var i = 0; ; i++)
            {
                Console.WriteLine($"{name}:{i}");
                await Task.Delay(1000);
            }
        }

        class CustomSynchronizationContext : SynchronizationContext
        {
            private Action<SendOrPostCallback, object> post;

            public CustomSynchronizationContext(Action<SendOrPostCallback, object> post) 
                => this.post = post;

            public override void Post(SendOrPostCallback d, object state)
                => this.post(d, state);
        }
    }
}
