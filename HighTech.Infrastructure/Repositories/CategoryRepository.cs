using HighTech.Core.Entities;
using HighTech.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Infrastructure.Repositories
{
	public class CategoryRepository : ICategoryRepository
	{
		private readonly ApplicationDbContext context;

		public CategoryRepository(ApplicationDbContext _context)
		{
			context = _context;
		}

		public Category Create(string? name)
		{
			var category = new Category()
			{
				Name = name
			};

			context.Categories.Add(category);
			context.SaveChanges();

			return category;
		}

		public Category? Edit(string? id, string? name)
		{
			var category = context.Categories.FirstOrDefault(c => c.Id == id);

			if (category is null)
			{
				return null;
			}

			category.Name = name;
			context.Categories.Update(category);
			context.SaveChanges();

			return category;
		}

		public Category? Get(string? id)
		{
			return context.Categories
				.Where(c => !c.IsRemoved)
                .Include(c => c.CategoryFields!)
					.ThenInclude(cf => cf.Field)
				.FirstOrDefault(c => c.Id == id);
		}

		public Category? GetByName(string? name)
		{
			return context.Categories
                .Where(c => !c.IsRemoved)
                .Include(c => c.CategoryFields!)
					.ThenInclude(cf => cf.Field)
				.FirstOrDefault(c => c.Name == name);
		}

		public ICollection<Category> GetAll()
		{
			return context.Categories
                .Where(c => !c.IsRemoved)
                .Include(c => c.CategoryFields!)
					.ThenInclude(cf => cf.Field)
				.ToList();
		}

		public bool SoftDelete(string? id)
		{
            var category = Get(id);

            if (category is null)
            {
                return false;
            }

            category.IsRemoved = true;
            context.Categories.Update(category);

			return context.SaveChanges() != 0;
		}

		public ICollection<CategoryField> GetCategoryFields(string? categoryId)
		{
			return context.CategoryFields
				.Include(cf => cf.Field)
				.Where(cf => cf.CategoryId == categoryId)
				.Where(cf => !cf.Field!.IsRemoved)
				.ToList();
		}

		public CategoryField? AddFieldToCategory(string? categoryId, string? fieldId)
		{
			var exists = context.CategoryFields.Any(cf => cf.CategoryId == categoryId && cf.FieldId == fieldId);
			if (exists)
			{
				return context.CategoryFields
					.Include(cf => cf.Field)
					.FirstOrDefault(cf => cf.CategoryId == categoryId && cf.FieldId == fieldId);
			}

			var categoryField = new CategoryField()
			{
				CategoryId = categoryId,
				FieldId = fieldId
			};

			context.CategoryFields.Add(categoryField);
			context.SaveChanges();

			return context.CategoryFields
				.Include(cf => cf.Field)
				.FirstOrDefault(cf => cf.CategoryId == categoryId && cf.FieldId == fieldId);
		}

		public bool RemoveFieldFromCategory(string? categoryId, string? fieldId)
		{
			var categoryField = context.CategoryFields
				.FirstOrDefault(cf => cf.CategoryId == categoryId && cf.FieldId == fieldId);

			if (categoryField is null)
			{
				return false;
			}

			context.CategoryFields.Remove(categoryField);
			return context.SaveChanges() != 0;
		}
	}
}
