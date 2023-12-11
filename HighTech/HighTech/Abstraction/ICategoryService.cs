using HighTech.Models;

namespace HighTech.Abstraction
{
    public interface ICategoryService
    {
        public ICollection<Category> GetAll();
        public Category Get(string categoryId, string fieldId);
        public Category GetCategoryByProduct(string id);
        public bool RemoveCategories(string categoryId);
        public Category CreateCategoryField(string categoryId, string fieldId);
        public bool RemoveCategoryField(string categoryId, string fieldId);
    }
}
