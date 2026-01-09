namespace HighTech.Core.Entities
{
	public class Review
	{
		public string? Id { get; set; }
		public int Rating { get; set; } // 1 to 5
		public string? Comment { get; set; }
		public bool IsAnonymous { get; set; }
		public string? UserId { get; set; }
		public AppUser? User { get; set; }
		public string? ProductId { get; set; }
		public Product? Product { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
	}
}
