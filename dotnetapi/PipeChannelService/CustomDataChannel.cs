
namespace PipeChannelService
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class CustomDataChannel
    {
        readonly SemaphoreSlim semaphoreSlim;
        readonly Queue<string> queue;

        public CustomDataChannel()
        {
            this.semaphoreSlim = new SemaphoreSlim(0);

            this.queue = new Queue<string>();
        }

        public async Task<string> ReadAsync()
        {
            await semaphoreSlim.WaitAsync();

            queue.TryDequeue(out var message);

            return message;
        }

        public Task WriterAsync(string message)
        {
            queue.Enqueue(message);

            semaphoreSlim.Release();

            return Task.CompletedTask;

        }
 
    }
}