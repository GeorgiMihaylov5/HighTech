using HighTech.Abstraction;
using HighTech.Data;
using HighTech.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        public Product? Get(string id)
        {
            return context.Products.Where(x => x.IsRemoved != true)
                .Include(p => p.ProductFields)
                .FirstOrDefault(x => x.Id == id);
        }

        public List<Product> GetAll()
        {
            return context.Products
                .Where(x => x.IsRemoved != true)
                .Include(p => p.ProductFields)
                .ToList();
        }

        public bool Create(string manufacturer, string model, int warranty, decimal price, decimal discount, int quantity, string image, ICollection<ProductField> fields)
        {
            context.Products.Add(new Product()
            {
                Manufacturer = manufacturer,
                Model = model,
                Warranty = warranty,
                Price = price,
                Discount = discount,
                Quantity = quantity,
                Image = image,
                ProductFields = fields
            });

            return context.SaveChanges() != 0;
        }

        public bool Edit(string id, string manufacturer, string model, int warranty, decimal price, decimal discount, int quantity, string image, ICollection<ProductField> fields)
        {
            var product = Get(id);

            if (product is null)
            {
                return false;
            }

            product.Manufacturer = manufacturer;
            product.Model = model;
            product.Warranty = warranty;
            product.Price = price;
            product.Discount = discount;
            product.Quantity = quantity;
            product.Image = image;
            product.ProductFields = fields;

            context.Update(product);

            return context.SaveChanges() != 0;
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
