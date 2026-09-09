using HighTech.DTOs;
using HighTech.Models;

namespace HighTech.Configurator.Rules
{
    public abstract class CompatibilityRuleBase : ICompatibilityRule
    {
        public abstract IEnumerable<IncompatibilityDTO> Check(IReadOnlyDictionary<string, Product> selection);

        protected static string GetFieldValue(Product product, string categoryName, string fieldName)
        {
            var value = product.ProductFields?
                .FirstOrDefault(pf =>
                    string.Equals(pf.CategoryField?.Category?.Name, categoryName, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(pf.CategoryField?.Field?.Name, fieldName, StringComparison.OrdinalIgnoreCase))
                ?.Value;

            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        protected static bool TryParseInt(string value, out int result)
        {
            if (string.IsNullOrWhiteSpace(value)) { result = 0; return false; }
            return int.TryParse(value.Trim(), out result);
        }

        protected static IncompatibilityDTO MissingField(string rule, string categoryName, string fieldName, Product product)
        {
            return new IncompatibilityDTO
            {
                Rule = rule,
                CategoryA = categoryName,
                CategoryB = null,
                Message = $"'{product.Manufacturer} {product.Model}' is missing field '{fieldName}' required for compatibility check."
            };
        }
    }
}
