using HighTech.DTOs;
using HighTech.Models;

namespace HighTech.Configurator.Rules
{
    public class CpuMotherboardSocketRule : CompatibilityRuleBase
    {
        public override IEnumerable<IncompatibilityDTO> Check(IReadOnlyDictionary<string, Product> sel)
        {
            var cpu = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.Cpu);
            var mobo = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.Motherboard);
            if (cpu == null || mobo == null) yield break;

            var cpuSocket = GetFieldValue(cpu, PcConfiguratorConstants.Categories.Cpu, PcConfiguratorConstants.Fields.SocketType);
            var moboSocket = GetFieldValue(mobo, PcConfiguratorConstants.Categories.Motherboard, PcConfiguratorConstants.Fields.SocketType);

            if (cpuSocket == null) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R1, PcConfiguratorConstants.Categories.Cpu, PcConfiguratorConstants.Fields.SocketType, cpu); yield break; }
            if (moboSocket == null) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R1, PcConfiguratorConstants.Categories.Motherboard, PcConfiguratorConstants.Fields.SocketType, mobo); yield break; }

            if (!string.Equals(cpuSocket, moboSocket, StringComparison.OrdinalIgnoreCase))
                yield return new IncompatibilityDTO
                {
                    Rule = PcConfiguratorConstants.RuleCodes.R1,
                    CategoryA = PcConfiguratorConstants.Categories.Cpu,
                    CategoryB = PcConfiguratorConstants.Categories.Motherboard,
                    Message = $"CPU socket '{cpuSocket}' does not match Motherboard socket '{moboSocket}'."
                };
        }
    }
}
