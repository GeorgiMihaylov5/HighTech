using HighTech.DTOs;
using HighTech.Models;

namespace HighTech.Configurator.Rules
{
    public class PsuWattageRule : CompatibilityRuleBase
    {
        public override IEnumerable<IncompatibilityDTO> Check(IReadOnlyDictionary<string, Product> sel)
        {
            var psu = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.Psu);
            var cpu = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.Cpu);
            var gpu = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.Gpu);
            if (psu == null || (cpu == null && gpu == null)) yield break;

            var psuWattageStr = GetFieldValue(psu, PcConfiguratorConstants.Categories.Psu, PcConfiguratorConstants.Fields.WattageW);
            if (!TryParseInt(psuWattageStr, out var psuWattage)) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R5, PcConfiguratorConstants.Categories.Psu, PcConfiguratorConstants.Fields.WattageW, psu); yield break; }

            int required;

            if (gpu != null)
            {
                var gpuPsuStr = GetFieldValue(gpu, PcConfiguratorConstants.Categories.Gpu, PcConfiguratorConstants.Fields.RecommendedPsuW);
                if (!TryParseInt(gpuPsuStr, out var gpuPsu)) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R5, PcConfiguratorConstants.Categories.Gpu, PcConfiguratorConstants.Fields.RecommendedPsuW, gpu); yield break; }

                if (cpu != null)
                {
                    var cpuTdpStr = GetFieldValue(cpu, PcConfiguratorConstants.Categories.Cpu, PcConfiguratorConstants.Fields.TdpW);
                    if (!TryParseInt(cpuTdpStr, out var cpuTdp)) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R5, PcConfiguratorConstants.Categories.Cpu, PcConfiguratorConstants.Fields.TdpW, cpu); yield break; }
                    required = Math.Max(gpuPsu, cpuTdp * 2 + 100);
                }
                else
                {
                    required = gpuPsu;
                }
            }
            else
            {
                var cpuTdpStr = GetFieldValue(cpu, PcConfiguratorConstants.Categories.Cpu, PcConfiguratorConstants.Fields.TdpW);
                if (!TryParseInt(cpuTdpStr, out var cpuTdp)) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R5, PcConfiguratorConstants.Categories.Cpu, PcConfiguratorConstants.Fields.TdpW, cpu); yield break; }
                required = cpuTdp * 2 + 100;
            }

            if (psuWattage < required)
                yield return new IncompatibilityDTO
                {
                    Rule = PcConfiguratorConstants.RuleCodes.R5,
                    CategoryA = PcConfiguratorConstants.Categories.Psu,
                    CategoryB = gpu != null ? PcConfiguratorConstants.Categories.Gpu : PcConfiguratorConstants.Categories.Cpu,
                    Message = $"PSU wattage ({psuWattage}W) is insufficient — at least {required}W required."
                };
        }
    }
}
