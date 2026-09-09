using HighTech.Abstraction;
using HighTech.Data;
using HighTech.Exceptions;
using HighTech.Models;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Services
{
    public class FieldService : IFieldService
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguratorMetadataGuard guard;

        public FieldService(ApplicationDbContext _context, IConfiguratorMetadataGuard _guard)
        {
            context = _context;
            guard = _guard;
        }
        public ProductFieldValue AddProductField(string productId, string categoryFieldId, string value)
        {
            var field = new ProductFieldValue()
            {
                ProductId = productId,
                CategoryFieldId = categoryFieldId,
                Value = value,
            };

            context.ProductFieldValues.Add(field);
            context.SaveChanges();

            return field;
        }

        public Field CreateField(string name, TypeCode typeCode)
        {
            var field = new Field()
            {
                Name = name,
                TypeCode = typeCode
            };

            context.Fields.Add(field);
            context.SaveChanges();

            return field;
        }

        public Field EditField(string id, string name, TypeCode typeCode)
        {
            var field = context.Fields.FirstOrDefault(x => x.Id == id);

            if (field is null)
            {
                return field;
            }

            if (guard.IsSystemFieldName(field.Name))
            {
                if (!string.Equals(name, field.Name, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ConfiguratorMetadataException(
                        $"Field '{field.Name}' cannot be renamed — it is {guard.LockReason}.");
                }

                if (typeCode != field.TypeCode)
                {
                    throw new ConfiguratorMetadataException(
                        $"Field '{field.Name}' type cannot be changed — it is {guard.LockReason}.");
                }
            }

            field.TypeCode = typeCode;
            field.Name = name;
            context.SaveChanges();

            return field;
        }

        public bool EditProductFieldValue(string pfId, string categoryFieldId, string value)
        {
            var productField = context.ProductFieldValues.FirstOrDefault(x => x.Id == pfId);

            if (productField is null)
            {
                return false;
            }

            productField.CategoryFieldId = categoryFieldId;
            productField.Value = value;

            return context.SaveChanges() != 0;
        }

        public bool RemoveProductFieldsExceptCategory(string productId, string categoryName)
        {
            var productFields = context.ProductFieldValues
                .Include(pf => pf.CategoryField)
                    .ThenInclude(cf => cf.Category)
                .Where(pf => pf.ProductId == productId
                    && pf.CategoryField.Category.Name != categoryName)
                .ToList();

            if (productFields.Count == 0)
            {
                return false;
            }

            context.ProductFieldValues.RemoveRange(productFields);
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

        public ICollection<ProductFieldValue> GetProductFields(string id)
        {
            return context.ProductFieldValues
                .Include(pf => pf.CategoryField)
                    .ThenInclude(cf => cf.Field)
                .Include(pf => pf.CategoryField)
                    .ThenInclude(cf => cf.Category)
                .Where(x => x.ProductId == id).ToList();
        }

        public bool IsFieldUsedByCategory(string id)
        {
            return context.CategoryFields.Any(cf => cf.FieldId == id);
        }

        public bool RemoveField(string id)
        {
            var field = GetField(id);

            if (field is null)
            {
                return false;
            }

            if (guard.IsSystemFieldName(field.Name))
            {
                throw new ConfiguratorMetadataException(
                    $"Field '{field.Name}' cannot be deleted — it is {guard.LockReason}.");
            }

            if (IsFieldUsedByCategory(id))
            {
                throw new FieldInUseException("Field is used by at least one category and cannot be removed.");
            }

            context.Fields.Remove(field);
            return context.SaveChanges() != 0;
        }
    }
}
