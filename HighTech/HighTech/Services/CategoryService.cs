using HighTech.Abstraction;
using HighTech.Data;
using HighTech.Models;

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
            context.CategoriesFields.Add(new Category()
            {
                CategoryId = categoryId,
                FieldId = fieldId
            });

            return context.SaveChanges() != 0;
        }

        public Category Get(string categoryId, string fieldId)
        {
            return context.CategoriesFields
                .FirstOrDefault(cf => cf.CategoryId == categoryId && cf.FieldId == fieldId);
        }

        public ICollection<Category> GetAll()
        {
            return context.CategoriesFields.ToList();
        }

        public Category GetCategoryByProduct(string id)
        {
            return context.ProductsFields
                .Where(pf => pf.ProductId == id)
                .SelectMany(pf => pf.Field.Categories)
                .FirstOrDefault();
        }

        public bool RemoveCategoryField(string categoryId, string fieldId)
        {
            var categoryField = Get(categoryId, fieldId);

            if (categoryField is null)
            {
                return false;
            }

            context.CategoriesFields.Remove(categoryField);
            return context.SaveChanges() != 0;
        }
    }
}
