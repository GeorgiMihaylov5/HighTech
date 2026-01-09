using HighTech.Core.Entities;
using HighTech.DTOs;

namespace HighTech.Mappers
{
	public static class ProductMapper
	{
		public static ProductDTO ToDTO(Product product)
		{
			if (product is null)
			{
				return null!;
			}

			var dto = new ProductDTO()
			{
				Id = product.Id,
				Manufacturer = product.Manufacturer,
				Model = product.Model,
				Price = product.Price,
				Warranty = product.Warranty,
				Discount = product.Discount,
				Image = product.Image,
				Quantity = product.Quantity,
				CategoryId = product.CategoryID,
				CategoryName = product.Category?.Name,
				Fields = new List<FieldDTO>()
			};

			// Add product field values
			if (product.ProductFieldValues is not null && product.ProductFieldValues.Count > 0)
			{
				foreach (var pf in product.ProductFieldValues)
				{
					dto.Fields.Add(new FieldDTO()
					{
						Id = pf.Field!.Id,
						Name = pf.Field.Name,
						TypeCode = pf.Field.TypeCode,
						Value = pf.Value
					});
				}
			}

			return dto;
		}

		public static ICollection<ProductDTO> ToDTOList(ICollection<Product> products)
		{
            return products.Select(ToDTO).ToList();
		}
	}
}
