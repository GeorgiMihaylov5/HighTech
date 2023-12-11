using HighTech.Models;

namespace HighTech.DTOs
{
    public class CategoryDTO
    {
        public string CategoryId { get; set; }
        public ICollection<FieldDTO> Fields { get; set; }
    }
}
