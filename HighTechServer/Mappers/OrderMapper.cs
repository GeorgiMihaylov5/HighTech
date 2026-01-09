using HighTech.Core.Entities;
using HighTech.DTOs;

namespace HighTech.Mappers
{
	public static class OrderMapper
	{
		public static OrderDTO ToDTO(Order order)
		{
			if (order is null)
			{
				return null!;
			}

			return new OrderDTO
			{
				Id = order.Id,
				OrderedOn = order.OrderedOn.Ticks.ToString(),
				Status = order.Status,
				Notes = order.Notes,
				OrderedProducts = order?.OrderedProducts?.Select(op => new OrderedProductDTO()
				{
					Id = op.Id,
					Count = op.Count,
					OrderedPrice = op.OrderedPrice,
					ProductId = op.ProductId,
					Product = new ProductDTO()
					{
						Id = op.ProductId,
						Manufacturer = op?.Product?.Manufacturer,
						Model = op?.Product?.Model,
						Warranty = op?.Product?.Warranty ?? 0,
						Image = op?.Product?.Image,
					}
				}).ToList(),
				User = new UserDTO()
				{
					UserId = order?.CustomerId,
					FirstName = order?.Customer?.FirstName,
					LastName = order?.Customer?.LastName,
					Email = order?.Customer?.Email,
					Username = order?.Customer?.UserName,
					PhoneNumber = order?.Customer?.PhoneNumber,
				}
			};
		}

		public static List<OrderDTO> ToDTOList(ICollection<Order> orders)
		{
			return orders?.Select(o => ToDTO(o)).ToList() ?? new List<OrderDTO>();
		}
	}
}
