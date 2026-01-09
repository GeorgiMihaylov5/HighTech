using HighTech.Core.Entities;

namespace HighTech.Core.Services.Abstraction
{
	public interface IFieldService
	{
		// Field CRUD
		ICollection<Field> GetFields();
		Field? GetField(string? id);
		Field? GetFieldByName(string? name);
		Field CreateField(string? name, TypeCode typeCode);
		Field EditField(string? id, string? name, TypeCode typeCode);
		bool RemoveField(string? id);

		// Get fields by category (useful for UI)
		ICollection<Field> GetFieldsByCategory(string? categoryId);
	}
}
