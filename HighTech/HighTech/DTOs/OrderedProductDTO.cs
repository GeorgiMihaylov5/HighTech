namespace HighTech.DTOs
{
    public class OrderedProductDTO
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public ProductDTO Product { get; set; }
        public decimal OrderedPrice { get; set; }
        public int Count { get; set; }
    }
}
