using HighTech.Models;

namespace HighTech.Abstraction
{
    public interface IFieldService
    {
        public ICollection<Field> GetFields();
        public Field GetField(string id);
        public Field CreateField(string name, TypeCode typeCode);
        public Field EditField(string id, string name, TypeCode typeCode);
        public bool IsFieldUsedByCategory(string id);
        public bool RemoveField(string id);
        public ICollection<ProductFieldValue> GetProductFields(string id);
        public ProductFieldValue AddProductField(string productId, string categoryFieldId, string value);
        public bool EditProductFieldValue(string pfId, string categoryFieldId, string value);
        public bool RemoveProductFieldsExceptCategory(string productId, string categoryName);
    }
}
