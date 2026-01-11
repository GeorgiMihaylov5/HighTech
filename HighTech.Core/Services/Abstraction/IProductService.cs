using HighTech.Core.Entities;

namespace HighTech.Core.Services.Abstraction
{
	public interface IProductService
	{
		ICollection<Product> GetAll();
		ICollection<Product> GetByCategory(string? categoryId);
		Product? Get(string? id);
		ICollection<Product> GetMostSellers(int top = 6);
        ICollection<Product> GetLowStockProducts(int threshold = 10);
        Product Create(string? manufacturer, string? model, int warranty, decimal price, decimal discount, int quantity, string? image, string? categoryId);
		Product Edit(string? id, string? manufacturer, string? model, int warranty, decimal price, decimal discount, int quantity, string? image, string? categoryId);
		bool Remove(string? id);
		Product? IncreaseDiscount(string? id, int percentage);
		Product? RemoveDiscount(string? id);
		bool SetProductFieldValues(string? productId, Dictionary<string, string?> fieldValues);
		bool UpdateStock(string? id, int quantity);
		bool IsInStock(string? id, int requestedQuantity = 1);
	}
}
