using HighTech.Core.Entities;
using HighTech.DTOs;

namespace HighTech.Mappers
{
	public static class ClientMapper
	{
		public static ClientDTO ToDTO(Client client)
		{
			if (client is null)
			{
				return null!;
			}

			return new ClientDTO()
			{
				Id = client.Id,
				UserId = client.UserId,
				Username = client?.User?.UserName,
				Email = client?.User?.Email,
				FirstName = client?.User?.FirstName,
				LastName = client?.User?.LastName,
				Address = client?.Address,
				PhoneNumber = client?.User?.PhoneNumber
			};
		}

		public static List<ClientDTO> ToDTOList(ICollection<Client> clients)
		{
			return clients?.Select(c => ToDTO(c)).ToList() ?? new List<ClientDTO>();
		}
	}
}
