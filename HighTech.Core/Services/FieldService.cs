using HighTech.Core.Entities;
using HighTech.Core.Repositories;
using HighTech.Core.Services.Abstraction;

namespace HighTech.Core.Services
{
	public class FieldService : IFieldService
	{
		private readonly IFieldRepository fieldRepository;
		private readonly ICategoryRepository categoryRepository;

		public FieldService(IFieldRepository _fieldRepository, ICategoryRepository _categoryRepository)
		{
			fieldRepository = _fieldRepository;
			categoryRepository = _categoryRepository;
		}

		// Field CRUD
		public Field CreateField(string name, TypeCode typeCode)
		{
			// Business validation: check if field with same name already exists
			var existing = GetFieldByName(name);
			if (existing != null)
			{
				throw new InvalidOperationException($"Field with name '{name}' already exists.");
			}

			return fieldRepository.CreateField(name, typeCode);
		}

		public Field? EditField(string id, string name, TypeCode typeCode)
		{
			// Business validation: check if another field with same name exists
			var existing = GetFieldByName(name);
			if (existing != null && existing.Id != id)
			{
				throw new InvalidOperationException($"Field with name '{name}' already exists.");
			}

			return fieldRepository.EditField(id, name, typeCode);
		}

		public Field? GetField(string id)
		{
			return fieldRepository.GetField(id);
		}

		public Field? GetFieldByName(string name)
		{
			return fieldRepository.GetFields().FirstOrDefault(f => f.Name == name);
		}

		public ICollection<Field> GetFields()
		{
			return fieldRepository.GetFields();
		}

		public bool RemoveField(string id)
		{
			return fieldRepository.RemoveField(id);
		}

		// Get fields by category
		public ICollection<Field> GetFieldsByCategory(string categoryId)
		{
			var categoryFields = categoryRepository.GetCategoryFields(categoryId);
			return categoryFields.Select(cf => cf.Field!).ToList();
		}
	}
}
