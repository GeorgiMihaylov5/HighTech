using HighTech.Core.Entities;
using HighTech.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Infrastructure.Repositories
{
	public class FieldRepository : IFieldRepository
	{
		private readonly ApplicationDbContext context;

		public FieldRepository(ApplicationDbContext _context)
		{
			context = _context;
		}

		public Field Create(string? name, TypeCode typeCode)
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

		public Field? Edit(string? id, string? name, TypeCode typeCode)
		{
			var field = Get(id);

            if (field is null)
			{
				return null;
			}

			field.TypeCode = typeCode;
			field.Name = name;
			context.SaveChanges();

			return field;
		}

		public Field? Get(string? id)
		{
			return context.Fields
				.Where(f => !f.IsRemoved)
                .Include(f => f.CategoryFields!)
					.ThenInclude(cf => cf.Category)
				.FirstOrDefault(x => x.Id == id);
		}

		public ICollection<Field> GetAll()
		{
			return context.Fields
                .Where(f => !f.IsRemoved)
                .Include(f => f.CategoryFields!)
					.ThenInclude(cf => cf.Category)
				.ToList();
		}

		public bool SoftDelete(string? id)
		{
            var field = Get(id);

            if (field is null)
            {
                return false;
            }

            field.IsRemoved = true;
            context.Fields.Update(field);

            return context.SaveChanges() != 0;
		}

        public ICollection<Field> GetFieldsByCategory(string? categoryId)
		{
			//TODO think about !
			return context.CategoryFields
				.Include(cf => cf.Field)
				.Where(cf => cf.CategoryId == categoryId)
                .Where(f => !f.Field!.IsRemoved)
                .Select(cf => cf.Field!)
				.ToList();
        }

        public ProductFieldValue SetProductFieldValue(string? productId, string? fieldId, string? value)
		{
			var existingProductFieldValue = context.ProductFieldValues
				.FirstOrDefault(pf => pf.ProductID == productId && pf.FieldID == fieldId);

			if (existingProductFieldValue != null)
			{
				existingProductFieldValue.Value = value;
				context.SaveChanges();
				return existingProductFieldValue;
			}

			var productFieldValue = new ProductFieldValue()
			{
				ProductID = productId,
				FieldID = fieldId,
				Value = value
			};

			context.ProductFieldValues.Add(productFieldValue);
			context.SaveChanges();

			return productFieldValue;

        }

        public Field? GetFieldByName(string? name)
        {
            return context.Fields
				.Where(f => !f.IsRemoved)
                .Include(f => f.CategoryFields!)
					.ThenInclude(cf => cf.Category)
				.FirstOrDefault(f => f.Name == name);
        }
    }
}