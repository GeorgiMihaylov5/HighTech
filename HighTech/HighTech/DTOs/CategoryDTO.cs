namespace HighTech.DTOs
{
    public class CategoryDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<FieldDTO> Fields { get; set; }
    }
}
