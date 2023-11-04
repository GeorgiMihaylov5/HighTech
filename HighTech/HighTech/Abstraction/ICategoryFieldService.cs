using HighTech.Models;

namespace HighTech.Abstraction
{
    public interface ICategoryFieldService
    {
        public ICollection<CategoryField> GetAll();
        public bool CreateCategoryField(string categoryId, string fieldId);
        public bool RemoveCategoryField(string categoryId, string fieldId);
    }
}
