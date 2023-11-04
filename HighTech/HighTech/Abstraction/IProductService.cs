using HighTech.Models;
using Newtonsoft.Json.Linq;

namespace HighTech.Abstraction
{
    public interface IProductService
    {
        public ICollection<Product> GetAll();
        public Product Get(string id);
        public ICollection<ProductField> GetProductFields(string id);
        public Product Create(string manufacturer, string model, int warranty, decimal price, decimal discount, int quantity, string image);
        public Product Edit(string id, string manufacturer, string model, int warranty, decimal price, decimal discount, int quantity, string image);
        public ProductField AddProductField(string productId, string fieldId, string value);
        public bool EditProductFieldValue(string pfId, string value);
        public bool Remove(string id);
        public bool MakeDiscount(string id, int discount);
        public bool RemoveDiscount(string id);


        //TODO
        // List<Product> Search(string filter, int minPrice, int maxPrice, ICollection<string> manufacturers, ICollection<string> models, IEnumerable<Product> oldProducts);
    }
}
