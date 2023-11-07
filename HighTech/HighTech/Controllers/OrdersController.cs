using HighTech.Abstraction;
using HighTech.DTOs;
using HighTech.Models.Enum;
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
        public IActionResult All()
        {
            var orders = orderService.GetOrders()
                .Select(x => new OrderDTO
                {
                    Id = x.Id,
                    OrderedOn = x.OrderedOn.Ticks.ToString(),
                    Status = x.Status.ToString(),
                    Notes = x.Notes,
                    User = new UserDTO() 
                    { 
                        Id = x.CustomerId,
                        FirstName = x.Customer.FirstName,
                        LastName = x.Customer.LastName,
                        Email = x.Customer.Email,
                        Username = x.Customer.UserName,
                        PhoneNumber = x.Customer.PhoneNumber,
                    }

                }).OrderByDescending(x => x.Status == OrderStatus.Pending.ToString())
                .ThenByDescending(x => x.Status == OrderStatus.Approved.ToString())
                .ThenByDescending(x => x.Status == OrderStatus.Completed.ToString()).ToList();

            return Json(orders);
        }

        [Authorize]
        public IActionResult My(string userId)
        {
            if (userId is null)
            {
                return NotFound();
            }

            var orders = orderService.GetMyOrders(userId)
                .Select(x => new OrderDTO
                {
                    Id = x.Id,
                    OrderedOn = x.OrderedOn.Ticks.ToString(),
                    Status = x.Status.ToString(),
                    Notes = x.Notes,
                    User = new UserDTO()
                    {
                        Id = x.CustomerId,
                        FirstName = x.Customer.FirstName,
                        LastName = x.Customer.LastName,
                        Email = x.Customer.Email,
                        Username = x.Customer.UserName,
                        PhoneNumber = x.Customer.PhoneNumber,
                    }

                }).OrderByDescending(x => x.Status == OrderStatus.Pending.ToString())
                .ThenByDescending(x => x.Status == OrderStatus.Approved.ToString())
                .ThenByDescending(x => x.Status == OrderStatus.Completed.ToString()).ToList();

            return Json(orders);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(OrderDTO dto)
        {
            var order = orderService.CreateOrder(new DateTime(int.Parse(dto.OrderedOn)), dto.User.Id);

            foreach (var orderedProductDto in dto.OrderedProducts)
            {
                orderService.CreateOrderedProduct(orderedProductDto.Product.Id, order.Id, orderedProductDto.OrderedPrice, orderedProductDto.Count);
            }

            return Ok();
        }
    }
}
