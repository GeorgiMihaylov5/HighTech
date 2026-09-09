using HighTech.DTOs;
using HighTech.Models;

namespace HighTech.Abstraction
{
    public interface IConfiguratorService
    {
        ICollection<Category> GetPcCategories();
        ICollection<Product> GetPartsInCategory(string categoryName);
        ICollection<Product> GetCompatibleParts(string categoryName, IDictionary<string, string> currentSelection);
        ConfiguratorValidationResultDTO Validate(IDictionary<string, string> selection);
    }
}
