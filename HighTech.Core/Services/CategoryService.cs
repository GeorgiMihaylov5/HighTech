using HighTech.Core.Entities;
using HighTech.Core.Repositories;
using HighTech.Core.Services.Abstraction;

namespace HighTech.Core.Services
{
	public class CategoryService : ICategoryService
	{
		private readonly ICategoryRepository categoryRepository;

		public CategoryService(ICategoryRepository _categoryRepository)
		{
			categoryRepository = _categoryRepository;
		}

		public Category CreateCategoryField(string name, string fieldId)
		{
			return categoryRepository.CreateCategoryField(name, fieldId);
		}

		public ICollection<Category> EditCategoryName(string id, string name)
		{
			return categoryRepository.EditCategoryName(id, name);
		}

		public Category Get(string name, string fieldId)
		{
			return categoryRepository.Get(name, fieldId);
		}

		public ICollection<Category> GetAll()
		{
			return categoryRepository.GetAll();
		}

		public ICollection<Category> GetAllByName(string name)
		{
			return categoryRepository.GetAllByName(name);
		}

		public string GetCategoryByProduct(string id)
		{
			return categoryRepository.GetCategoryByProduct(id);
		}

		public bool RemoveCategory(string id)
		{
			return categoryRepository.RemoveCategory(id);
		}

		public bool RemoveCategoryByName(string name)
		{
			return categoryRepository.RemoveCategoryByName(name);
		}

		public bool RemoveCategoryField(string name, string fieldId)
		{
			return categoryRepository.RemoveCategoryField(name, fieldId);
		}
	}
}
