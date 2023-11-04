using HighTech.Abstraction;
using HighTech.Data;
using HighTech.Models;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext context;

        public ProductService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public Product Get(string id)
        {
            return context.Products.Where(x => x.IsRemoved != true)
                .Include(p => p.ProductFields)
                .FirstOrDefault(x => x.Id == id);
        }

        public ICollection<Product> GetAll()
        {
            return context.Products
                .Where(x => x.IsRemoved != true)
                .Include(p => p.ProductFields)
                .ToList();
        }

        public Product Create(string manufacturer, string model, int warranty, decimal price, decimal discount, int quantity, string image)
        {
            var product = new Product()
            {
                Manufacturer = manufacturer,
                Model = model,
                Warranty = warranty,
                Price = price,
                Discount = discount,
                Quantity = quantity,
                Image = image
            };

            context.Products.Add(product);
            context.SaveChanges();

            return product;
        }
        public Product Edit(string id, string manufacturer, string model, int warranty, decimal price, decimal discount, int quantity, string image)
        {
            var product = Get(id);

            if (product is null)
            {
                return null;
            }

            product.Manufacturer = manufacturer;
            product.Model = model;
            product.Warranty = warranty;
            product.Price = price;
            product.Discount = discount;
            product.Quantity = quantity;
            product.Image = image;

            context.Update(product);
            context.SaveChanges();

            return product;
        }

        public bool Remove(string id)
        {
            var product = Get(id);

            if (product is null)
            {
                return false;
            }

            product.IsRemoved = true;
            context.Products.Update(product);

            return context.SaveChanges() != 0;
        }

        public bool MakeDiscount(string id, int discount)
        {
            var product = Get(id);

            if (product is null)
            {
                return false;
            }

            product.Discount = product.Price * discount / 100;
            product.Price -= product.Discount;

            context.Products.Update(product);

            return context.SaveChanges() != 0;
        }

        public bool RemoveDiscount(string id)
        {
            var product = Get(id);

            if (product is null)
            {
                return false;
            }

            product.Price += product.Discount;
            product.Discount = 0;

            context.Products.Update(product);

            return context.SaveChanges() != 0;
        }
    }
}
