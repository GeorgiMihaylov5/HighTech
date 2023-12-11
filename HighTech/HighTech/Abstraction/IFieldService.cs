using HighTech.Models;

namespace HighTech.Abstraction
{
    public interface IFieldService
    {
        public ICollection<Field> GetFields();
        public Field GetField(string id);
        public Field CreateField(string id, TypeCode typeCode);
        public bool EditField(string id, TypeCode typeCode);
        public bool RemoveField(string id);
        public ICollection<ProductCategory> GetProductFields(string id);
        public ProductCategory AddProductField(string productId, string categoryId, string fieldId, string value);
        public bool EditProductFieldValue(string pfId, string value);
    }
}
