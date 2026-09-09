using HighTech.Services.Assistant.Providers;

namespace HighTech.Abstraction
{
    public interface IAssistantConversationStore
    {
        IReadOnlyList<AiChatMessage> GetHistory(string userId, string sessionId);
        void Append(string userId, string sessionId, AiChatMessage userMessage, AiChatMessage assistantMessage);
        void Clear(string userId, string sessionId);
        void CleaningOldSession();
    }
}
