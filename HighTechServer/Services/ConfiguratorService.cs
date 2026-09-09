using HighTech.Abstraction;
using HighTech.Configurator;
using HighTech.Configurator.Rules;
using HighTech.Data;
using HighTech.DTOs;
using HighTech.Models;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Services
{
    public class ConfiguratorService : IConfiguratorService
    {
        private readonly ApplicationDbContext context;

        private static readonly IReadOnlyList<ICompatibilityRule> Rules = new ICompatibilityRule[]
        {
            new CpuMotherboardSocketRule(),
            new RamTypeRule(),
            new FormFactorRule(),
            new CoolerSocketRule(),
            new PsuWattageRule(),
            new GpuLengthRule(),
            new CoolerHeightRule(),
            new RamSpeedRule(),
            new StorageInterfaceRule(),
        };

        public ConfiguratorService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public ICollection<Category> GetPcCategories()
        {
            var systemNames = PcConfiguratorConstants.Categories.All
                .Where(category => category != PcConfiguratorConstants.Categories.Assembly)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return context.Categories
                .Include(c => c.CategoryFields)
                    .ThenInclude(cf => cf.Field)
                .Where(c => systemNames.Contains(c.Name))
                .ToList();
        }

        public ICollection<Product> GetPartsInCategory(string categoryName)
        {
            return LoadProductsInCategory(categoryName);
        }

        public ICollection<Product> GetCompatibleParts(string categoryName, IDictionary<string, string> categorySelection)
        {
            var systemCategoryName = GetSystemCategoryName(categoryName);
            var candidates = LoadProductsInCategory(systemCategoryName);

            if (categorySelection == null || categorySelection.Count == 0)
                return candidates;

            var categorySelectionExcludingCurrent = categorySelection
               .Where(pair => !string.Equals(GetSystemCategoryName(pair.Key), systemCategoryName, StringComparison.OrdinalIgnoreCase))
               .ToDictionary(pair => GetSystemCategoryName(pair.Key), pair => pair.Value, StringComparer.OrdinalIgnoreCase);

            var alreadySelectedProducts = LoadProductsInCategories(categorySelectionExcludingCurrent);
            var currentIssues = GetIssues(alreadySelectedProducts);

            return candidates.Where(candidate =>
            {
                var testProducts = new Dictionary<string, Product>(alreadySelectedProducts, StringComparer.OrdinalIgnoreCase)
                {
                    [systemCategoryName] = candidate
                };
                var candidateIssues = GetIssues(testProducts);
                return candidateIssues.All(issue => currentIssues.Contains(issue));
            }).ToList();
        }

        public ConfiguratorValidationResultDTO Validate(IDictionary<string, string> categorySelection)
        {
            var selectedProducts = categorySelection == null || categorySelection.Count == 0
                ? new Dictionary<string, Product>(StringComparer.OrdinalIgnoreCase)
                : LoadProductsInCategories(categorySelection);

            var issues = new List<IncompatibilityDTO>();
            issues.AddRange(GetMissingRequiredPartIssues(selectedProducts));
            issues.AddRange(RunRules(selectedProducts));

            var resolvedItems = PcConfiguratorConstants.Categories.All
                .Where(category => category != PcConfiguratorConstants.Categories.Assembly)
                .Where(selectedProducts.ContainsKey)
                .Select(category => ToOrderedProductDTO(selectedProducts[category], category))
                .ToList();

            var assemblyProduct = LoadDefaultAssemblyProduct();
            if (assemblyProduct == null)
            {
                issues.Add(new IncompatibilityDTO
                {
                    Rule = PcConfiguratorConstants.RuleCodes.AssemblyService,
                    CategoryA = PcConfiguratorConstants.Categories.Assembly,
                    CategoryB = null,
                    Message = "Default PC assembly service is missing."
                });
            }
            else
            {
                resolvedItems.Add(ToOrderedProductDTO(assemblyProduct, PcConfiguratorConstants.Categories.Assembly));
            }

            var cpu = selectedProducts.GetValueOrDefault(PcConfiguratorConstants.Categories.Cpu);

            return new ConfiguratorValidationResultDTO
            {
                IsValid = issues.Count == 0,
                Issues = issues,
                ResolvedItems = resolvedItems,
                HasIntegratedGraphics = cpu != null && CpuHasGraphicCore(cpu)
            };
        }

        private HashSet<string> GetIssues(IReadOnlyDictionary<string, Product> categoryProducts)
        {
            var currentIssues = RunRules(categoryProducts);
            return BuildRelevantIssueKeys(currentIssues);
        }

        private ICollection<Product> LoadProductsInCategory(string categoryName)
        {
            var systemCategoryName = GetSystemCategoryName(categoryName);

            return context.Products
                .Where(p => p.IsRemoved != true
                    && p.ProductFields.Any(pf => pf.CategoryField.Category.Name == systemCategoryName))
                .IncludeFieldsAndCategories()
                .ToList();
        }

        private Product LoadDefaultAssemblyProduct()
        {
            return context.Products
                .Where(p => p.IsRemoved != true
                    && p.Manufacturer == PcConfiguratorConstants.AssemblyProduct.Manufacturer
                    && p.Model == PcConfiguratorConstants.AssemblyProduct.Model)
                .OrderBy(p => p.Id)
                .FirstOrDefault();
        }

        private Dictionary<string, Product> LoadProductsInCategories(IDictionary<string, string> selection)
        {
            var productIds = selection.Values.Where(v => !string.IsNullOrWhiteSpace(v)).ToList();

            var products = context.Products
                .Where(p => p.IsRemoved != true && productIds.Contains(p.Id))
                .IncludeFieldsAndCategories()
                .ToDictionary(p => p.Id, StringComparer.OrdinalIgnoreCase);

            var result = new Dictionary<string, Product>(StringComparer.OrdinalIgnoreCase);

            foreach (var (categoryName, productId) in selection)
            {
                var systemCategoryName = GetSystemCategoryName(categoryName);
                if (!string.IsNullOrWhiteSpace(productId) && products.TryGetValue(productId, out var product))
                    result[systemCategoryName] = product;
            }

            return result;
        }

        private static string GetSystemCategoryName(string categoryName)
        {
            var trimmedCategoryName = categoryName?.Trim();
            if (string.IsNullOrWhiteSpace(trimmedCategoryName))
                return string.Empty;

            return PcConfiguratorConstants.Categories.All
                .FirstOrDefault(category => string.Equals(category, trimmedCategoryName, StringComparison.OrdinalIgnoreCase))
                ?? trimmedCategoryName;
        }

        private static IEnumerable<IncompatibilityDTO> RunRules(IReadOnlyDictionary<string, Product> sel)
        {
            return Rules.SelectMany(rule => rule.Check(sel));
        }

        private IEnumerable<IncompatibilityDTO> GetMissingRequiredPartIssues(Dictionary<string, Product> categorySelection)
        {
            var cpu = categorySelection.GetValueOrDefault(PcConfiguratorConstants.Categories.Cpu);
            var gpuIsOptional = cpu != null && CpuHasGraphicCore(cpu);
            var requiredCategories = PcConfiguratorConstants.Categories.All
                .Where(category => category != PcConfiguratorConstants.Categories.Assembly)
                .Where(category => category != PcConfiguratorConstants.Categories.Gpu || !gpuIsOptional);

            foreach (var categoryName in requiredCategories)
            {
                if (!categorySelection.ContainsKey(categoryName))
                    yield return new IncompatibilityDTO
                    {
                        Rule = PcConfiguratorConstants.RuleCodes.CompleteBuild,
                        CategoryA = categoryName,
                        CategoryB = null,
                        Message = $"Select a {categoryName} to complete the PC configuration."
                    };
            }
        }

        private static bool CpuHasGraphicCore(Product cpu)
        {
            var graphicCore = cpu.ProductFields?
                .FirstOrDefault(pf =>
                    string.Equals(pf.CategoryField?.Category?.Name, PcConfiguratorConstants.Categories.Cpu, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(pf.CategoryField?.Field?.Name, PcConfiguratorConstants.Fields.GraphicCore, StringComparison.OrdinalIgnoreCase))
                ?.Value;

            if (string.IsNullOrWhiteSpace(graphicCore)) return false;

            var negativeValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "None", "No", "N/A", "Without", "No integrated graphics"
            };
            return !negativeValues.Contains(graphicCore.Trim());
        }

        private static HashSet<string> BuildRelevantIssueKeys(IEnumerable<IncompatibilityDTO> issues)
        {
            return issues
                .Where(issue => issue.Rule != PcConfiguratorConstants.RuleCodes.CompleteBuild
                             && issue.Rule != PcConfiguratorConstants.RuleCodes.AssemblyService)
                .Select(issue => $"{issue.Rule}|{issue.CategoryA}|{issue.CategoryB}")
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        private static OrderedProductDTO ToOrderedProductDTO(Product product, string categoryName)
        {
            return new OrderedProductDTO
            {
                ProductId = product.Id,
                OrderedPrice = product.Price,
                Count = 1,
                Product = new ProductDTO
                {
                    Id = product.Id,
                    Manufacturer = product.Manufacturer,
                    Model = product.Model,
                    Price = product.Price,
                    Discount = product.Discount,
                    Quantity = product.Quantity,
                    Image = product.Image,
                    Warranty = product.Warranty,
                    CategoryName = categoryName,
                    Fields = product.ProductFields?.Select(pf => new FieldDTO
                    {
                        Id = pf.CategoryField?.Field?.Id,
                        Name = pf.CategoryField?.Field?.Name,
                        TypeCode = pf.CategoryField?.Field?.TypeCode ?? TypeCode.String,
                        Value = pf.Value
                    }).ToList() ?? new List<FieldDTO>()
                }
            };
        }
    }
}

