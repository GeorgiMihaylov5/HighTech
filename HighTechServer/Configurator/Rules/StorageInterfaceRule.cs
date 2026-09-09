using HighTech.DTOs;
using HighTech.Models;

namespace HighTech.Configurator.Rules
{
    public class StorageInterfaceRule : CompatibilityRuleBase
    {
        public override IEnumerable<IncompatibilityDTO> Check(IReadOnlyDictionary<string, Product> sel)
        {
            var storage = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.Storage);
            var mobo = sel.GetValueOrDefault(PcConfiguratorConstants.Categories.Motherboard);
            if (storage == null || mobo == null) yield break;

            var interfaceVal = GetFieldValue(storage, PcConfiguratorConstants.Categories.Storage, PcConfiguratorConstants.Fields.StorageInterface);
            if (string.IsNullOrWhiteSpace(interfaceVal)) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R9, PcConfiguratorConstants.Categories.Storage, PcConfiguratorConstants.Fields.StorageInterface, storage); yield break; }

            if (string.Equals(interfaceVal, "NVMe", StringComparison.OrdinalIgnoreCase))
            {
                var m2SlotsStr = GetFieldValue(mobo, PcConfiguratorConstants.Categories.Motherboard, PcConfiguratorConstants.Fields.M2Slots);
                if (!TryParseInt(m2SlotsStr, out var m2Slots)) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R9, PcConfiguratorConstants.Categories.Motherboard, PcConfiguratorConstants.Fields.M2Slots, mobo); yield break; }
                if (m2Slots < 1)
                    yield return new IncompatibilityDTO
                    {
                        Rule = PcConfiguratorConstants.RuleCodes.R9,
                        CategoryA = PcConfiguratorConstants.Categories.Storage,
                        CategoryB = PcConfiguratorConstants.Categories.Motherboard,
                        Message = "Motherboard has no M.2 slots for the selected NVMe storage drive."
                    };
            }
            else if (string.Equals(interfaceVal, "SATA", StringComparison.OrdinalIgnoreCase))
            {
                var sataPortsStr = GetFieldValue(mobo, PcConfiguratorConstants.Categories.Motherboard, PcConfiguratorConstants.Fields.SataPorts);
                if (!TryParseInt(sataPortsStr, out var sataPorts)) { yield return MissingField(PcConfiguratorConstants.RuleCodes.R9, PcConfiguratorConstants.Categories.Motherboard, PcConfiguratorConstants.Fields.SataPorts, mobo); yield break; }
                if (sataPorts < 1)
                    yield return new IncompatibilityDTO
                    {
                        Rule = PcConfiguratorConstants.RuleCodes.R9,
                        CategoryA = PcConfiguratorConstants.Categories.Storage,
                        CategoryB = PcConfiguratorConstants.Categories.Motherboard,
                        Message = "Motherboard has no SATA ports for the selected SATA storage drive."
                    };
            }
        }
    }
}
