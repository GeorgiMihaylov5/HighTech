namespace HighTech.DTOs.Assistant
{
    public class AssistantChatRequestDTO
    {
        public string SessionId { get; set; }
        public string Message { get; set; }
        public AssistantContextDTO Context { get; set; }
    }
}
