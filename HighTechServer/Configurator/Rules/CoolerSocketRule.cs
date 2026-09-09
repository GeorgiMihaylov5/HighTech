using HighTech.DTOs;
using HighTech.Models;

namespace HighTech.Configurator.Rules
{
    public class CoolerSocketRule : CompatibilityRuleBase
    {
        public override IEnumerable<IncompatibilityDTO> Check(IReadOnlyDictionary<string, Product> sel)
        {
            var cooler = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.CpuCooler);
            var cpu = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.Cpu);
            var mobo = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.Motherboard);
            if (cooler == null || (cpu == null && mobo == null)) yield break;

            var coolerSocketsRaw = GetFieldValue(cooler, PcConfiguratorConstants.Categories.CpuCooler, PcConfiguratorConstants.Fields.SocketType);

            if (coolerSocketsRaw == null) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R4, PcConfiguratorConstants.Categories.CpuCooler, PcConfiguratorConstants.Fields.SocketType, cooler); yield break; }

            var coolerSockets = new HashSet<string>(
                coolerSocketsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                StringComparer.OrdinalIgnoreCase);

            if (cpu != null)
            {
                var cpuSocket = GetFieldValue(cpu, PcConfiguratorConstants.Categories.Cpu, PcConfiguratorConstants.Fields.SocketType);
                if (cpuSocket == null) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R4, PcConfiguratorConstants.Categories.Cpu, PcConfiguratorConstants.Fields.SocketType, cpu); yield break; }

                if (!coolerSockets.Contains(cpuSocket))
                    yield return new IncompatibilityDTO
                    {
                        Rule = PcConfiguratorConstants.RuleCodes.R4,
                        CategoryA = PcConfiguratorConstants.Categories.CpuCooler,
                        CategoryB = PcConfiguratorConstants.Categories.Cpu,
                        Message = $"CPU Cooler does not support CPU socket '{cpuSocket}' (cooler supports: {coolerSocketsRaw})."
                    };
            }

            if (mobo != null)
            {
                var moboSocket = GetFieldValue(mobo, PcConfiguratorConstants.Categories.Motherboard, PcConfiguratorConstants.Fields.SocketType);
                if (moboSocket == null) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R4, PcConfiguratorConstants.Categories.Motherboard, PcConfiguratorConstants.Fields.SocketType, mobo); yield break; }

                if (!coolerSockets.Contains(moboSocket))
                    yield return new IncompatibilityDTO
                    {
                        Rule = PcConfiguratorConstants.RuleCodes.R4,
                        CategoryA = PcConfiguratorConstants.Categories.CpuCooler,
                        CategoryB = PcConfiguratorConstants.Categories.Motherboard,
                        Message = $"CPU Cooler does not support Motherboard socket '{moboSocket}' (cooler supports: {coolerSocketsRaw})."
                    };
            }
        }
    }
}
