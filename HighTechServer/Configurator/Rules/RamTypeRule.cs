using HighTech.DTOs;
using HighTech.Models;

namespace HighTech.Configurator.Rules
{
    public class RamTypeRule : CompatibilityRuleBase
    {
        public override IEnumerable<IncompatibilityDTO> Check(IReadOnlyDictionary<string, Product> sel)
        {
            var ram = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.Ram);
            var mobo = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.Motherboard);
            if (ram == null || mobo == null) yield break;

            var ramType = GetFieldValue(ram, PcConfiguratorConstants.Categories.Ram, PcConfiguratorConstants.Fields.RamType);
            var moboRamType = GetFieldValue(mobo, PcConfiguratorConstants.Categories.Motherboard, PcConfiguratorConstants.Fields.RamType);

            if (ramType == null) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R2, PcConfiguratorConstants.Categories.Ram, PcConfiguratorConstants.Fields.RamType, ram); yield break; }
            if (moboRamType == null) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R2, PcConfiguratorConstants.Categories.Motherboard, PcConfiguratorConstants.Fields.RamType, mobo); yield break; }

            if (!string.Equals(ramType, moboRamType, StringComparison.OrdinalIgnoreCase))
                yield return new IncompatibilityDTO
                {
                    Rule = PcConfiguratorConstants.RuleCodes.R2,
                    CategoryA = PcConfiguratorConstants.Categories.Ram,
                    CategoryB = PcConfiguratorConstants.Categories.Motherboard,
                    Message = $"RAM type '{ramType}' is not compatible with Motherboard RAM type '{moboRamType}'."
                };
        }
    }
}
