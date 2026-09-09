using System.Collections.Concurrent;
using HighTech.Abstraction;
using HighTech.Options;
using Microsoft.Extensions.Options;

namespace HighTech.Services.Assistant
{
    public class AssistantRateLimiter : IAssistantRateLimiter
    {
        private readonly ConcurrentDictionary<string, Queue<DateTime>> windows = new();
        private readonly AssistantOptions options;
        private static readonly TimeSpan Window = TimeSpan.FromHours(1);

        public AssistantRateLimiter(IOptions<AssistantOptions> options)
        {
            this.options = options.Value;
        }

        public bool TryConsume(string userId)
        {
            var queue = windows.GetOrAdd(userId, _ => new Queue<DateTime>());
            var now = DateTime.UtcNow;
            var cutoff = now - Window;

            lock (queue)
            {
                while (queue.Count > 0 && queue.Peek() < cutoff)
                    queue.Dequeue();

                if (queue.Count >= options.RateLimitPerUserPerHour)
                    return false;

                queue.Enqueue(now);
                return true;
            }
        }
    }
}
