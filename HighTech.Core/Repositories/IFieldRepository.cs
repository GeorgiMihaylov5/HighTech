using HighTech.Core.Entities;

namespace HighTech.Core.Repositories
{
	public interface IFieldRepository
	{
		public ICollection<Field> GetAll();
		public Field? Get(string? id);
        public Field? GetFieldByName(string? name);
        ICollection<Field> GetFieldsByCategory(string? categoryId);
        public Field Create(string? name, TypeCode typeCode);
		public Field? Edit(string? id, string? name, TypeCode typeCode);
		public bool SoftDelete(string? id);
		public ProductFieldValue SetProductFieldValue(string? productId, string? fieldId, string? value);
	}
}
