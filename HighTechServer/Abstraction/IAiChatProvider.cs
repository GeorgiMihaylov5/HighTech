using HighTech.Services.Assistant.Providers;

namespace HighTech.Abstraction
{
    public interface IAiChatProvider
    {
        string ProviderName { get; }
        Task<string> CompleteJsonAsync(AiChatRequest request, CancellationToken ct = default);
    }
}
