namespace HighTech.DTOs.Assistant
{
    public class AssistantFetchedDataDTO
    {
        public Dictionary<string, List<AssistantProductDTO>> Products { get; set; } = new();
        public List<AssistantProductDTO> ContextProducts { get; set; } = new();

        // Full specs (category -> product with its Fields) for the parts already in the user's build.
        // Lets the model verify compatibility rules against parts it is keeping instead of bailing
        // because it only had their ids. The bare id map is not duplicated here — it already travels
        // in every turn as context.currentSelection.
        public Dictionary<string, AssistantProductDTO> CurrentSelectionProducts { get; set; }
        public Dictionary<string, List<AssistantProductDTO>> CompatibleParts { get; set; }
    }
}
