using HighTech.Abstraction;
using HighTech.Data;
using HighTech.Models;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext context;

        public CategoryService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public bool CreateCategoryField(string categoryId, string fieldId)
        {
            context.Categories.Add(new Category()
            {
                CategoryId = categoryId,
                FieldId = fieldId
            });

            return context.SaveChanges() != 0;
        }

        public Category Get(string categoryId, string fieldId)
        {
            return context.Categories
                .FirstOrDefault(cf => cf.CategoryId == categoryId && cf.FieldId == fieldId);
        }

        public ICollection<Category> GetAll()
        {
            return context.Categories.Include(c => c.Field).ToList();
        }

        public Category GetCategoryByProduct(string id)
        {
            return context.ProductsFields
                .Where(pf => pf.ProductId == id)
                .SelectMany(pf => pf.Field.Categories)
                .FirstOrDefault();
        }

        public bool RemoveCategories(string categoryId)
        {
            var categoryFields = context.Categories.Where(c => c.CategoryId == categoryId).ToList();

            if (categoryFields is null || categoryFields.Count == 0)
            {
                return false;
            }

            context.RemoveRange(categoryFields);
            return context.SaveChanges() != 0;
        }

        public bool RemoveCategoryField(string categoryId, string fieldId)
        {
            var categoryField = Get(categoryId, fieldId);

            if (categoryField is null)
            {
                return false;
            }

            context.Categories.Remove(categoryField);
            return context.SaveChanges() != 0;
        }
    }
}
