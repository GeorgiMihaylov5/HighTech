using HighTech.Abstraction;
using HighTech.Data;
using HighTech.DTOs;
using HighTech.Exceptions;
using HighTech.Models;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Services
{
    public class CategoryService : ICategoryService
    {
        private const int SampleProductLimit = 5;

        private readonly ApplicationDbContext context;
        private readonly IConfiguratorMetadataGuard guard;

        public CategoryService(ApplicationDbContext _context, IConfiguratorMetadataGuard _guard)
        {
            context = _context;
            guard = _guard;
        }

        public Category Create(string name, ICollection<string> fieldIds)
        {
            if (string.IsNullOrWhiteSpace(name) || fieldIds is null || fieldIds.Count == 0)
            {
                return null;
            }

            var distinctFieldIds = fieldIds
                .Where(fieldId => !string.IsNullOrWhiteSpace(fieldId))
                .Distinct()
                .ToList();

            if (distinctFieldIds.Count == 0)
            {
                return null;
            }

            var category = new Category()
            {
                Name = name,
                CategoryFields = distinctFieldIds.Select(fieldId => new CategoryField()
                {
                    FieldId = fieldId
                }).ToList()
            };

            context.Categories.Add(category);
            context.SaveChanges();

            return Get(category.Id);
        }

        public Category Edit(string id, string name, ICollection<string> fieldIds)
        {
            var category = context.Categories
                .Include(c => c.CategoryFields)
                    .ThenInclude(cf => cf.Field)
                .FirstOrDefault(c => c.Id == id);

            if (category is null || string.IsNullOrWhiteSpace(name) || fieldIds is null || fieldIds.Count == 0)
            {
                return null;
            }

            var distinctFieldIds = fieldIds
                .Where(fieldId => !string.IsNullOrWhiteSpace(fieldId))
                .Distinct()
                .ToList();

            if (distinctFieldIds.Count == 0)
            {
                return null;
            }

            if (guard.IsSystemCategoryName(category.Name))
            {
                if (!string.Equals(name, category.Name, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ConfiguratorMetadataException(
                        $"Category '{category.Name}' cannot be renamed — it is {guard.LockReason}.");
                }

                var removedRequiredFields = category.CategoryFields
                    .Where(cf => guard.IsRequiredPairing(category.Name, cf.Field.Name)
                        && !distinctFieldIds.Contains(cf.FieldId))
                    .Select(cf => cf.Field.Name)
                    .ToList();

                if (removedRequiredFields.Count > 0)
                {
                    throw new ConfiguratorMetadataException(
                        $"Cannot remove required fields from '{category.Name}': {string.Join(", ", removedRequiredFields)} — {guard.LockReason}.");
                }
            }

            category.Name = name;

            var currentFieldIds = category.CategoryFields.Select(cf => cf.FieldId).ToHashSet();
            var updatedFieldIds = distinctFieldIds.ToHashSet();

            var removedFields = category.CategoryFields
                .Where(cf => !updatedFieldIds.Contains(cf.FieldId))
                .ToList();
            var removedCategoryFieldIds = removedFields
                .Select(cf => cf.Id)
                .ToList();
            var impactedProductIds = removedCategoryFieldIds.Count == 0
                ? new List<string>()
                : context.ProductFieldValues
                    .Where(pf => removedCategoryFieldIds.Contains(pf.CategoryFieldId))
                    .Select(pf => pf.ProductId)
                    .Distinct()
                    .ToList();

            if (removedFields.Count > 0)
            {
                context.CategoryFields.RemoveRange(removedFields);
            }

            var addedFieldIds = distinctFieldIds
                .Where(fieldId => !currentFieldIds.Contains(fieldId))
                .ToList();

            foreach (var fieldId in addedFieldIds)
            {
                category.CategoryFields.Add(new CategoryField()
                {
                    CategoryId = category.Id,
                    FieldId = fieldId
                });
            }

            using var transaction = context.Database.BeginTransaction();

            try
            {
                context.Categories.Update(category);
                context.SaveChanges();

                PreserveCategoryForImpactedProducts(category.Id, impactedProductIds);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            return Get(category.Id);
        }

        public Category Get(string id)
        {
            return context.Categories
                .Include(c => c.CategoryFields)
                    .ThenInclude(cf => cf.Field)
                .FirstOrDefault(c => c.Id == id);
        }

        public ICollection<Category> GetAll()
        {
            return context.Categories
                .Include(c => c.CategoryFields)
                    .ThenInclude(cf => cf.Field)
                .AsEnumerable()
                .Where(c => !guard.IsConfiguratorOnlyCategory(c.Name))
                .ToList();
        }

        public Category GetByName(string name)
        {
            return context.Categories
                .Include(c => c.CategoryFields)
                    .ThenInclude(cf => cf.Field)
                .FirstOrDefault(c => c.Name == name);
        }

        public bool ExistsByName(string name, string excludedId = null)
        {
            return context.Categories.Any(c => c.Name == name && c.Id != excludedId);
        }

        public CategoryField GetCategoryField(string categoryName, string fieldId)
        {
            return context.CategoryFields
                .Include(cf => cf.Category)
                .Include(cf => cf.Field)
                .FirstOrDefault(cf => cf.Category.Name == categoryName && cf.FieldId == fieldId);
        }

        public string GetCategoryByProduct(string id)
        {
            return context.ProductFieldValues
                .Where(pf => pf.ProductId == id)
                .Select(pf => pf.CategoryField.Category.Name)
                .FirstOrDefault();
        }

        public bool IsCategoryUsedByProduct(string id)
        {
            return context.ProductFieldValues.Any(pf => pf.CategoryField.CategoryId == id);
        }

        public CategoryRemovalPreviewDTO PreviewFieldRemoval(string id, ICollection<string> removedFieldIds)
        {
            var category = context.Categories.FirstOrDefault(c => c.Id == id);

            if (category is null)
            {
                return null;
            }

            var removedIds = (removedFieldIds ?? new List<string>())
                .Where(fieldId => !string.IsNullOrWhiteSpace(fieldId))
                .ToHashSet();

            var removedCategoryFields = context.CategoryFields
                .Include(cf => cf.Field)
                .Where(cf => cf.CategoryId == id && removedIds.Contains(cf.FieldId))
                .ToList();

            return BuildPreview(removedCategoryFields);
        }

        public CategoryRemovalPreviewDTO PreviewCategoryRemoval(string id)
        {
            var category = context.Categories.FirstOrDefault(c => c.Id == id);

            if (category is null)
            {
                return null;
            }

            var categoryFields = context.CategoryFields
                .Include(cf => cf.Field)
                .Where(cf => cf.CategoryId == id)
                .ToList();

            return BuildPreview(categoryFields);
        }

        public bool Remove(string id)
        {
            var category = context.Categories.FirstOrDefault(c => c.Id == id);

            if (category is null)
            {
                return false;
            }

            if (guard.IsSystemCategoryName(category.Name))
            {
                throw new ConfiguratorMetadataException(
                    $"Category '{category.Name}' cannot be deleted — it is {guard.LockReason}.");
            }

            context.Categories.Remove(category);
            return context.SaveChanges() != 0;
        }

        private CategoryRemovalPreviewDTO BuildPreview(ICollection<CategoryField> removedCategoryFields)
        {
            if (removedCategoryFields.Count == 0)
            {
                return new CategoryRemovalPreviewDTO()
                {
                    TotalAffectedProducts = 0,
                    AffectedFields = new List<AffectedFieldDTO>(),
                    SampleProducts = new List<string>()
                };
            }

            var removedCategoryFieldIds = removedCategoryFields.Select(cf => cf.Id).ToHashSet();

            var impactedRows = context.ProductFieldValues
                .Where(pf => removedCategoryFieldIds.Contains(pf.CategoryFieldId))
                .Where(pf => pf.Value != null && pf.Value.Trim() != "")
                .Select(pf => new
                {
                    pf.CategoryFieldId,
                    pf.ProductId,
                    ProductLabel = pf.Product.Manufacturer + " " + pf.Product.Model
                })
                .ToList();

            var affectedFields = removedCategoryFields
                .Select(cf => new AffectedFieldDTO()
                {
                    FieldId = cf.FieldId,
                    FieldName = cf.Field?.Name,
                    ProductCount = impactedRows
                        .Where(row => row.CategoryFieldId == cf.Id)
                        .Select(row => row.ProductId)
                        .Distinct()
                        .Count(),
                    SampleProducts = impactedRows
                        .Where(row => row.CategoryFieldId == cf.Id)
                        .Select(row => row.ProductLabel)
                        .Distinct()
                        .Take(SampleProductLimit)
                        .ToList()
                })
                .Where(field => field.ProductCount > 0)
                .ToList();

            var sampleProducts = impactedRows
                .Select(row => row.ProductLabel)
                .Distinct()
                .Take(SampleProductLimit)
                .ToList();

            return new CategoryRemovalPreviewDTO()
            {
                TotalAffectedProducts = impactedRows.Select(row => row.ProductId).Distinct().Count(),
                AffectedFields = affectedFields,
                SampleProducts = sampleProducts
            };
        }

        private void PreserveCategoryForImpactedProducts(string categoryId, ICollection<string> impactedProductIds)
        {
            if (impactedProductIds.Count == 0)
            {
                return;
            }

            var anchorCategoryFieldId = context.CategoryFields
                .Where(cf => cf.CategoryId == categoryId)
                .OrderBy(cf => cf.Field.Name)
                .Select(cf => cf.Id)
                .FirstOrDefault();

            if (anchorCategoryFieldId is null)
            {
                return;
            }

            var productsStillLinkedToCategory = context.ProductFieldValues
                .Where(pf => impactedProductIds.Contains(pf.ProductId))
                .Where(pf => pf.CategoryField.CategoryId == categoryId)
                .Select(pf => pf.ProductId)
                .Distinct()
                .ToHashSet();

            var placeholderRows = impactedProductIds
                .Where(productId => !productsStillLinkedToCategory.Contains(productId))
                .Select(productId => new ProductFieldValue()
                {
                    ProductId = productId,
                    CategoryFieldId = anchorCategoryFieldId,
                    Value = null
                })
                .ToList();

            if (placeholderRows.Count == 0)
            {
                return;
            }

            context.ProductFieldValues.AddRange(placeholderRows);
            context.SaveChanges();
        }
    }
}
