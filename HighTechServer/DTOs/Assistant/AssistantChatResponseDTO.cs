namespace HighTech.DTOs.Assistant
{
    public class AssistantChatResponseDTO
    {
        public AssistantMessageDTO Message { get; set; }

        // Manufacturer / model / price for every productId referenced by Message.
        // Empty for chat messages.
        public Dictionary<string, AssistantProductSummaryDTO> Products { get; set; } = new();
    }
}
