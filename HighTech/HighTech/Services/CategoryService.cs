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

        public Category CreateCategoryField(string name, string fieldId)
        {
            var cateogry = new Category()
            {
                Name = name,
                FieldId = fieldId
            };

            context.Categories.Add(cateogry);
            context.SaveChanges();

            return cateogry;
        }

        public ICollection<Category> EditCategoryName(string id, string name)
        {
            var originalName = context.Categories.FirstOrDefault(c => c.Id == id)?.Name;

            if (originalName is null)
            {
                return null;
            }

            var categories = GetAllByName(originalName);

            if(categories is null)
            {
                return null;
            }

            foreach (var c in categories)
            {
                c.Name = name;
            }

            context.Categories.UpdateRange(categories);

            context.SaveChanges();

            return categories;
        }

        public Category Get(string name, string fieldId)
        {
            return context.Categories
                .Include(c => c.Field)
                .FirstOrDefault(cf => cf.Name == name && cf.FieldId == fieldId);
        }

        public ICollection<Category> GetAll()
        {
            return context.Categories.Include(c => c.Field).ToList();
        }

        public ICollection<Category> GetAllByName(string name)
        {
            return context.Categories.Include(c => c.Field).Where(x => x.Name == name).ToList();
        }

        public string GetCategoryByProduct(string id)
        {
            return context.ProductsCategories
                .Where(pf => pf.ProductId == id)
                .Select(c => c.Category.Name)
                .FirstOrDefault();
        }

        public bool RemoveCategory(string id)
        {
            var category = context.Categories.FirstOrDefault(c => c.Id == id);

            if (category is null)
            {
                return false;
            }

            context.Remove(category);
            return context.SaveChanges() != 0;
        }

        public bool RemoveCategoryByName(string name)
        {
            var categoryFields = context.Categories.Where(c => c.Name == name).ToList();

            if (categoryFields is null || categoryFields.Count == 0)
            {
                return false;
            }

            context.RemoveRange(categoryFields);
            return context.SaveChanges() != 0;
        }

        public bool RemoveCategoryField(string name, string fieldId)
        {
            var categoryField = Get(name, fieldId);

            if (categoryField is null)
            {
                return false;
            }

            context.Categories.Remove(categoryField);
            return context.SaveChanges() != 0;
        }
    }
}
