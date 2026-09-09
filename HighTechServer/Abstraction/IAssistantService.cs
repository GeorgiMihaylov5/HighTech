using HighTech.DTOs.Assistant;

namespace HighTech.Abstraction
{
    public interface IAssistantService
    {
        Task<AssistantChatResponseDTO> ChatAsync(AssistantChatRequestDTO request, string userId, string sessionId, CancellationToken ct = default);
    }
}
