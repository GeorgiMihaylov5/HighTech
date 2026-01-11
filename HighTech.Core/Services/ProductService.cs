using HighTech.Core.Entities;
using HighTech.Core.Repositories;
using HighTech.Core.Services.Abstraction;

namespace HighTech.Core.Services
{
	public class ProductService : IProductService
	{
		private readonly IProductRepository productRepository;
		private readonly IFieldRepository fieldRepository;
		private readonly ICategoryRepository categoryRepository;

		public ProductService(
			IProductRepository _productRepository,
			IFieldRepository _fieldRepository,
			ICategoryRepository _categoryRepository)
		{
			productRepository = _productRepository;
			fieldRepository = _fieldRepository;
			categoryRepository = _categoryRepository;
		}

		public Product? Get(string? id)
		{
			if (string.IsNullOrEmpty(id))
				return null;
			return productRepository.Get(id);
		}

		public ICollection<Product> GetAll()
		{
			return productRepository.GetAll();
		}

		public ICollection<Product> GetByCategory(string? categoryId)
		{
			if (string.IsNullOrEmpty(categoryId))
				return new List<Product>();
			return productRepository.GetByCategory(categoryId);
		}

		public ICollection<Product> GetMostSellers(int top = 6)
		{
			return productRepository.GetMostSellers(top);
		}

		public Product Create(string? manufacturer, string? model, int warranty, decimal price, decimal discount, int quantity, string? image, string? categoryId)
		{
			Guard.NotNullOrEmpty(manufacturer, nameof(manufacturer));
			Guard.NotNullOrEmpty(model, nameof(model));
			Guard.NotNegative(price, nameof(price));
			Guard.NotBetween(discount, 0, price, nameof(discount));
			Guard.NotNegative(quantity, nameof(quantity));
			Guard.NotNegative(warranty, nameof(warranty));

			//TODO custom exception or guard
			var category = categoryRepository.Get(categoryId);
			if (category == null)
				throw new InvalidOperationException($"Category with ID '{categoryId}' does not exist.");

			return productRepository.Create(manufacturer, model, warranty, price, discount, quantity, image, categoryId);
		}

		public Product Edit(string? id, string? manufacturer, string? model, int warranty, decimal price, decimal discount, int quantity, string? image, string? categoryId)
		{
			Guard.NotNullOrEmpty(id, nameof(id));
			Guard.NotNullOrEmpty(manufacturer, nameof(manufacturer));
			Guard.NotNullOrEmpty(model, nameof(model));
			Guard.NotNegative(price, nameof(price));
			Guard.NotBetween(discount, 0, price, nameof(discount));
			Guard.NotNegative(quantity, nameof(quantity));
			Guard.NotNegative(warranty, nameof(warranty));

			//TODO custom exception or guard
			var category = categoryRepository.Get(categoryId);
			if (category == null)
				throw new InvalidOperationException($"Category with ID '{categoryId}' does not exist.");

			//TODO add custom exception
			return productRepository.Edit(id, manufacturer, model, warranty, price, discount, quantity, image, categoryId)
				?? throw new NullReferenceException();
		}

		public bool Remove(string? id)
		{
			return productRepository.SoftDelete(id);
		}

		//TODO RENAME AND MODIFY and custom and remove nullable 
		public Product? IncreaseDiscount(string? id, int percentage)
		{
			Guard.NotBetween(percentage, 1, 100, nameof(percentage));
			if (percentage <= 0 || percentage > 100)
			{
				throw new InvalidOperationException("Percentage must be between 1 and 100.");
			}

			return productRepository.IncreaseDiscount(id, percentage);
		}

		public Product? RemoveDiscount(string? id)
		{
			return productRepository.RemoveDiscount(id);
		}

		public bool SetProductFieldValues(string? productId, Dictionary<string, string?> fieldValues)
		{
			var product = productRepository.Get(productId);
			Guard.NotNull(product, nameof(product));

			var categoryFields = categoryRepository.GetCategoryFields(product?.CategoryID!);
			var validFieldIds = categoryFields.Select(cf => cf.FieldId).ToHashSet();

			var invalidFields = fieldValues.Keys.Where(fid => !validFieldIds.Contains(fid)).ToList();
			if (invalidFields.Any())
			{
				//TODO custom exception
				throw new InvalidOperationException($"Fields with IDs [{string.Join(", ", invalidFields)}] are not associated with the product's category.");
			}

			// Set each field value
			foreach (var kvp in fieldValues)
			{
				fieldRepository.SetProductFieldValue(productId, kvp.Key, kvp.Value);
			}

			return true;
		}

		// Inventory management
		public ICollection<Product> GetLowStockProducts(int threshold = 10)
		{
			Guard.NotNegative(threshold, nameof(threshold));

			return productRepository.GetWhere(p => p.Quantity <= threshold && p.Quantity > 0);
		}

		//TODO check is needed
		public bool UpdateStock(string? id, int quantity)
		{
			Guard.NotNullOrEmpty(id, nameof(id));
			Guard.NotNegative(quantity, nameof(quantity));

			var product = productRepository.Get(id);
			if (product == null)
				return false;

			return productRepository.Edit(id, product.Manufacturer!, product.Model!, product.Warranty,
				product.Price, product.Discount, quantity, product.Image!, product.CategoryID!) != null;
		}

		//TODO check is needed
		public bool IsInStock(string? id, int requestedQuantity = 1)
		{
			Guard.NotNullOrEmpty(id, nameof(id));
			Guard.NotNegative(requestedQuantity, nameof(requestedQuantity));

			var product = productRepository.Get(id);
			return product != null && product.Quantity >= requestedQuantity;
		}
	}
}
