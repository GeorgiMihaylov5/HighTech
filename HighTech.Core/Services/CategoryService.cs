using HighTech.Core.Entities;
using HighTech.Core.Repositories;
using HighTech.Core.Services.Abstraction;

namespace HighTech.Core.Services
{
	public class CategoryService : ICategoryService
	{
		private readonly ICategoryRepository categoryRepository;
		private readonly IFieldRepository fieldRepository;

		public CategoryService(ICategoryRepository _categoryRepository, IFieldRepository _fieldRepository)
		{
			categoryRepository = _categoryRepository;
			fieldRepository = _fieldRepository;
		}

		public Category Create(string? name)
		{
			var existing = categoryRepository.GetByName(name);
			if (existing != null)
			{
				//TODO add custom exception
				throw new InvalidOperationException($"Category with name '{name}' already exists.");
			}

			return categoryRepository.Create(name);
		}

		public Category Edit(string? id, string? name)
		{
			var existing = categoryRepository.GetByName(name);
			if (existing != null && existing.Id != id)
			{
				//TODO add custom exception
				throw new InvalidOperationException($"Category with name '{name}' already exists.");
			}

			return categoryRepository.Edit(id, name)
				?? throw new NullReferenceException();
		}

		public Category? Get(string? id)
		{
			return categoryRepository.Get(id);
		}

		public Category? GetByName(string? name)
		{
			return categoryRepository.GetByName(name);
		}

		public ICollection<Category> GetAll()
		{
			return categoryRepository.GetAll();
		}

		public bool Remove(string? id)
		{
			return categoryRepository.SoftDelete(id);
		}

		public ICollection<CategoryField> GetCategoryFields(string? categoryId)
		{
			return categoryRepository.GetCategoryFields(categoryId);
		}

		public CategoryField? AddFieldToCategory(string? categoryId, string? fieldId)
		{
			var category = categoryRepository.Get(categoryId);
			Guard.NotNull(category, nameof(category));

			var field = fieldRepository.Get(fieldId);
            Guard.NotNull(field, nameof(field));

            return categoryRepository.AddFieldToCategory(categoryId, fieldId);
		}

		public bool RemoveFieldFromCategory(string? categoryId, string? fieldId)
		{
			if (string.IsNullOrWhiteSpace(categoryId))
			{
				throw new ArgumentException("Category ID is required.", nameof(categoryId));
			}

			if (string.IsNullOrWhiteSpace(fieldId))
			{
				throw new ArgumentException("Field ID is required.", nameof(fieldId));
			}

			return categoryRepository.RemoveFieldFromCategory(categoryId, fieldId);
		}
	}
}
