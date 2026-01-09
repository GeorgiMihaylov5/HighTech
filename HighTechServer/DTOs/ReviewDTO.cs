namespace HighTech.DTOs
{
	public class ReviewDTO
	{
		public string? Id { get; set; }
		public int Rating { get; set; }
		public string? Comment { get; set; }
		public bool IsAnonymous { get; set; }
		public string? UserId { get; set; }
		public string? Username { get; set; }
		public string? UserFullName { get; set; }
		public string? ProductId { get; set; }
		public string? ProductName { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
	}
}
