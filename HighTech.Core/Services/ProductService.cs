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

		// Product CRUD
		public Product? Get(string id)
		{
			return productRepository.Get(id);
		}

		public ICollection<Product> GetAll()
		{
			return productRepository.GetAll();
		}

		public ICollection<Product> GetByCategory(string categoryId)
		{
			return productRepository.GetByCategory(categoryId);
		}

		public ICollection<Product> GetMostSellers()
		{
			return productRepository.GetMostSellers();
		}

		public Product? Create(string manufacturer, string model, int warranty, decimal price, decimal discount, int quantity, string image, string categoryId)
		{
			// Business validation
			if (string.IsNullOrWhiteSpace(manufacturer))
			{
				throw new ArgumentException("Manufacturer is required.", nameof(manufacturer));
			}

			if (string.IsNullOrWhiteSpace(model))
			{
				throw new ArgumentException("Model is required.", nameof(model));
			}

			if (price < 0)
			{
				throw new InvalidOperationException("Price cannot be negative.");
			}

			if (discount < 0 || discount > price)
			{
				throw new InvalidOperationException("Discount must be between 0 and price.");
			}

			if (quantity < 0)
			{
				throw new InvalidOperationException("Quantity cannot be negative.");
			}

			if (warranty < 0)
			{
				throw new InvalidOperationException("Warranty cannot be negative.");
			}

			// Validate category exists
			var category = categoryRepository.Get(categoryId);
			if (category == null)
			{
				throw new InvalidOperationException($"Category with ID '{categoryId}' does not exist.");
			}

			return productRepository.Create(manufacturer, model, warranty, price, discount, quantity, image, categoryId);
		}

		public Product? Edit(string id, string manufacturer, string model, int warranty, decimal price, decimal discount, int quantity, string image, string categoryId)
		{
			// Business validation
			if (string.IsNullOrWhiteSpace(manufacturer))
			{
				throw new ArgumentException("Manufacturer is required.", nameof(manufacturer));
			}

			if (string.IsNullOrWhiteSpace(model))
			{
				throw new ArgumentException("Model is required.", nameof(model));
			}

			if (price < 0)
			{
				throw new InvalidOperationException("Price cannot be negative.");
			}

			if (discount < 0 || discount > price)
			{
				throw new InvalidOperationException("Discount must be between 0 and price.");
			}

			if (quantity < 0)
			{
				throw new InvalidOperationException("Quantity cannot be negative.");
			}

			// Validate category exists
			var category = categoryRepository.Get(categoryId);
			if (category == null)
			{
				throw new InvalidOperationException($"Category with ID '{categoryId}' does not exist.");
			}

			return productRepository.Edit(id, manufacturer, model, warranty, price, discount, quantity, image, categoryId);
		}

		public bool Remove(string id)
		{
			return productRepository.Remove(id);
		}

		// Discount management
		public Product? IncreaseDiscount(string id, int percentage)
		{
			if (percentage <= 0 || percentage > 100)
			{
				throw new InvalidOperationException("Percentage must be between 1 and 100.");
			}

			return productRepository.IncreaseDiscount(id, percentage);
		}

		public Product? RemoveDiscount(string id)
		{
			return productRepository.RemoveDiscount(id);
		}

		// Product field values management
		public ICollection<ProductFieldValue> GetProductFieldValues(string productId)
		{
			return fieldRepository.GetProductFieldValues(productId);
		}

		public ProductFieldValue? SetProductFieldValue(string productId, string fieldId, string value)
		{
			// Validate product exists
			var product = productRepository.Get(productId);
			if (product == null)
			{
				throw new InvalidOperationException($"Product with ID '{productId}' does not exist.");
			}

			// Validate field exists and belongs to product's category
			var categoryFields = categoryRepository.GetCategoryFields(product.CategoryID!);
			var fieldBelongsToCategory = categoryFields.Any(cf => cf.FieldId == fieldId);

			if (!fieldBelongsToCategory)
			{
				throw new InvalidOperationException($"Field with ID '{fieldId}' is not associated with the product's category.");
			}

			return fieldRepository.AddProductFieldValue(productId, fieldId, value);
		}

		public bool RemoveProductFieldValue(string productId, string fieldId)
		{
			return fieldRepository.RemoveProductFieldValue(productId, fieldId);
		}

		public bool SetProductFieldValues(string productId, Dictionary<string, string> fieldValues)
		{
			// Validate product exists
			var product = productRepository.Get(productId);
			if (product == null)
			{
				throw new InvalidOperationException($"Product with ID '{productId}' does not exist.");
			}

			// Get valid fields for this product's category
			var categoryFields = categoryRepository.GetCategoryFields(product.CategoryID!);
			var validFieldIds = categoryFields.Select(cf => cf.FieldId).ToHashSet();

			// Validate all provided fields belong to the category
			var invalidFields = fieldValues.Keys.Where(fid => !validFieldIds.Contains(fid)).ToList();
			if (invalidFields.Any())
			{
				throw new InvalidOperationException($"Fields with IDs [{string.Join(", ", invalidFields)}] are not associated with the product's category.");
			}

			// Set each field value
			foreach (var kvp in fieldValues)
			{
				fieldRepository.AddProductFieldValue(productId, kvp.Key, kvp.Value);
			}

			return true;
		}

		// Inventory management
		public ICollection<Product> GetLowStockProducts(int threshold = 10)
		{
			if (threshold < 0)
			{
				throw new ArgumentException("Threshold cannot be negative.", nameof(threshold));
			}

			var allProducts = productRepository.GetAll();
			return allProducts.Where(p => p.Quantity <= threshold && p.Quantity > 0).ToList();
		}

		public bool UpdateStock(string id, int quantity)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw new ArgumentException("Product ID is required.", nameof(id));
			}

			if (quantity < 0)
			{
				throw new InvalidOperationException("Quantity cannot be negative.");
			}

			var product = productRepository.Get(id);
			if (product == null)
			{
				return false;
			}

			// Update through Edit method
			return productRepository.Edit(id, product.Manufacturer!, product.Model!, product.Warranty,
				product.Price, product.Discount, quantity, product.Image!, product.CategoryID!) != null;
		}

		public bool IsInStock(string id, int requestedQuantity = 1)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw new ArgumentException("Product ID is required.", nameof(id));
			}

			if (requestedQuantity <= 0)
			{
				throw new ArgumentException("Requested quantity must be greater than zero.", nameof(requestedQuantity));
			}

			var product = productRepository.Get(id);
			return product != null && product.Quantity >= requestedQuantity;
		}

		// Advanced search and filtering
		public ICollection<Product> SearchProducts(string searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
			{
				return new List<Product>();
			}

			var allProducts = productRepository.GetAll();
			searchTerm = searchTerm.ToLower();

			return allProducts.Where(p =>
				(p.Manufacturer?.ToLower().Contains(searchTerm) ?? false) ||
				(p.Model?.ToLower().Contains(searchTerm) ?? false) ||
				(p.Category?.Name?.ToLower().Contains(searchTerm) ?? false)
			).ToList();
		}

		public ICollection<Product> FilterByPriceRange(decimal minPrice, decimal maxPrice)
		{
			if (minPrice < 0)
			{
				throw new ArgumentException("Minimum price cannot be negative.", nameof(minPrice));
			}

			if (maxPrice < minPrice)
			{
				throw new ArgumentException("Maximum price cannot be less than minimum price.", nameof(maxPrice));
			}

			var allProducts = productRepository.GetAll();
			return allProducts.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
		}

		public ICollection<Product> GetProductsByManufacturer(string manufacturer)
		{
			if (string.IsNullOrWhiteSpace(manufacturer))
			{
				throw new ArgumentException("Manufacturer is required.", nameof(manufacturer));
			}

			var allProducts = productRepository.GetAll();
			return allProducts.Where(p =>
				p.Manufacturer?.Equals(manufacturer, StringComparison.OrdinalIgnoreCase) ?? false
			).ToList();
		}
	}
}
