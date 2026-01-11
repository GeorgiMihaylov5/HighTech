using HighTech.Core.Entities.Enum;
using HighTech.Core.Services.Abstraction;
using HighTech.DTOs;
using HighTech.Mappers;
using HighTechServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HighTech.Controllers
{
	[ApiController]
	[Route("[controller]/[action]")]
	public class OrdersController : Controller
	{
		private readonly IOrderService orderService;

		public OrdersController(IOrderService _orderService)
		{
			orderService = _orderService;
		}

		[Authorize(Roles = "Administrator,Employee")]
		public IActionResult GetOrders()
		{
			var orders = orderService.GetOrders()
				.Select(OrderMapper.ToDTO)
				.OrderByDescending(x => x.Status == OrderStatus.Pending)
				.ThenByDescending(x => x.Status == OrderStatus.Approved)
				.ThenByDescending(x => x.Status == OrderStatus.Completed).ToList();

			return Response.Success(orders);
		}

		[Authorize]
		public IActionResult GetMyOrders(string username)
		{
			if (username is null)
			{
				return Response.Error("Username is required.", ErrorCode.OrderUsernameMissing, 400);
			}

			var orders = orderService.GetMyOrders(username)
				.Select(OrderMapper.ToDTO)
				.OrderByDescending(x => x.Status == OrderStatus.Pending)
				.ThenByDescending(x => x.Status == OrderStatus.Approved)
				.ThenByDescending(x => x.Status == OrderStatus.Completed).ToList();

			return Response.Success(orders);
		}

		[Authorize]
		[HttpPost]
		public IActionResult CreateOrder(OrderDTO dto)
		{
			try
			{
				var order = orderService.CreateOrder(DateTime.UtcNow, dto.Username);

				foreach (var orderedProductDto in dto?.OrderedProducts!)
				{
					orderService.CreateOrderedProduct(orderedProductDto.ProductId, order.Id, orderedProductDto.OrderedPrice, orderedProductDto.Count);
				}

				return Ok();
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.OrderCreateError, 400);
			}
		}

		[Authorize(Roles = "Administrator,Employee")]
		[HttpPut]
		public IActionResult EditStatus(OrderDTO dto)
		{
			try
			{
				var edited = orderService.EditOrder(dto.Id, dto.Status, dto.Notes);

				if (edited)
				{
					return Ok();
				}

				return Response.Error("Order cannot be edited!", ErrorCode.OrderUpdateError, 400);
			}
			catch (Exception ex)
			{
				return Response.Error(ex.Message, ErrorCode.OrderUpdateError, 400);
			}
		}
	}
}
