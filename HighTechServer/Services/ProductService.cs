using HighTech.Abstraction;
using HighTech.Configurator;
using HighTech.Data;
using HighTech.Models;
using HighTech.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HighTech.Services
{
	public class ProductService : IProductService
	{
		private readonly ApplicationDbContext context;
		private readonly ProductOptions options;

		public ProductService(ApplicationDbContext _context, IOptions<ProductOptions> options)
		{
			context = _context;
			this.options = options.Value;
		}

		public Product Get(string id)
		{
			return context.Products.Where(x => x.IsRemoved != true)
				.Include(p => p.ProductFields)
				.FirstOrDefault(x => x.Id == id);
		}

		public ICollection<Product> GetAll()
		{
			return ProductsVisibleOnProductsPage()
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
				.Join(ProductsVisibleOnProductsPage()
					.Include(p => p.ProductFields),
					orderedProduct => orderedProduct.ProductId,
					product => product.Id,
					(orderedProduct, product) => product)
				.Take(Math.Max(1, options.MostSellersCount))
				.ToList();
		}

		private IQueryable<Product> ProductsVisibleOnProductsPage()
		{
			return context.Products.Where(product =>
				product.IsRemoved != true
				&& !(product.Manufacturer == PcConfiguratorConstants.AssemblyProduct.Manufacturer
					&& product.Model == PcConfiguratorConstants.AssemblyProduct.Model));
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

            if (percentage < 0)
            {
                throw new InvalidDataException("Percentage cannot be lower than 0!");
            }

            if (percentage > 100)
			{
				throw new InvalidDataException("Percentage cannot be higher than 100!");
			}

			var basePrice = product.Price + product.Discount;

			product.Discount = basePrice * percentage / 100;
			product.Price = basePrice - product.Discount;

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
