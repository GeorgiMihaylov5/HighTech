using HighTech.Models;

namespace HighTech.Abstraction
{
    public interface ICategoryService
    {
        public ICollection<Category> GetCategories();
        public Category? GetCategory(string id);
        public bool CreateCategory(string id);
        public bool EditCategory(string id);
        public bool RemoveCategory(string id);
    }
}
