using HighTech.Core.Entities;

namespace HighTech.Core.Services.Abstraction
{
	public interface IJWTService
	{
		public int ExpiresDays { get; }
		public string CreateJWT(AppUser user, IList<string> roles);
	}
}
