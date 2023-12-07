using HighTech.Models;

namespace HighTech.Abstraction
{
    public interface IFieldService
    {
        public ICollection<Field> GetFields();
        public ICollection<Field> GetFieldsByCategory(string id);
        public Field GetField(string id);
        public bool CreateField(string id, TypeCode typeCode);
        public bool EditField(string id, TypeCode typeCode);
        public bool RemoveField(string id);
        public ICollection<ProductField> GetProductFields(string id);
        public ProductField AddProductField(string productId, string fieldId, string value);
        public bool EditProductFieldValue(string pfId, string value);
    }
}
