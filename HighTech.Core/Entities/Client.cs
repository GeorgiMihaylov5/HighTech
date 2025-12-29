namespace HighTech.Core.Entities
{
	public class Client
	{
		public string? Id { get; set; }
		public string? Address { get; set; }
		public string? UserId { get; set; }
		public AppUser? User { get; set; }
	}
}
