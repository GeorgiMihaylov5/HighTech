using HighTech.Models;

namespace HighTech.Abstraction
{
    public interface IClientService
    {
        public List<Client> GetClients();
        public Client GetClient(string id);
        public Client CreateClient(string address, string userId);
        public bool Remove(string clientId);
        public string GetFullName(string clientId);
        public bool Update(string id, string phone, string address);
    }
}
