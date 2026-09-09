using HighTech.DTOs;
using HighTech.Models;

namespace HighTech.Abstraction
{
    public interface ICategoryService
    {
        public ICollection<Category> GetAll();
        public Category Get(string id);
        public Category GetByName(string name);
        public bool ExistsByName(string name, string excludedId = null);
        public CategoryField GetCategoryField(string categoryName, string fieldId);
        public string GetCategoryByProduct(string id);
        public bool IsCategoryUsedByProduct(string id);
        public CategoryRemovalPreviewDTO PreviewFieldRemoval(string id, ICollection<string> removedFieldIds);
        public CategoryRemovalPreviewDTO PreviewCategoryRemoval(string id);
        public Category Create(string name, ICollection<string> fieldIds);
        public Category Edit(string id, string name, ICollection<string> fieldIds);
        public bool Remove(string id);
    }
}
