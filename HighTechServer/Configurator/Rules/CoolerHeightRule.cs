using HighTech.DTOs;
using HighTech.Models;

namespace HighTech.Configurator.Rules
{
    public class CoolerHeightRule : CompatibilityRuleBase
    {
        public override IEnumerable<IncompatibilityDTO> Check(IReadOnlyDictionary<string, Product> sel)
        {
            var cooler = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.CpuCooler);
            var pcCase = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.Case);
            if (cooler == null || pcCase == null) yield break;

            var coolerHStr = GetFieldValue(cooler, PcConfiguratorConstants.Categories.CpuCooler, PcConfiguratorConstants.Fields.HeightMm);
            var caseMaxStr = GetFieldValue(pcCase, PcConfiguratorConstants.Categories.Case, PcConfiguratorConstants.Fields.MaxCpuCoolerHeightMm);

            if (!TryParseInt(coolerHStr, out var coolerH)) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R7, PcConfiguratorConstants.Categories.CpuCooler, PcConfiguratorConstants.Fields.HeightMm, cooler); yield break; }
            if (!TryParseInt(caseMaxStr, out var caseMax)) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R7, PcConfiguratorConstants.Categories.Case, PcConfiguratorConstants.Fields.MaxCpuCoolerHeightMm, pcCase); yield break; }

            if (coolerH > caseMax)
                yield return new IncompatibilityDTO
                {
                    Rule = PcConfiguratorConstants.RuleCodes.R7,
                    CategoryA = PcConfiguratorConstants.Categories.CpuCooler,
                    CategoryB = PcConfiguratorConstants.Categories.Case,
                    Message = $"CPU Cooler height ({coolerH}mm) exceeds Case max cooler clearance ({caseMax}mm)."
                };
        }
    }
}
