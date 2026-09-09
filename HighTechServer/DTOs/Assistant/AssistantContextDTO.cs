namespace HighTech.DTOs.Assistant
{
    public class AssistantContextDTO
    {
        public bool OnConfigurator { get; set; }
        public Dictionary<string, string> CurrentSelection { get; set; } = new();
    }
}
