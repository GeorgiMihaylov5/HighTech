using HighTech.Models;

namespace HighTech.DTOs
{
    public class CategoryDTO
    {
        public string CategoryId { get; set; }
        public string FieldId { get; set; }
        public virtual Field Field { get; set; }
    }
}
