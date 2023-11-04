namespace HighTech.DTOs
{
    public class ProductDTO
    {
        public string Id { get; set; } 
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int Warranty { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; }
        public string CategoryName { get; set; } 
        public ICollection<FieldDTO> Fields { get; set; }
    }
}
