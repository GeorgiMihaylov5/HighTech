namespace HighTech.Abstraction
{
    public interface IConfiguratorMetadataGuard
    {
        bool IsSystemCategoryName(string name);
        bool IsSystemFieldName(string name);
        bool IsRequiredPairing(string categoryName, string fieldName);
        bool IsConfiguratorOnlyCategory(string name);
        string LockReason { get; }
    }
}

