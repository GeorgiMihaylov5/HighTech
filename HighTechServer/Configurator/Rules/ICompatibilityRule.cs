using HighTech.DTOs;
using HighTech.Models;

namespace HighTech.Configurator.Rules
{
    public interface ICompatibilityRule
    {
        IEnumerable<IncompatibilityDTO> Check(IReadOnlyDictionary<string, Product> selection);
    }
}
