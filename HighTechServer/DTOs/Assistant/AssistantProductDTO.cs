namespace HighTech.DTOs.Assistant
{
    public class AssistantProductDTO
    {
        public string Id { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public List<AssistantProductFieldDTO> Fields { get; set; } = new();
    }

    public class AssistantProductFieldDTO
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
