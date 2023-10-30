using HighTech.Abstraction;
using HighTech.Data;
using HighTech.Models;
using static Duende.IdentityServer.Models.IdentityResources;

namespace HighTech.Services
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext context;

        public ClientService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public bool CreateClient(string address, string userId)
        {
            if (context.Clients.Any(x => x.UserId == userId))
            {
                throw new InvalidOperationException("User already exist.");
            }

            context.Clients.Add(new Client
            {
                Address = address,
                UserId = userId,
                User = context.Users.Find(userId)
            });
            return context.SaveChanges() != 0;
        }

        public Client? GetClient(string id)
        {
            return context.Clients.FirstOrDefault(x => x.UserId == id);
        }

        public List<Client> GetClients()
        {
            return context.Clients.ToList();
        }

        public string GetFullName(string clientId)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string clientId)
        {
            throw new NotImplementedException();
        }

        public bool Update(string id, string phone, string address)
        {
            throw new NotImplementedException();
        }
    }
}
