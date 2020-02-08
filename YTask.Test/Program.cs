using System;
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
    }
}
