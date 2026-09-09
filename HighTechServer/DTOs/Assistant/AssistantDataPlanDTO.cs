namespace HighTech.DTOs.Assistant
{
    public class AssistantDataPlanDTO
    {
        public List<AssistantDataPlanFetchDTO> Fetches { get; set; } = new();
        public bool NeedsCurrentSelection { get; set; }
        public bool NeedsCompatibleParts { get; set; }
    }
}
