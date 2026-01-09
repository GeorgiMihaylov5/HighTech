namespace HighTech.DTOs
{
	public class FavoriteProductDTO
	{
		public string? Id { get; set; }
		public string? UserId { get; set; }
		public string? ProductId { get; set; }
		public ProductDTO? Product { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
