using HighTech.Core.Entities;

namespace HighTech.Core.Repositories
{
	public interface IFieldRepository
	{
		public ICollection<Field> GetFields();
		public Field? GetField(string id);
		public Field CreateField(string name, TypeCode typeCode);
		public Field? EditField(string id, string name, TypeCode typeCode);
		public bool RemoveField(string id);

		// ProductFieldValue operations
		public ICollection<ProductFieldValue> GetProductFieldValues(string productId);
		public ProductFieldValue? AddProductFieldValue(string productId, string fieldId, string value);
		public bool EditProductFieldValue(string productId, string fieldId, string value);
		public bool RemoveProductFieldValue(string productId, string fieldId);
	}
}
