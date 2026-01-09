using HighTech.Core.Entities;

namespace HighTech.Core.Repositories
{
	public interface IClientRepository
	{
		public List<Client> GetClients();
		public Client? GetClient(string? id);
		public Client? GetClientByUsername(string? username);
		public Client CreateClient(string? address, string? userId);
		public bool Update(string? id, string? firstName, string? lastName, string? phone, string? address);
	}
}
