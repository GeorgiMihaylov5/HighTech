namespace HighTech.DTOs
{
	public class CreateReviewRequest
	{
		public string? ProductId { get; set; }
		public int Rating { get; set; }
		public string? Comment { get; set; }
		public bool IsAnonymous { get; set; }
	}

	public class UpdateReviewRequest
	{
		public int Rating { get; set; }
		public string? Comment { get; set; }
	}
}
