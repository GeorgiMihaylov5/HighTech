namespace HighTech.DTOs
{
    public class ConfiguratorValidationResultDTO
    {
        public bool IsValid { get; set; }
        public ICollection<IncompatibilityDTO> Issues { get; set; } = new List<IncompatibilityDTO>();
        public ICollection<OrderedProductDTO> ResolvedItems { get; set; } = new List<OrderedProductDTO>();
        public bool HasIntegratedGraphics { get; set; }
    }
}
