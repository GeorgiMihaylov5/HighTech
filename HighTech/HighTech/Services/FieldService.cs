using HighTech.Abstraction;
using HighTech.Data;
using HighTech.Models;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Services
{
    public class FieldService : IFieldService
    {
        private readonly ApplicationDbContext context;

        public FieldService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public bool CreateField(string id, TypeCode typeCode)
        {
            context.Fields.Add(new Field()
            {
                Id = id,
                TypeCode = typeCode
            });

            return context.SaveChanges() != 0;
        }

        public bool EditField(string id,TypeCode typeCode)
        {
            var field = context.Fields.FirstOrDefault(x => x.Id == id);

            if (field is null)
            {
                return false;
            }

            field.TypeCode = typeCode;

            return context.SaveChanges() != 0;
        }

        public Field? GetField(string id)
        {
            return context.Fields.FirstOrDefault(x => x.Id == id);
        }

        public ICollection<Field> GetFieldsByCategory(string id)
        {
            return context.Fields
                .Include(x => x.ProductsFields)
                .Where(f => f.CategoryFields!.Select(x => x.FieldId).Contains(f.Id))
                .ToList();
        }

        public bool RemoveField(string id)
        {
            var field = GetField(id);

            if (field is null)
            {
                return false;
            }

            context.Fields .Remove(field);
            return context.SaveChanges() != 0;
        }
    }
}
