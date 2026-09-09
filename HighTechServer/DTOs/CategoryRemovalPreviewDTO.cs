namespace HighTech.DTOs
{
    public class CategoryRemovalPreviewDTO
    {
        public int TotalAffectedProducts { get; set; }
        public ICollection<AffectedFieldDTO> AffectedFields { get; set; }
        public ICollection<string> SampleProducts { get; set; }
    }

    public class AffectedFieldDTO
    {
        public string FieldId { get; set; }
        public string FieldName { get; set; }
        public int ProductCount { get; set; }
        public ICollection<string> SampleProducts { get; set; }
    }
}
