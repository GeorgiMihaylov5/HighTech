using HighTech.Abstraction;
using HighTech.Configurator;
using HighTech.Data;
using HighTech.DTOs.Assistant;
using HighTech.Models;
using HighTech.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace HighTech.Services.Assistant
{
    public class AssistantDataResolver
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguratorService configurator;
        private readonly AssistantOptions options;

        private static readonly HashSet<string> CategoryWhitelist = new(
            PcConfiguratorConstants.Categories.All
                .Where(c => c != PcConfiguratorConstants.Categories.Assembly),
            StringComparer.OrdinalIgnoreCase);

        // Maps common aliases the LLM might emit back to canonical category names.
        // Keys are lowercased; the resolver compares case-insensitively.
        private static readonly Dictionary<string, string> CategoryAliases = new(StringComparer.OrdinalIgnoreCase)
        {
            ["cpu"] = PcConfiguratorConstants.Categories.Cpu,
            ["processor"] = PcConfiguratorConstants.Categories.Cpu,
            ["motherboard"] = PcConfiguratorConstants.Categories.Motherboard,
            ["mobo"] = PcConfiguratorConstants.Categories.Motherboard,
            ["ram"] = PcConfiguratorConstants.Categories.Ram,
            ["memory"] = PcConfiguratorConstants.Categories.Ram,
            ["gpu"] = PcConfiguratorConstants.Categories.Gpu,
            ["graphics card"] = PcConfiguratorConstants.Categories.Gpu,
            ["video card"] = PcConfiguratorConstants.Categories.Gpu,
            ["storage"] = PcConfiguratorConstants.Categories.Storage,
            ["ssd"] = PcConfiguratorConstants.Categories.Storage,
            ["hdd"] = PcConfiguratorConstants.Categories.Storage,
            ["drive"] = PcConfiguratorConstants.Categories.Storage,
            ["psu"] = PcConfiguratorConstants.Categories.Psu,
            ["power supply"] = PcConfiguratorConstants.Categories.Psu,
            ["power supply unit"] = PcConfiguratorConstants.Categories.Psu,
            ["case"] = PcConfiguratorConstants.Categories.Case,
            ["chassis"] = PcConfiguratorConstants.Categories.Case,
            ["tower"] = PcConfiguratorConstants.Categories.Case,
            ["cpu cooler"] = PcConfiguratorConstants.Categories.CpuCooler,
            ["cooler"] = PcConfiguratorConstants.Categories.CpuCooler,
            ["heatsink"] = PcConfiguratorConstants.Categories.CpuCooler,
        };

        public AssistantDataResolver(
            ApplicationDbContext context,
            IConfiguratorService configurator,
            IOptions<AssistantOptions> options)
        {
            this.context = context;
            this.configurator = configurator;
            this.options = options.Value;
        }

        public static string NormalizeCategory(string categoryName)
        {
            var trimmed = categoryName?.Trim();
            if (string.IsNullOrEmpty(trimmed)) return null;

            if (CategoryWhitelist.TryGetValue(trimmed, out var canonical))
                return canonical;

            return CategoryAliases.TryGetValue(trimmed, out var aliased) ? aliased : null;
        }

        public async Task<AssistantFetchedDataDTO> ResolveAsync(
            AssistantDataPlanDTO plan,
            AssistantContextDTO requestContext,
            IEnumerable<string> productReferenceTexts = null,
            CancellationToken ct = default)
        {
            plan ??= new AssistantDataPlanDTO();
            plan.Fetches ??= new List<AssistantDataPlanFetchDTO>();

            var normalizedFetches = NormalizeAndDedupeFetches(plan.Fetches)
                .Take(options.MaxFetchesPerRequest)
                .ToList();

            var products = new Dictionary<string, List<AssistantProductDTO>>(StringComparer.OrdinalIgnoreCase);
            var seenProductIds = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var fetch in normalizedFetches)
            {
                var fetched = await FetchProductsAsync(fetch, ct);

                if (!products.TryGetValue(fetch.CategoryName, out var list))
                {
                    list = new List<AssistantProductDTO>();
                    products[fetch.CategoryName] = list;
                    seenProductIds[fetch.CategoryName] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                }

                var ids = seenProductIds[fetch.CategoryName];
                foreach (var product in fetched)
                {
                    if (ids.Add(product.Id))
                        list.Add(product);
                }
            }

            Dictionary<string, List<AssistantProductDTO>> compatibleParts = null;
            if (plan.NeedsCompatibleParts && requestContext?.CurrentSelection != null && requestContext.CurrentSelection.Count > 0)
            {
                compatibleParts = ResolveCompatibleParts(requestContext.CurrentSelection, normalizedFetches);
            }

            var contextProducts = await FetchContextProductsAsync(
                productReferenceTexts,
                ct);

            Dictionary<string, AssistantProductDTO> currentSelectionProducts = null;
            if (plan.NeedsCurrentSelection && requestContext?.CurrentSelection != null && requestContext.CurrentSelection.Count > 0)
            {
                currentSelectionProducts = await ResolveCurrentSelectionProductsAsync(requestContext.CurrentSelection, ct);
            }

            return new AssistantFetchedDataDTO
            {
                Products = products,
                ContextProducts = contextProducts,
                CurrentSelectionProducts = currentSelectionProducts,
                CompatibleParts = compatibleParts
            };
        }

        private IEnumerable<AssistantDataPlanFetchDTO> NormalizeAndDedupeFetches(IEnumerable<AssistantDataPlanFetchDTO> raw)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var fetch in raw)
            {
                if (fetch == null) continue;

                var canonical = NormalizeCategory(fetch.CategoryName);
                if (canonical == null) continue;

                var priceMin = fetch.PriceMin.HasValue && fetch.PriceMin.Value >= 0 ? fetch.PriceMin : null;
                var priceMax = fetch.PriceMax.HasValue && fetch.PriceMax.Value >= 0 ? fetch.PriceMax : null;
                var manufacturer = string.IsNullOrWhiteSpace(fetch.Manufacturer) ? null : fetch.Manufacturer.Trim();

                var key = $"{canonical}|{manufacturer?.ToLowerInvariant()}|{priceMin}|{priceMax}";
                if (!seen.Add(key)) continue;

                yield return new AssistantDataPlanFetchDTO
                {
                    CategoryName = canonical,
                    PriceMin = priceMin,
                    PriceMax = priceMax,
                    Manufacturer = manufacturer
                };
            }
        }

        private async Task<List<AssistantProductDTO>> FetchProductsAsync(AssistantDataPlanFetchDTO fetch, CancellationToken ct)
        {
            var havePriceFilter = fetch.PriceMin.HasValue || fetch.PriceMax.HasValue;
            var haveManufacturerFilter = !string.IsNullOrWhiteSpace(fetch.Manufacturer);

            var attempts = new[]
            {
                (Price: havePriceFilter, Manufacturer: haveManufacturerFilter),
                (Price: false, Manufacturer: haveManufacturerFilter),
                (Price: false, Manufacturer: false)
            }.Distinct();

            foreach (var attempt in attempts)
            {
                var products = await RunFetchAsync(fetch, attempt.Price, attempt.Manufacturer, ct);
                if (products.Count > 0)
                    return products;
            }

            return new List<AssistantProductDTO>();
        }

        private async Task<List<AssistantProductDTO>> RunFetchAsync(
            AssistantDataPlanFetchDTO fetch,
            bool applyPriceFilters,
            bool applyManufacturerFilter,
            CancellationToken ct)
        {
            var query = context.Products
                .Where(p => p.IsRemoved != true)
                .Where(p => p.ProductFields.Any(pf => pf.CategoryField.Category.Name == fetch.CategoryName));

            if (applyPriceFilters && fetch.PriceMin.HasValue)
                query = query.Where(p => p.Price >= fetch.PriceMin.Value);

            if (applyPriceFilters && fetch.PriceMax.HasValue)
                query = query.Where(p => p.Price <= fetch.PriceMax.Value);

            if (applyManufacturerFilter && !string.IsNullOrWhiteSpace(fetch.Manufacturer))
            {
                var pattern = $"%{fetch.Manufacturer}%";
                query = query.Where(p => EF.Functions.Like(p.Manufacturer, pattern));
            }

            var take = options.MaxProductsPerFetch;
            List<Product> rows;

            if (applyPriceFilters && fetch.PriceMax.HasValue)
            {
                rows = await ShuffleProductsByPriceAsync(query, take, ct);
            }
            else
            {
                rows = await query
                    .OrderBy(p => p.Price)
                    .Take(take)
                    .IncludeFieldsAndCategories()
                    .AsNoTracking()
                    .ToListAsync(ct);
            }

            return rows.Select(p => ToAssistantProductDTO(p, fetch.CategoryName)).ToList();
        }

        private static async Task<List<Product>> ShuffleProductsByPriceAsync(
            IQueryable<Product> query,
            int take,
            CancellationToken ct)
        {
            var topCount = (take + 1) / 2;
            var bottomCount = take - topCount;

            var top = await query
                .OrderByDescending(p => p.Price)
                .Take(topCount)
                .IncludeFieldsAndCategories()
                .AsNoTracking()
                .ToListAsync(ct);

            var bottom = bottomCount > 0
                ? await query
                    .OrderBy(p => p.Price)
                    .Take(bottomCount)
                    .IncludeFieldsAndCategories()
                    .AsNoTracking()
                    .ToListAsync(ct)
                : new List<Product>();

            return top
                .Concat(bottom)
                .GroupBy(p => p.Id)
                .Select(g => g.First())
                .ToList();
        }

        private async Task<List<AssistantProductDTO>> FetchContextProductsAsync(
            IEnumerable<string> productReferenceTexts,
            CancellationToken ct)
        {
            return await FetchReferencedProductsAsync(productReferenceTexts, Array.Empty<string>(), ct);
        }

        private async Task<List<AssistantProductDTO>> FetchReferencedProductsAsync(
            IEnumerable<string> productReferenceTexts,
            ICollection<string> excludedIds,
            CancellationToken ct)
        {
            var corpus = NormalizeSearchText(string.Join(" ", productReferenceTexts ?? Enumerable.Empty<string>()));
            if (corpus.Length < 3)
                return new List<AssistantProductDTO>();

            var rows = await context.Products
                .Where(p => p.IsRemoved != true)
                .IncludeFieldsAndCategories()
                .AsNoTracking()
                .ToListAsync(ct);

            return rows
                .Where(p => !excludedIds.Contains(p.Id))
                .Select(p => new { Product = p, Score = ScoreProductReference(p, corpus) })
                .Where(x => x.Score >= 30)
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Product.Price)
                .Take(8)
                .Select(x => ToContextProductDTO(x.Product))
                .ToList();
        }

        private Dictionary<string, List<AssistantProductDTO>> ResolveCompatibleParts(
            IDictionary<string, string> currentSelection,
            IEnumerable<AssistantDataPlanFetchDTO> fetches)
        {
            var result = new Dictionary<string, List<AssistantProductDTO>>(StringComparer.OrdinalIgnoreCase);
            var distinctCategories = fetches
                .Select(f => f.CategoryName)
                .Where(name => CategoryWhitelist.Contains(name))
                .Distinct(StringComparer.OrdinalIgnoreCase);

            foreach (var categoryName in distinctCategories)
            {
                if (result.ContainsKey(categoryName)) continue;

                var compatible = configurator.GetCompatibleParts(categoryName, currentSelection);
                var capped = compatible.Take(options.MaxProductsPerFetch);
                result[categoryName] = capped.Select(p => ToAssistantProductDTO(p, categoryName)).ToList();
            }

            return result;
        }

        private async Task<Dictionary<string, AssistantProductDTO>> ResolveCurrentSelectionProductsAsync(
            IDictionary<string, string> currentSelection,
            CancellationToken ct)
        {
            var idToCategory = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var (rawCategory, productId) in currentSelection)
            {
                if (string.IsNullOrWhiteSpace(productId)) continue;
                var category = NormalizeCategory(rawCategory);
                if (category == null) continue;
                idToCategory[productId] = category;
            }

            if (idToCategory.Count == 0)
                return new Dictionary<string, AssistantProductDTO>(StringComparer.OrdinalIgnoreCase);

            var ids = idToCategory.Keys.ToList();
            var rows = await context.Products
                .Where(p => p.IsRemoved != true && ids.Contains(p.Id))
                .IncludeFieldsAndCategories()
                .AsNoTracking()
                .ToListAsync(ct);

            var result = new Dictionary<string, AssistantProductDTO>(StringComparer.OrdinalIgnoreCase);
            foreach (var product in rows)
            {
                if (!idToCategory.TryGetValue(product.Id, out var category)) continue;
                result[category] = ToAssistantProductDTO(product, category);
            }

            return result;
        }

        public async Task<Dictionary<string, AssistantProductSummaryDTO>> GetProductSummariesAsync(
            IEnumerable<string> productIds,
            CancellationToken ct = default)
        {
            var ids = productIds?
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .ToList();

            if (ids == null || ids.Count == 0)
                return new Dictionary<string, AssistantProductSummaryDTO>();

            var products = await context.Products
                .Where(p => p.IsRemoved != true && ids.Contains(p.Id))
                .Select(p => new AssistantProductSummaryDTO
                {
                    Id = p.Id,
                    Manufacturer = p.Manufacturer,
                    Model = p.Model,
                    Price = p.Price
                })
                .AsNoTracking()
                .ToListAsync(ct);

            return products.ToDictionary(p => p.Id);
        }

        private static AssistantProductDTO ToAssistantProductDTO(Product product, string categoryName)
        {
            var dto = new AssistantProductDTO
            {
                Id = product.Id,
                Manufacturer = product.Manufacturer,
                Model = product.Model,
                CategoryName = categoryName,
                Price = product.Price,
                Fields = new List<AssistantProductFieldDTO>()
            };

            if (product.ProductFields == null) return dto;

            foreach (var pf in product.ProductFields)
            {
                var fieldName = pf.CategoryField?.Field?.Name;
                var fieldCategory = pf.CategoryField?.Category?.Name;

                // Only emit fields that belong to the category we're listing the product under,
                // so an Intel CPU does not leak Motherboard fields if it appears in both.
                if (string.IsNullOrWhiteSpace(fieldName)) continue;
                if (!string.Equals(fieldCategory, categoryName, StringComparison.OrdinalIgnoreCase)) continue;

                dto.Fields.Add(new AssistantProductFieldDTO
                {
                    Name = fieldName,
                    Value = pf.Value
                });
            }

            return dto;
        }

        private static AssistantProductDTO ToContextProductDTO(Product product)
        {
            var categoryName = product.ProductFields?
                .Select(pf => pf.CategoryField?.Category?.Name)
                .FirstOrDefault(name => !string.IsNullOrWhiteSpace(name));

            var dto = new AssistantProductDTO
            {
                Id = product.Id,
                Manufacturer = product.Manufacturer,
                Model = product.Model,
                CategoryName = categoryName,
                Price = product.Price,
                Fields = new List<AssistantProductFieldDTO>()
            };

            if (product.ProductFields == null) return dto;

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var pf in product.ProductFields)
            {
                var fieldName = pf.CategoryField?.Field?.Name;
                if (string.IsNullOrWhiteSpace(fieldName)) continue;
                if (!seen.Add(fieldName)) continue;

                dto.Fields.Add(new AssistantProductFieldDTO
                {
                    Name = fieldName,
                    Value = pf.Value
                });
            }

            return dto;
        }

        private static int ScoreProductReference(Product product, string corpus)
        {
            var manufacturer = NormalizeSearchText(product.Manufacturer);
            var model = NormalizeSearchText(product.Model);
            var fullName = NormalizeSearchText($"{product.Manufacturer} {product.Model}");
            var score = 0;

            if (fullName.Length > 0 && corpus.Contains(fullName))
                score += 160;
            if (model.Length > 0 && corpus.Contains(model))
                score += 120;
            if (manufacturer.Length > 0 && corpus.Contains(manufacturer))
                score += 10;

            var tokens = model
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(token => token.Length >= 2)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            score += tokens.Count(token => corpus.Contains(token)) * 15;

            return score;
        }

        private static string NormalizeSearchText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var lower = value.ToLowerInvariant();
            return Regex.Replace(lower, @"[^a-z0-9]+", " ").Trim();
        }
    }
}
