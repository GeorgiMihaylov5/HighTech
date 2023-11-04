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
        public ProductField AddProductField(string productId, string fieldId, string value)
        {
            var field = new ProductField()
            {
                ProductId = productId,
                FieldId = fieldId,
                Value = value,
            };

            context.ProductsFields.Add(field);
            context.SaveChanges();

            return field;
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

        public bool EditProductFieldValue(string pfId, string value)
        {
            var productField = context.ProductsFields.FirstOrDefault(x => x.Id == pfId);

            if (productField is null)
            {
                return false;
            }

            productField.Value = value;

            return context.SaveChanges() != 0;
        }

        public Field GetField(string id)
        {
            return context.Fields.FirstOrDefault(x => x.Id == id);
        }

        public ICollection<Field> GetFieldsByCategory(string id)
        {
            return context.Fields
                .Include(x => x.ProductsFields)
                .Where(f => f.Categories!.Select(x => x.FieldId).Contains(f.Id))
                .ToList();
        }

        public ICollection<ProductField> GetProductFields(string id)
        {
            return context.ProductsFields
                .Include(pf => pf.Field)
                .Where(x => x.ProductId == id).ToList();
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
