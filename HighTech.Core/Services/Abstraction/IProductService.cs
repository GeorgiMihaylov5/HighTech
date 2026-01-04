using HighTech.Core.Entities;

namespace HighTech.Core.Services.Abstraction
{
	public interface IProductService
	{
		// Product CRUD
		ICollection<Product> GetAll();
		ICollection<Product> GetByCategory(string categoryId);
		Product? Get(string id);
		ICollection<Product> GetMostSellers();
		Product? Create(string manufacturer, string model, int warranty, decimal price, decimal discount, int quantity, string image, string categoryId);
		Product? Edit(string id, string manufacturer, string model, int warranty, decimal price, decimal discount, int quantity, string image, string categoryId);
		bool Remove(string id);

		// Discount management
		Product? IncreaseDiscount(string id, int percentage);
		Product? RemoveDiscount(string id);

		// Product field values management (product-specific attribute values)
		ICollection<ProductFieldValue> GetProductFieldValues(string productId);
		ProductFieldValue? SetProductFieldValue(string productId, string fieldId, string value);
		bool RemoveProductFieldValue(string productId, string fieldId);

		// Bulk set field values for a product (useful when creating/editing product with all its attributes)
		bool SetProductFieldValues(string productId, Dictionary<string, string> fieldValues);

		// Inventory management
		ICollection<Product> GetLowStockProducts(int threshold = 10);
		bool UpdateStock(string id, int quantity);
		bool IsInStock(string id, int requestedQuantity = 1);

		// Advanced search and filtering
		ICollection<Product> SearchProducts(string searchTerm);
		ICollection<Product> FilterByPriceRange(decimal minPrice, decimal maxPrice);
		ICollection<Product> GetProductsByManufacturer(string manufacturer);
	}
}
