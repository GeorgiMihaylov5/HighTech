using HighTech.Core.Entities;
using HighTech.Core.Repositories;
using HighTech.Core.Services.Abstraction;

namespace HighTech.Core.Services
{
	public class ClientService : IClientService
	{
		private readonly IClientRepository clientRepository;

		public ClientService(IClientRepository _clientRepository)
		{
			clientRepository = _clientRepository;
		}

		public Client CreateClient(string? address, string? userId)
		{
			return clientRepository.CreateClient(address, userId);
		}

		public Client? GetClient(string? id)
		{
			return clientRepository.GetClient(id);
		}

		public Client? GetClientByUsername(string? username)
		{
			return clientRepository.GetClientByUsername(username);
		}

		public List<Client> GetClients()
		{
			return clientRepository.GetClients();
		}

		public bool Update(string? id, string? firstName, string? lastName, string? phone, string? address)
		{
			return clientRepository.Update(id, firstName, lastName, phone, address);
		}
	}
}
