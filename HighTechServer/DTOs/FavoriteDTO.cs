namespace HighTech.DTOs
{
    public class FavoriteDTO
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public ProductDTO Product { get; set; }
    }
}
