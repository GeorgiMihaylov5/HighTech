using HighTech.Models;

namespace HighTech.Abstraction
{
    public interface IProductService
    {
        public List<Product> GetAll();
        public Product? Get(string id);
        public bool Create(string manufacturer, string model, int warranty, decimal price, decimal discount, int quantity, string image, ICollection<ProductField> fields);
        public bool Edit(string id, string manufacturer, string model, int warranty, decimal price, decimal discount, int quantity, string image, ICollection<ProductField> fields);
        public bool Remove(string id);
        public bool MakeDiscount(string id, int discount);
        public bool RemoveDiscount(string id);
        //TODO
        // List<Product> Search(string filter, int minPrice, int maxPrice, ICollection<string> manufacturers, ICollection<string> models, IEnumerable<Product> oldProducts);
    }
}
