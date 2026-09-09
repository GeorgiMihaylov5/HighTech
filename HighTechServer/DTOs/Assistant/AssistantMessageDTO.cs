using System.Text.Json.Serialization;

namespace HighTech.DTOs.Assistant
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(AssistantChatMessageDTO), "chat")]
    [JsonDerivedType(typeof(AssistantPcCreationMessageDTO), "pc-creation")]
    [JsonDerivedType(typeof(AssistantPcModificationMessageDTO), "pc-modification")]
    public abstract class AssistantMessageDTO
    {
        [JsonIgnore]
        public string Type { get; set; }
        public string Rationale { get; set; }
    }

    public class AssistantChatMessageDTO : AssistantMessageDTO
    {
        public AssistantChatMessageDTO() => Type = "chat";
        public string Text { get; set; }
    }

    public class AssistantPcCreationMessageDTO : AssistantMessageDTO
    {
        public AssistantPcCreationMessageDTO() => Type = "pc-creation";
        public Dictionary<string, string> Selection { get; set; } = new();
    }

    public class AssistantPcModificationMessageDTO : AssistantMessageDTO
    {
        public AssistantPcModificationMessageDTO() => Type = "pc-modification";
        public List<AssistantPcModificationChangeDTO> Changes { get; set; } = new();
    }

    public class AssistantPcModificationChangeDTO
    {
        public string CategoryName { get; set; }
        public string ProductId { get; set; }
    }

}
