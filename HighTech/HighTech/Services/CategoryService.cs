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
        public bool CreateCategory(string categoryName)
        {
            context.Categories.Add(new Category()
            {
                Id = categoryName,
            });

            return context.SaveChanges() != 0;
        }

        public bool EditCategory(string id)
        {
            //TODO ON UPDATE CASCADE
            var category = GetCategory(id);

            if (category is null)
            {
                return false;
            }

            category.Id = id;

            return context.SaveChanges() != 0;
        }

        public ICollection<Category> GetCategories()
        {
            return context.Categories.ToList();
        }

        public Category GetCategory(string id)
        {
            return context.Categories.FirstOrDefault(c => c.Id == id);
        }

        public Category GetCategoryByProduct(string id)
        {
            return context.ProductsFields
                .Where(pf => pf.ProductId == id)
                .SelectMany(pf => pf.Field.CategoryFields)
                .Select(cf => cf.Category)
                .FirstOrDefault();
        }

        public bool RemoveCategory(string id)
        {
            var category = GetCategory(id);

            if (category is null)
            {
                return false;
            }

            context.Categories.Remove(category);
            return context.SaveChanges() != 0;
        }
    }
}
