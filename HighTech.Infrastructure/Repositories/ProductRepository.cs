using HighTech.Core.Entities;
using HighTech.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HighTech.Infrastructure.Repositories
{
	public class ProductRepository : IProductRepository
	{
		private readonly ApplicationDbContext context;

		public ProductRepository(ApplicationDbContext _context)
		{
			context = _context;
		}

		public Product? Get(string? id)
		{
			return context.Products.Where(x => x.IsRemoved != true)
				.Include(p => p.Category)
				.Include(p => p.ProductFieldValues!)
					.ThenInclude(pf => pf.Field)
				.FirstOrDefault(x => x.Id == id);
		}

		public ICollection<Product> GetAll()
		{
			return context.Products
				.Where(x => x.IsRemoved != true)
				.Include(p => p.Category)
				.Include(p => p.ProductFieldValues!)
					.ThenInclude(pf => pf.Field)
				.ToList();
		}

		public ICollection<Product> GetWhere(Expression<Func<Product, bool>> predicate)
		{
			return context.Products
				.Where(x => x.IsRemoved != true)
				.Where(predicate)
				.Include(p => p.Category)
				.Include(p => p.ProductFieldValues!)
					.ThenInclude(pf => pf.Field)
				.ToList();
		}

		public ICollection<Product> GetByCategory(string? categoryId)
		{
			return context.Products
				.Where(x => x.IsRemoved != true && x.CategoryID == categoryId)
				.Include(p => p.Category)
				.Include(p => p.ProductFieldValues!)
					.ThenInclude(pf => pf.Field)
				.ToList();
		}

		public ICollection<Product> GetMostSellers(int top)
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
					.Include(p => p.Category)
					.Include(p => p.ProductFieldValues!)
						.ThenInclude(pf => pf.Field),
					orderedProduct => orderedProduct.ProductId,
					product => product.Id,
					(orderedProduct, product) => product)
				.Take(top)
				.ToList();
		}

		public Product Create(string? manufacturer, string? model, int warranty, decimal price, decimal discount, int quantity, string? image, string? categoryId)
		{
			var product = new Product()
			{
				Manufacturer = manufacturer,
				Model = model,
				Warranty = warranty,
				Price = price,
				Discount = discount,
				Quantity = quantity,
				Image = image,
				CategoryID = categoryId,
				IsRemoved = false
			};

			context.Products.Add(product);
			context.SaveChanges();

			return product;
		}

		public Product? Edit(string? id, string? manufacturer, string? model, int warranty, decimal price, decimal discount, int quantity, string? image, string? categoryId)
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
			product.CategoryID = categoryId;

			context.Update(product);
			context.SaveChanges();

			return product;
		}

		public bool Remove(string? id)
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

		public Product? IncreaseDiscount(string? id, int percentage)
		{
			var product = Get(id);

			if (product is null)
			{
				return null;
			}

			// Calculate new discount amount (logic moved to service)
			decimal currentPrice = product.Price + product.Discount;
			decimal totalDiscountPercent = percentage;

			if (product.Discount != 0)
			{
				product.Price += product.Discount;
				totalDiscountPercent += (int)(product.Discount * 100 / product.Price);
			}

			product.Discount = product.Price * totalDiscountPercent / 100;
			product.Price -= product.Discount;

			context.Products.Update(product);
			context.SaveChanges();

			return product;
		}

		public Product? RemoveDiscount(string? id)
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
