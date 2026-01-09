namespace HighTech.Core.Entities
{
	public class FavoriteProduct
	{
		public string? Id { get; set; }
		public string? UserId { get; set; }
		public AppUser? User { get; set; }
		public string? ProductId { get; set; }
		public Product? Product { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
