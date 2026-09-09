using HighTech.DTOs;
using HighTech.Models;

namespace HighTech.Configurator.Rules
{
    public class FormFactorRule : CompatibilityRuleBase
    {
        public override IEnumerable<IncompatibilityDTO> Check(IReadOnlyDictionary<string, Product> sel)
        {
            var mobo = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.Motherboard);
            var pcCase = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.Case);
            if (mobo == null || pcCase == null) yield break;

            var moboFormFactor = GetFieldValue(mobo, PcConfiguratorConstants.Categories.Motherboard, PcConfiguratorConstants.Fields.MotherboardFormFactor);
            var caseSupportedRaw = GetFieldValue(pcCase, PcConfiguratorConstants.Categories.Case, PcConfiguratorConstants.Fields.SupportedMotherboardFormFactors);

            if (moboFormFactor == null) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R3, PcConfiguratorConstants.Categories.Motherboard, PcConfiguratorConstants.Fields.MotherboardFormFactor, mobo); yield break; }
            if (caseSupportedRaw == null) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R3, PcConfiguratorConstants.Categories.Case, PcConfiguratorConstants.Fields.SupportedMotherboardFormFactors, pcCase); yield break; }

            var supported = ResolveSupportedFormFactors(caseSupportedRaw);
            if (!supported.Contains(moboFormFactor.Trim()))
                yield return new IncompatibilityDTO
                {
                    Rule = PcConfiguratorConstants.RuleCodes.R3,
                    CategoryA = PcConfiguratorConstants.Categories.Motherboard,
                    CategoryB = PcConfiguratorConstants.Categories.Case,
                    Message = $"Motherboard form factor '{moboFormFactor}' is not supported by the Case (supports: {caseSupportedRaw})."
                };
        }

        private static HashSet<string> ResolveSupportedFormFactors(string raw)
        {
            var parts = raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 1 && PcConfiguratorConstants.FormFactorCompatibility.TryGetValue(parts[0], out var expanded))
                return new HashSet<string>(expanded, StringComparer.OrdinalIgnoreCase);
            return new HashSet<string>(parts, StringComparer.OrdinalIgnoreCase);
        }
    }
}
