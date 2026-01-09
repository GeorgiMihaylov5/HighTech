using HighTech.Core.Entities;
using System.Linq.Expressions;

namespace HighTech.Core.Repositories
{
	public interface IProductRepository
	{
		public ICollection<Product> GetAll();
		public ICollection<Product> GetWhere(Expression<Func<Product, bool>> predicate);
		public ICollection<Product> GetByCategory(string? categoryId);
		public Product? Get(string? id);
		public ICollection<Product> GetMostSellers(int top);
		public Product Create(string? manufacturer, string? model, int warranty, decimal price, decimal discount, int quantity, string? image, string? categoryId);
		public Product? Edit(string? id, string? manufacturer, string? model, int warranty, decimal price, decimal discount, int quantity, string? image, string? categoryId);
		public bool Remove(string? id);
		public Product? IncreaseDiscount(string? id, int percentage);
		public Product? RemoveDiscount(string? id);
	}
}
