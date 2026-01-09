using HighTech.Core.Entities;

namespace HighTech.Core.Services.Abstraction
{
	public interface ICategoryService
	{
		ICollection<Category> GetAll();
		Category? Get(string? id);
		Category? GetByName(string? name);
		Category Create(string? name);
		Category Edit(string? id, string? name);
		bool Remove(string? id);
		ICollection<CategoryField> GetCategoryFields(string? categoryId);
		CategoryField? AddFieldToCategory(string? categoryId, string? fieldId);
		bool RemoveFieldFromCategory(string? categoryId, string? fieldId);
	}
}
