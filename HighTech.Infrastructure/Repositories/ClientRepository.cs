using HighTech.Core.Entities;
using HighTech.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Infrastructure.Repositories
{
	public class ClientRepository : IClientRepository
	{
		private readonly ApplicationDbContext context;

		public ClientRepository(ApplicationDbContext _context)
		{
			context = _context;
		}

		public Client CreateClient(string? address, string? userId)
		{
			if (context.Clients.Any(x => x.UserId == userId))
			{
				throw new InvalidOperationException($"Client for user ID '{userId}' already exists.");
			}

			// Validate user exists
			var userExists = context.Users.Any(u => u.Id == userId);
			if (!userExists)
			{
				throw new InvalidOperationException($"User with ID '{userId}' does not exist.");
			}

			var client = new Client
			{
				Address = address,
				UserId = userId
			};

			context.Clients.Add(client);
			context.SaveChanges();

			return client;
		}

		public Client? GetClient(string? id)
		{
			return context.Clients.FirstOrDefault(x => x.UserId == id);
		}

		public Client? GetClientByUsername(string? username)
		{
			var user = context.Users.FirstOrDefault(x => x.UserName == username);
			if (user == null) return null;
			return context.Clients.FirstOrDefault(x => x.UserId == user.Id);
		}

		public List<Client> GetClients()
		{
			return context.Clients.ToList();
		}

		public bool Update(string? id, string? firstName, string? lastName, string? phone, string? address)
		{
			var client = context.Clients.Find(id);

			if (client == null)
			{
				return false;
			}
			var user = context.Users.FirstOrDefault(x => x.Id == client.UserId);

			if (user == null)
			{
				return false;
			}

			user.FirstName = firstName;
			user.LastName = lastName;
			user.PhoneNumber = phone;
			client.Address = address;

			context.Clients.Update(client);
			context.Users.Update(user);

			return context.SaveChanges() != 0;
		}
	}
}
