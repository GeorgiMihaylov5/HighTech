using HighTech.Models;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Data
{
    public static class ProductQueryExtensions
    {
        public static IQueryable<Product> IncludeFieldsAndCategories(this IQueryable<Product> query)
        {
            return query
                .Include(p => p.ProductFields)
                    .ThenInclude(pf => pf.CategoryField)
                        .ThenInclude(cf => cf.Field)
                .Include(p => p.ProductFields)
                    .ThenInclude(pf => pf.CategoryField)
                        .ThenInclude(cf => cf.Category);
        }
    }
}
