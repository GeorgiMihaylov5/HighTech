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
        public ProductCategory AddProductField(string productId, string categoryId, string fieldId, string value)
        {
            var field = new ProductCategory()
            {
                ProductId = productId,
                CategoryFieldId = fieldId,
                CategoryId = categoryId,
                Value = value,
            };

            context.ProductsCategories.Add(field);
            context.SaveChanges();

            return field;
        }

        public Field CreateField(string id, TypeCode typeCode)
        {
            var field = new Field()
            {
                Id = id,
                TypeCode = typeCode
            };

            context.Fields.Add(field);
            context.SaveChanges();

            return field;
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
            var productField = context.ProductsCategories.FirstOrDefault(x => x.Id == pfId);

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

        public ICollection<Field> GetFields()
        {
            return context.Fields.ToList();
        }

        public ICollection<ProductCategory> GetProductFields(string id)
        {
            return context.ProductsCategories
                .Include(pf => pf.Category.Field)
                .Where(x => x.ProductId == id).ToList();
        }

        public bool RemoveField(string id)
        {
            var field = GetField(id);

            if (field is null)
            {
                return false;
            }

            context.Fields.Remove(field);
            return context.SaveChanges() != 0;
        }
    }
}
