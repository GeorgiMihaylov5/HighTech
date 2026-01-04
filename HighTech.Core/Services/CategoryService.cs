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

		// Category CRUD
		public Category Create(string name)
		{
			// Business validation: check if category with same name already exists
			var existing = categoryRepository.GetByName(name);
			if (existing != null)
			{
				throw new InvalidOperationException($"Category with name '{name}' already exists.");
			}

			return categoryRepository.Create(name);
		}

		public Category? Edit(string id, string name)
		{
			// Business validation: check if another category with same name exists
			var existing = categoryRepository.GetByName(name);
			if (existing != null && existing.Id != id)
			{
				throw new InvalidOperationException($"Category with name '{name}' already exists.");
			}

			return categoryRepository.Edit(id, name);
		}

		public Category? Get(string id)
		{
			return categoryRepository.Get(id);
		}

		public Category? GetByName(string name)
		{
			return categoryRepository.GetByName(name);
		}

		public ICollection<Category> GetAll()
		{
			return categoryRepository.GetAll();
		}

		public string? GetCategoryByProduct(string productId)
		{
			return categoryRepository.GetCategoryByProduct(productId);
		}

		public bool Remove(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw new ArgumentException("Category ID is required.", nameof(id));
			}

			// Business rule: Check if there are products using this category
			var categoryProducts = categoryRepository.GetCategoryByProduct(id);

			// Alternative check - get all products in this category
			var category = categoryRepository.Get(id);
			if (category == null)
			{
				return false;
			}

			// Check if category has products (business validation)
			// Note: This requires a method to check product count by category
			// For now, we'll allow deletion and let FK constraints handle it
			// In production, you'd want to add a method like: productRepository.CountByCategory(id)

			return categoryRepository.Remove(id);
		}

		// CategoryField management
		public ICollection<CategoryField> GetCategoryFields(string categoryId)
		{
			return categoryRepository.GetCategoryFields(categoryId);
		}

		public ICollection<Field> GetAvailableFieldsForCategory(string categoryId)
		{
			// Get all fields that are NOT already assigned to this category
			var allFields = fieldRepository.GetFields();
			var assignedFieldIds = categoryRepository.GetCategoryFields(categoryId)
				.Select(cf => cf.FieldId)
				.ToHashSet();

			return allFields.Where(f => !assignedFieldIds.Contains(f.Id!)).ToList();
		}

		public CategoryField? AddFieldToCategory(string categoryId, string fieldId)
		{
			// Business validation: verify category exists
			var category = categoryRepository.Get(categoryId);
			if (category == null)
			{
				throw new InvalidOperationException($"Category with ID '{categoryId}' does not exist.");
			}

			// Business validation: verify field exists
			var field = fieldRepository.GetField(fieldId);
			if (field == null)
			{
				throw new InvalidOperationException($"Field with ID '{fieldId}' does not exist.");
			}

			return categoryRepository.AddFieldToCategory(categoryId, fieldId);
		}

		public bool RemoveFieldFromCategory(string categoryId, string fieldId)
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

		public bool AddMultipleFieldsToCategory(string categoryId, IEnumerable<string> fieldIds)
		{
			if (string.IsNullOrWhiteSpace(categoryId))
			{
				throw new ArgumentException("Category ID is required.", nameof(categoryId));
			}

			if (fieldIds == null || !fieldIds.Any())
			{
				throw new ArgumentException("At least one field ID is required.", nameof(fieldIds));
			}

			// Verify category exists
			var category = categoryRepository.Get(categoryId);
			if (category == null)
			{
				throw new InvalidOperationException($"Category with ID '{categoryId}' does not exist.");
			}

			// Add each field
			var successCount = 0;
			foreach (var fieldId in fieldIds)
			{
				try
				{
					var result = AddFieldToCategory(categoryId, fieldId);
					if (result != null)
					{
						successCount++;
					}
				}
				catch (InvalidOperationException)
				{
					// Field doesn't exist or already assigned, continue
					continue;
				}
			}

			return successCount > 0;
		}
	}
}
