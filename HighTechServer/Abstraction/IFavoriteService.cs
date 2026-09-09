using HighTech.Models;

namespace HighTech.Abstraction
{
    public interface IFavoriteService
    {
        public Favorite Add(string productId, string username);
        public bool Remove(string productId, string username);
        public bool Clear(string username);
        public ICollection<Favorite> GetByUsername(string username);
        public int GetCount(string username);
        public bool IsFavorite(string productId, string username);
    }
}
