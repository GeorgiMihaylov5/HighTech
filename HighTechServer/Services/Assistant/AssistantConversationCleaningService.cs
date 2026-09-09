using HighTech.Abstraction;

namespace HighTech.Services.Assistant
{
    public class AssistantConversationCleaningService : BackgroundService
    {
        private readonly IAssistantConversationStore store;

        public AssistantConversationCleaningService(IAssistantConversationStore store)
        {
            this.store = store;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));
            while (await timer.WaitForNextTickAsync(stoppingToken))
                store.CleaningOldSession();
        }
    }
}
