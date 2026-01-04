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

		public Field? EditField(string id, string name, TypeCode typeCode)
		{
			var field = context.Fields.FirstOrDefault(x => x.Id == id);

			if (field is null)
			{
				return null;
			}

			field.TypeCode = typeCode;
			field.Name = name;
			context.SaveChanges();

			return field;
		}

		public Field? GetField(string id)
		{
			return context.Fields
				.Include(f => f.CategoryFields!)
					.ThenInclude(cf => cf.Category)
				.FirstOrDefault(x => x.Id == id);
		}

		public ICollection<Field> GetFields()
		{
			return context.Fields
				.Include(f => f.CategoryFields!)
					.ThenInclude(cf => cf.Category)
				.ToList();
		}

		public bool RemoveField(string id)
		{
			var field = context.Fields.FirstOrDefault(f => f.Id == id);

			if (field is null)
			{
				return false;
			}

			context.Fields.Remove(field);
			return context.SaveChanges() != 0;
		}

		// ProductFieldValue operations
		public ICollection<ProductFieldValue> GetProductFieldValues(string productId)
		{
			return context.ProductFieldValues
				.Include(pf => pf.Field)
				.Where(pf => pf.ProductID == productId)
				.ToList();
		}

		public ProductFieldValue? AddProductFieldValue(string productId, string fieldId, string value)
		{
			// Validate product exists
			var productExists = context.Products.Any(p => p.Id == productId);
			if (!productExists)
			{
				throw new InvalidOperationException($"Product with ID '{productId}' does not exist.");
			}

			// Validate field exists
			var fieldExists = context.Fields.Any(f => f.Id == fieldId);
			if (!fieldExists)
			{
				throw new InvalidOperationException($"Field with ID '{fieldId}' does not exist.");
			}

			// Check if already exists
			var existing = context.ProductFieldValues
				.FirstOrDefault(pf => pf.ProductID == productId && pf.FieldID == fieldId);

			if (existing != null)
			{
				existing.Value = value;
				context.SaveChanges();
				return existing;
			}

			var productFieldValue = new ProductFieldValue()
			{
				ProductID = productId,
				FieldID = fieldId,
				Value = value
			};

			context.ProductFieldValues.Add(productFieldValue);
			context.SaveChanges();

			return context.ProductFieldValues
				.Include(pf => pf.Field)
				.FirstOrDefault(pf => pf.ProductID == productId && pf.FieldID == fieldId);
		}

		public bool EditProductFieldValue(string productId, string fieldId, string value)
		{
			var productFieldValue = context.ProductFieldValues
				.FirstOrDefault(pf => pf.ProductID == productId && pf.FieldID == fieldId);

			if (productFieldValue is null)
			{
				return false;
			}

			productFieldValue.Value = value;
			return context.SaveChanges() != 0;
		}

		public bool RemoveProductFieldValue(string productId, string fieldId)
		{
			var productFieldValue = context.ProductFieldValues
				.FirstOrDefault(pf => pf.ProductID == productId && pf.FieldID == fieldId);
			if (productFieldValue is null)
			{
				return false;
			}

			context.ProductFieldValues.Remove(productFieldValue);
			return context.SaveChanges() != 0;
		}
	}
}