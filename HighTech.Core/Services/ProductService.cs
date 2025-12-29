using HighTech.Core.Entities;
using HighTech.Core.Repositories;
using HighTech.Core.Services.Abstraction;

namespace HighTech.Core.Services
{
	public class ProductService : IProductService
	{
		private readonly IProductRepository productRepository;

		public ProductService(IProductRepository _productRepository)
		{
			productRepository = _productRepository;
		}

		public Product Get(string id)
		{
			return productRepository.Get(id);
		}

		public ICollection<Product> GetAll()
		{
			return productRepository.GetAll();
		}

		public ICollection<Product> GetMostSellers()
		{
			return productRepository.GetMostSellers();
		}

		public Product Create(string manufacturer, string model, int warranty, decimal price, decimal discount, int quantity, string image)
		{
			return productRepository.Create(manufacturer, model, warranty, price, discount, quantity, image);
		}

		public Product Edit(string id, string manufacturer, string model, int warranty, decimal price, decimal discount, int quantity, string image)
		{
			return productRepository.Edit(id, manufacturer, model, warranty, price, discount, quantity, image);
		}

		public bool Remove(string id)
		{
			return productRepository.Remove(id);
		}

		public Product IncreaseDiscount(string id, int percentage)
		{
			return productRepository.IncreaseDiscount(id, percentage);
		}

		public Product RemoveDiscount(string id)
		{
			return productRepository.RemoveDiscount(id);
		}
	}
}
