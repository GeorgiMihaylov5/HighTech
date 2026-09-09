using System.Collections.Concurrent;
using HighTech.Abstraction;
using HighTech.Options;
using HighTech.Services.Assistant.Providers;
using Microsoft.Extensions.Options;

namespace HighTech.Services.Assistant
{
    public class AssistantConversationStore : IAssistantConversationStore
    {
        //sessionId -> conversation history
        private readonly ConcurrentDictionary<string, ConversationEntry> entries = new();
        // userId -> sessionId
        private readonly ConcurrentDictionary<string, string> userSessions = new();
        private readonly AssistantOptions options;

        public AssistantConversationStore(IOptions<AssistantOptions> options)
        {
            this.options = options.Value;
        }

        public IReadOnlyList<AiChatMessage> GetHistory(string userId, string sessionId)
        {
            ClearStaleSessionForUser(userId, sessionId);

            if (!entries.TryGetValue(sessionId, out var entry))
                return Array.Empty<AiChatMessage>();

            lock (entry.Lock)
            {
                entry.LastTouchedUtc = DateTime.UtcNow;
                var max = options.MaxHistoryMessages;
                if (entry.Messages.Count <= max)
                    return entry.Messages.ToArray();
                return entry.Messages.Skip(entry.Messages.Count - max).ToArray();
            }
        }

        public void Append(string userId, string sessionId, AiChatMessage userMessage, AiChatMessage assistantMessage)
        {
            ClearStaleSessionForUser(userId, sessionId);

            var entry = entries.GetOrAdd(sessionId, _ => new ConversationEntry());
            lock (entry.Lock)
            {
                entry.Messages.Add(userMessage);
                entry.Messages.Add(assistantMessage);
                entry.LastTouchedUtc = DateTime.UtcNow;

                var maxHistoryMessages = Math.Max(1, options.MaxHistoryMessages);
                if (entry.Messages.Count > maxHistoryMessages)
                {
                    var excess = entry.Messages.Count - maxHistoryMessages;
                    entry.Messages.RemoveRange(0, excess);
                }
            }
        }

        public void Clear(string userId, string sessionId)
        {
            entries.TryRemove(sessionId, out _);
            RemoveUserSession(userId, sessionId);
        }

        // Called by background service.
        public void CleaningOldSession()
        {
            var idleCutoff = DateTime.UtcNow - TimeSpan.FromMinutes(options.ConversationIdleTimeoutMinutes);
            foreach (var pair in entries)
            {
                if (pair.Value.LastTouchedUtc < idleCutoff)
                {
                    entries.TryRemove(pair.Key, out _);
                    RemoveSessionMappings(pair.Key);
                }
            }
        }

        private void ClearStaleSessionForUser(string userId, string sessionId)
        {
            var oldSessionId = userSessions.GetOrAdd(userId, sessionId);
            if (oldSessionId == sessionId) return;

            entries.TryRemove(oldSessionId, out _);
            userSessions.TryUpdate(userId, sessionId, oldSessionId);
        }

        private void RemoveUserSession(string userId, string sessionId)
        {
            ((ICollection<KeyValuePair<string, string>>)userSessions)
                .Remove(new KeyValuePair<string, string>(userId, sessionId));
        }

        private void  RemoveSessionMappings(string sessionId)
        {
            foreach (var pair in userSessions)
            {
                if (pair.Value == sessionId)
                    RemoveUserSession(pair.Key, sessionId);
            }
        }

        private class ConversationEntry
        {
            public List<AiChatMessage> Messages { get; } = new();
            public DateTime LastTouchedUtc { get; set; } = DateTime.UtcNow;
            public object Lock { get; } = new();
        }
    }
}
