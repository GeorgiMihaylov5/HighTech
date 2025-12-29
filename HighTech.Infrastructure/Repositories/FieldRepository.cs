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

		public ProductCategory AddProductField(string productId, string categoryId, string value)
		{
			// Validate foreign keys exist
			var productExists = context.Products.Any(p => p.Id == productId);
			if (!productExists)
			{
				throw new InvalidOperationException($"Product with ID '{productId}' does not exist.");
			}

			var categoryExists = context.Categories.Any(c => c.Id == categoryId);
			if (!categoryExists)
			{
				throw new InvalidOperationException($"Category with ID '{categoryId}' does not exist.");
			}

			var field = new ProductCategory()
			{
				ProductId = productId,
				CategoryId = categoryId,
				Value = value,
			};

			context.ProductsCategories.Add(field);
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

		public Field? EditField(string id, string name, TypeCode typeCode)
		{
			var field = context.Fields.FirstOrDefault(x => x.Id == id);

			if (field is null)
			{
				return field;
			}

			field.TypeCode = typeCode;
			field.Name = name;
			context.SaveChanges();

			return field;
		}

		public bool EditProductFieldValue(string pfId, string categoryId, string value)
		{
			var productField = context.ProductsCategories.FirstOrDefault(x => x.Id == pfId);

			if (productField is null)
			{
				return false;
			}

			// Validate category exists
			var categoryExists = context.Categories.Any(c => c.Id == categoryId);
			if (!categoryExists)
			{
				throw new InvalidOperationException($"Category with ID '{categoryId}' does not exist.");
			}

			productField.CategoryId = categoryId;
			productField.Value = value;

			return context.SaveChanges() != 0;
		}

		public Field? GetField(string id)
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
				.Include(pf => pf.Category!.Field)
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
