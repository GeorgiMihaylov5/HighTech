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

		public ProductCategory AddProductField(string productId, string categoryId, string value)
		{
			return fieldRepository.AddProductField(productId, categoryId, value);
		}

		public Field CreateField(string name, TypeCode typeCode)
		{
			return fieldRepository.CreateField(name, typeCode);
		}

		public Field EditField(string id, string name, TypeCode typeCode)
		{
			return fieldRepository.EditField(id, name, typeCode);
		}

		public bool EditProductFieldValue(string pfId, string categoryId, string value)
		{
			return fieldRepository.EditProductFieldValue(pfId, categoryId, value);
		}

		public Field GetField(string id)
		{
			return fieldRepository.GetField(id);
		}

		public ICollection<Field> GetFields()
		{
			return fieldRepository.GetFields();
		}

		public ICollection<ProductCategory> GetProductFields(string id)
		{
			return fieldRepository.GetProductFields(id);
		}

		public bool RemoveField(string id)
		{
			return fieldRepository.RemoveField(id);
		}
	}
}
