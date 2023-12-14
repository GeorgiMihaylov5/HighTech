using AutoMapper.Internal;
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

        public ICollection<Product> GetMostSellers()
        {
            return context.OrderedProducts
                .GroupBy(x => x.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    Count = g.Sum(x => x.Count)
                })
                .OrderByDescending(x => x.Count)
                .Join(context.Products
                    .Where(x => x.IsRemoved != true)
                    .Include(p => p.ProductFields),
                    orderedProduct => orderedProduct.ProductId,
                    product => product.Id,
                    (orderedProduct, product) => product)
                .Take(6)
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

        public Product IncreaseDiscount(string id, int percentage)
        {
            var product = Get(id);

            if (product is null)
            {
                return null;
            }

            if (product.Discount != 0)
            {
                product.Price += product.Discount;
                percentage += (int)(product.Discount * 100 / product.Price);
            }

            if (percentage > 100)
            {
                throw new InvalidDataException("Percentage cannot be highter that 100!");
            }

            product.Discount = product.Price * percentage / 100;
            product.Price -= product.Discount;

            context.Products.Update(product);
            context.SaveChanges();

            return product;
        }

        public Product RemoveDiscount(string id)
        {
            var product = Get(id);

            if (product is null)
            {
                return null;
            }

            product.Price += product.Discount;
            product.Discount = 0;

            context.Products.Update(product);
            context.SaveChanges();

            return product;
        }
    }
}
