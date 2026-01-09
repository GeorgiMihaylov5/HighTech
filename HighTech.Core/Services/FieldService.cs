using HighTech.Core.Entities;
using HighTech.Core.Repositories;
using HighTech.Core.Services.Abstraction;

namespace HighTech.Core.Services
{
	public class FieldService : IFieldService
	{
		private readonly IFieldRepository fieldRepository;

		public FieldService(IFieldRepository _fieldRepository)
		{
			fieldRepository = _fieldRepository;
		}

		public Field CreateField(string? name, TypeCode typeCode)
		{
			var existing = GetFieldByName(name);
			if (existing != null)
			{
                //TODO custom exception
                throw new InvalidOperationException($"Field with name '{name}' already exists.");
			}

			return fieldRepository.CreateField(name, typeCode);
		}

		public Field EditField(string? id, string? name, TypeCode typeCode)
		{
			Guard.NotNullOrEmpty(id, nameof(id));
			Guard.NotNullOrEmpty(name, nameof(name));

            return fieldRepository.EditField(id, name, typeCode)
				?? throw new NullReferenceException();
		}

		public Field? GetField(string? id)
		{
			return fieldRepository.GetField(id);
		}

		public Field? GetFieldByName(string? name)
		{
			return fieldRepository.GetFieldByName(name);
        }

		public ICollection<Field> GetFields()
		{
			return fieldRepository.GetFields();
		}

		public bool RemoveField(string? id)
		{
			return fieldRepository.RemoveField(id);
		}

		public ICollection<Field> GetFieldsByCategory(string? categoryId)
		{
			return fieldRepository.GetFieldsByCategory(categoryId);
        }
	}
}
