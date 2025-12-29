namespace HighTech.Core.Entities
{
	public class Employee
	{
		public string? Id { get; set; }
		public string? JobTitle { get; set; }
		public string? UserId { get; set; }
		public AppUser? User { get; set; }
	}
}
