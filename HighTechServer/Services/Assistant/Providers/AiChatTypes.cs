namespace HighTech.Services.Assistant.Providers
{
    public record AiChatRequest(
        IReadOnlyList<AiChatMessage> Messages,
        string ResponseJsonSchema,
        string SchemaName,
        string Model
    );

    // Role: "system" | "user" | "assistant"
    public record AiChatMessage(string Role, string Content);
}
