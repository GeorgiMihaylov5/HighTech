using HighTech.Abstraction;
using HighTech.Data;
using HighTech.Models;

namespace HighTech.Services
{
    public class CategoryFieldService : ICategoryFieldService
    {
        private readonly ApplicationDbContext context;

        public CategoryFieldService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public bool CreateCategoryField(string categoryId, string fieldId)
        {
            context.CategoriesFields.Add(new CategoryField()
            {
                CategoryId = categoryId,
                FieldId = fieldId
            });

            return context.SaveChanges() != 0;
        }

        public ICollection<CategoryField> GetAll()
        {
            return context.CategoriesFields.ToList();
        }

        public bool RemoveCategoryField(string categoryId, string fieldId)
        {
            var categoryField = context.CategoriesFields
                .FirstOrDefault(cf => cf.CategoryId == categoryId && cf.FieldId == fieldId);

            if (categoryField is null)
            {
                return false;
            }

            context.CategoriesFields.Remove(categoryField);
            return context.SaveChanges() != 0;
        }
    }
}
