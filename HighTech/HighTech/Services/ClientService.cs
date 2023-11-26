using HighTech.Abstraction;
using HighTech.Data;
using HighTech.Models;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Services
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext context;

        public ClientService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public Client CreateClient(string address, string userId)
        {
            if (context.Clients.Any(x => x.UserId == userId))
            {
                throw new InvalidOperationException("User already exist.");
            }

            var client = new Client
            {
                Address = address,
                UserId = userId,
                User = context.Users.Find(userId)
            };

            context.Clients.Add(client);
            context.SaveChanges();

            return client;
        }

        public Client GetClient(string id)
        {
            return context.Clients.FirstOrDefault(x => x.UserId == id);
        }

        public Client GetClientByUsername(string username)
        {
            return context.Clients.Include(c => c.User).FirstOrDefault(x => x.User.UserName == username);
        }

        public List<Client> GetClients()
        {
            return context.Clients.ToList();
        }

        //TODO
        public string GetFullName(string clientId)
        {
            throw new NotImplementedException();
        }
        //TODO
        public bool Remove(string clientId)
        {
            throw new NotImplementedException();
        }

        public bool Update(string id, string firstName, string lastName, string phone, string address)
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
