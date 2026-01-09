using HighTech.Core.Entities;

namespace HighTech.Core.Repositories
{
	public interface IFieldRepository
	{
		public ICollection<Field> GetFields();
		public Field? GetField(string? id);
        public Field? GetFieldByName(string? name);
        ICollection<Field> GetFieldsByCategory(string? categoryId);
        public Field CreateField(string? name, TypeCode typeCode);
		public Field? EditField(string? id, string? name, TypeCode typeCode);
		public bool RemoveField(string? id);

		// ProductFieldValue operations
		public ICollection<ProductFieldValue> GetProductFieldValues(string? productId);
		public ProductFieldValue SetProductFieldValue(string? productId, string? fieldId, string? value);
		public bool RemoveProductFieldValue(string? productId, string? fieldId);
	}
}
