using HighTech.DTOs;
using HighTech.Models;

namespace HighTech.Configurator.Rules
{
    public class RamSpeedRule : CompatibilityRuleBase
    {
        public override IEnumerable<IncompatibilityDTO> Check(IReadOnlyDictionary<string, Product> sel)
        {
            var ram = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.Ram);
            var mobo = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.Motherboard);
            if (ram == null || mobo == null) yield break;

            var ramSpeedStr = GetFieldValue(ram, PcConfiguratorConstants.Categories.Ram, PcConfiguratorConstants.Fields.RamSpeedMhz);
            var moboMaxStr = GetFieldValue(mobo, PcConfiguratorConstants.Categories.Motherboard, PcConfiguratorConstants.Fields.MaxRamSpeedMhz);

            if (!TryParseInt(ramSpeedStr, out var ramSpeed)) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R8, PcConfiguratorConstants.Categories.Ram, PcConfiguratorConstants.Fields.RamSpeedMhz, ram); yield break; }
            if (!TryParseInt(moboMaxStr, out var moboMax)) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R8, PcConfiguratorConstants.Categories.Motherboard, PcConfiguratorConstants.Fields.MaxRamSpeedMhz, mobo); yield break; }

            if (ramSpeed > moboMax)
                yield return new IncompatibilityDTO
                {
                    Rule = PcConfiguratorConstants.RuleCodes.R8,
                    CategoryA = PcConfiguratorConstants.Categories.Ram,
                    CategoryB = PcConfiguratorConstants.Categories.Motherboard,
                    Message = $"RAM speed ({ramSpeed}MHz) exceeds Motherboard's max supported speed ({moboMax}MHz)."
                };
        }
    }
}
