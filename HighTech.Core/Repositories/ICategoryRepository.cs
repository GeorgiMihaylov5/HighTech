using HighTech.Core.Entities;

namespace HighTech.Core.Repositories
{
	public interface ICategoryRepository
	{
		public ICollection<Category> GetAll();
		public ICollection<Category> GetAllByName(string name);
		public Category? Get(string name, string fieldId);
		public string? GetCategoryByProduct(string id);
		public Category CreateCategoryField(string name, string fieldId);
		public ICollection<Category> EditCategoryName(string id, string name);
		public bool RemoveCategoryField(string name, string fieldId);
		public bool RemoveCategory(string id);
		public bool RemoveCategoryByName(string name);

	}
}
