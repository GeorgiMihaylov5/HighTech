using HighTech.Models;

namespace HighTech.Abstraction
{
    public interface IFieldService
    {
        public ICollection<Field> GetFieldsByCategory(string id);
        public Field GetField(string id);
        public bool CreateField(string id, TypeCode typeCode);
        public bool EditField(string id, TypeCode typeCode);
        public bool RemoveField(string id);
    }
}
