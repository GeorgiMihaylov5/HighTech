using HighTech.Models;
using Newtonsoft.Json.Linq;

namespace HighTech.Abstraction
{
    public interface IProductService
    {
        public ICollection<Product> GetAll();
        public Product Get(string id);
        public Product Create(string manufacturer, string model, int warranty, decimal price, decimal discount, int quantity, string image);
        public Product Edit(string id, string manufacturer, string model, int warranty, decimal price, decimal discount, int quantity, string image);
        public bool Remove(string id);
        public Product IncreaseDiscount(string id, int percentage);
        public Product RemoveDiscount(string id);
    }
}
