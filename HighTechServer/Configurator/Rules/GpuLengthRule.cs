using HighTech.DTOs;
using HighTech.Models;

namespace HighTech.Configurator.Rules
{
    public class GpuLengthRule : CompatibilityRuleBase
    {
        public override IEnumerable<IncompatibilityDTO> Check(IReadOnlyDictionary<string, Product> sel)
        {
            var gpu = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.Gpu);
            var pcCase = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.Case);
            if (gpu == null || pcCase == null) yield break;

            var gpuLenStr = GetFieldValue(gpu, PcConfiguratorConstants.Categories.Gpu, PcConfiguratorConstants.Fields.LengthMm);
            var caseMaxStr = GetFieldValue(pcCase, PcConfiguratorConstants.Categories.Case, PcConfiguratorConstants.Fields.MaxGpuLengthMm);

            if (!TryParseInt(gpuLenStr, out var gpuLen)) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R6, PcConfiguratorConstants.Categories.Gpu, PcConfiguratorConstants.Fields.LengthMm, gpu); yield break; }
            if (!TryParseInt(caseMaxStr, out var caseMax)) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R6, PcConfiguratorConstants.Categories.Case, PcConfiguratorConstants.Fields.MaxGpuLengthMm, pcCase); yield break; }

            if (gpuLen > caseMax)
                yield return new IncompatibilityDTO
                {
                    Rule = PcConfiguratorConstants.RuleCodes.R6,
                    CategoryA = PcConfiguratorConstants.Categories.Gpu,
                    CategoryB = PcConfiguratorConstants.Categories.Case,
                    Message = $"GPU length ({gpuLen}mm) exceeds Case max GPU clearance ({caseMax}mm)."
                };
        }
    }
}
