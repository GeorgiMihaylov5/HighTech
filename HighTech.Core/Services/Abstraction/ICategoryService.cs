using HighTech.Core.Entities;

namespace HighTech.Core.Services.Abstraction
{
	public interface ICategoryService
	{
		// Category CRUD
		ICollection<Category> GetAll();
		Category? Get(string id);
		Category? GetByName(string name);
		string? GetCategoryByProduct(string productId);
		Category Create(string name);
		Category? Edit(string id, string name);
		bool Remove(string id);

		// CategoryField management (which fields belong to a category)
		ICollection<CategoryField> GetCategoryFields(string categoryId);
		ICollection<Field> GetAvailableFieldsForCategory(string categoryId);
		CategoryField? AddFieldToCategory(string categoryId, string fieldId);
		bool RemoveFieldFromCategory(string categoryId, string fieldId);
		bool AddMultipleFieldsToCategory(string categoryId, IEnumerable<string> fieldIds);
	}
}
