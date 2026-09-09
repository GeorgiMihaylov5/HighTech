using System.ClientModel;
using HighTech.Abstraction;
using HighTech.Options;
using OpenAI.Chat;

namespace HighTech.Services.Assistant.Providers
{
    public class OpenAiChatProvider : IAiChatProvider
    {
        private readonly ChatClient chatClient;
        private readonly AssistantOptions options;
        private readonly ILogger<OpenAiChatProvider> logger;

        private const int MaxAttempts = 3;

        public OpenAiChatProvider(AssistantOptions options, ILogger<OpenAiChatProvider> logger)
        {
            this.options = options;
            this.logger = logger;
            chatClient = new ChatClient(model: options.Model, apiKey: options.ApiKey);
        }

        public string ProviderName => "OpenAI";

        public async Task<string> CompleteJsonAsync(AiChatRequest request, CancellationToken ct = default)
        {
            var messages = BuildMessages(request);
            var completionOptions = BuildOptions(request);

            var delay = TimeSpan.FromSeconds(1);
            for (int attempt = 1; attempt <= MaxAttempts; attempt++)
            {
                try
                {
                    var completion = await chatClient.CompleteChatAsync(messages, completionOptions, ct);
                    var parts = completion.Value.Content;
                    if (parts == null || parts.Count == 0)
                        throw new InvalidOperationException("Empty completion from OpenAI.");
                    return parts[0].Text;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex) when (attempt < MaxAttempts && IsTransient(ex))
                {
                    logger.LogWarning(ex, "OpenAI transient error on attempt {Attempt}/{Max}; retrying in {Delay}s.", attempt, MaxAttempts, delay.TotalSeconds);
                    await Task.Delay(delay, ct);
                    delay = TimeSpan.FromTicks(delay.Ticks * 2);
                }
            }

            // Final attempt — let the exception propagate.
            {
                var completion = await chatClient.CompleteChatAsync(messages, completionOptions, ct);
                var parts = completion.Value.Content;
                if (parts == null || parts.Count == 0)
                    throw new InvalidOperationException("Empty completion from OpenAI.");
                return parts[0].Text;
            }
        }

        private static List<ChatMessage> BuildMessages(AiChatRequest request)
        {
            var messages = new List<ChatMessage>(request.Messages.Count);
            foreach (var msg in request.Messages)
            {
                messages.Add(msg.Role switch
                {
                    "system" => ChatMessage.CreateSystemMessage(msg.Content),
                    "user" => ChatMessage.CreateUserMessage(msg.Content),
                    "assistant" => ChatMessage.CreateAssistantMessage(msg.Content),
                    _ => throw new ArgumentException($"Unknown role: {msg.Role}")
                });
            }
            return messages;
        }

        private static ChatCompletionOptions BuildOptions(AiChatRequest request)
        {
            return new ChatCompletionOptions
            {
                Temperature = 1f,
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: request.SchemaName,
                    jsonSchema: BinaryData.FromString(request.ResponseJsonSchema),
                    jsonSchemaIsStrict: true)
            };
        }

        private static bool IsTransient(Exception ex)
        {
            if (ex is HttpRequestException) return true;
            if (ex is ClientResultException cre)
                return cre.Status == 429 || cre.Status >= 500;
            return false;
        }
    }
}
