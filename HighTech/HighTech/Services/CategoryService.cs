﻿using HighTech.Abstraction;
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

        public Category CreateCategoryField(string categoryId, string fieldId)
        {
            var cateogry = new Category()
            {
                Id = categoryId,
                FieldId = fieldId
            };

            context.Categories.Add(cateogry);
            context.SaveChanges();

            return cateogry;
        }

        public Category Get(string categoryId, string fieldId)
        {
            return context.Categories
                .FirstOrDefault(cf => cf.Id == categoryId && cf.FieldId == fieldId);
        }

        public ICollection<Category> GetAll()
        {
            return context.Categories.Include(c => c.Field).ToList();
        }

        public string GetCategoryByProduct(string id)
        {
            return context.ProductsCategories
                .Where(pf => pf.ProductId == id)
                .Select(x => x.CategoryId)
                .FirstOrDefault();
        }

        public bool RemoveCategories(string categoryId)
        {
            var categoryFields = context.Categories.Where(c => c.Id == categoryId).ToList();

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
