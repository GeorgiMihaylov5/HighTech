using HighTech.Core.Entities;

namespace HighTech.Core.Repositories
{
	public interface ICategoryRepository
	{
		public ICollection<Category> GetAll();
		public Category? Get(string id);
		public Category? GetByName(string name);
		public string? GetCategoryByProduct(string productId);
		public Category Create(string name);
		public Category? Edit(string id, string name);
		public bool Remove(string id);

		// CategoryField operations
		public ICollection<CategoryField> GetCategoryFields(string categoryId);
		public CategoryField? AddFieldToCategory(string categoryId, string fieldId);
		public bool RemoveFieldFromCategory(string categoryId, string fieldId);
	}
}
