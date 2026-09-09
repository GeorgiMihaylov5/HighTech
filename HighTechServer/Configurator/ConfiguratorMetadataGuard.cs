using HighTech.Abstraction;

namespace HighTech.Configurator
{
    public class ConfiguratorMetadataGuard : IConfiguratorMetadataGuard
    {
        private static readonly HashSet<string> SystemCategories =
            new(PcConfiguratorConstants.Categories.All, StringComparer.OrdinalIgnoreCase);

        private static readonly HashSet<string> SystemFields =
            new(PcConfiguratorConstants.Fields.All, StringComparer.OrdinalIgnoreCase);

        private static readonly HashSet<(string, string)> RequiredPairings =
            new(PcConfiguratorConstants.RequiredPairings
                .Select(p => (p.Category.ToLowerInvariant(), p.Field.ToLowerInvariant())));

        public string LockReason => PcConfiguratorConstants.LockReason;

        public bool IsSystemCategoryName(string name) =>
            !string.IsNullOrWhiteSpace(name) && SystemCategories.Contains(name);

        public bool IsSystemFieldName(string name) =>
            !string.IsNullOrWhiteSpace(name) && SystemFields.Contains(name);

        public bool IsRequiredPairing(string categoryName, string fieldName) =>
            !string.IsNullOrWhiteSpace(categoryName)
            && !string.IsNullOrWhiteSpace(fieldName)
            && RequiredPairings.Contains((categoryName.ToLowerInvariant(), fieldName.ToLowerInvariant()));

        public bool IsConfiguratorOnlyCategory(string name) =>
            !string.IsNullOrWhiteSpace(name)
            && PcConfiguratorConstants.Categories.ConfiguratorOnly.Contains(name);
    }
}

