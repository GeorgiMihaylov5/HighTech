using HighTech.Models;

namespace HighTech.DTOs
{
    public class CategoryDTO
    {
        public string CategoryId { get; set; }
        public IEnumerable<FieldDTO> Fields { get; set; }
    }
}
