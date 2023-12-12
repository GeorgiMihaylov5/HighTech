using HighTech.Models;

namespace HighTech.Abstraction
{
    public interface IFieldService
    {
        public ICollection<Field> GetFields();
        public Field GetField(string id);
        public Field CreateField(string name, TypeCode typeCode);
        public Field EditField(string id, string name, TypeCode typeCode);
        public bool RemoveField(string id);
        public ICollection<ProductCategory> GetProductFields(string id);
        public ProductCategory AddProductField(string productId, string categoryId, string value); //Removed fieldId
        public bool EditProductFieldValue(string pfId, string categoryId, string value);
    }
}
