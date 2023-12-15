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
        public IActionResult GetOrders()
        {
            var orders = orderService.GetOrders()
                .Select(x => new OrderDTO
                {
                    Id = x.Id,
                    OrderedOn = x.OrderedOn.Ticks.ToString(),
                    Status = x.Status,
                    Notes = x.Notes,
                    OrderedProducts = x.OrderedProducts.Select(op => new OrderedProductDTO()
                    {
                        Id = op.Id,
                        Count = op.Count,
                        OrderedPrice = op.OrderedPrice,
                        ProductId = op.ProductId,
                        Product = new ProductDTO()
                        {
                            Id = op.ProductId,
                            Manufacturer = op.Product.Manufacturer,
                            Model = op.Product.Model,
                            Warranty = op.Product.Warranty,
                            Image = op.Product.Image,
                        }
                    }).ToList(),
                    User = new UserDTO() 
                    { 
                        UserId = x.CustomerId,
                        FirstName = x.Customer.FirstName,
                        LastName = x.Customer.LastName,
                        Email = x.Customer.Email,
                        Username = x.Customer.UserName,
                        PhoneNumber = x.Customer.PhoneNumber,
                    }

                }).OrderByDescending(x => x.Status == OrderStatus.Pending)
                .ThenByDescending(x => x.Status == OrderStatus.Approved)
                .ThenByDescending(x => x.Status == OrderStatus.Completed).ToList();

            return Json(orders);
        }

        [Authorize]
        public IActionResult GetMyOrders(string username)
        {
            if (username is null)
            {
                return NotFound();
            }

            var orders = orderService.GetMyOrders(username)
                .Select(x => new OrderDTO
                {
                    Id = x.Id,
                    OrderedOn = x.OrderedOn.Ticks.ToString(),
                    Status = x.Status,
                    Notes = x.Notes,
                    OrderedProducts = x.OrderedProducts.Select(op => new OrderedProductDTO()
                    {
                        Id = op.Id,
                        Count = op.Count,
                        OrderedPrice = op.OrderedPrice,
                        ProductId = op.ProductId,
                        Product = new ProductDTO()
                        {
                            Id = op.ProductId,
                            Manufacturer = op.Product.Manufacturer,
                            Model = op.Product.Model,
                            Warranty = op.Product.Warranty,
                            Image = op.Product.Image,
                        }
                    }).ToList(),
                    User = new UserDTO()
                    {
                        UserId = x.CustomerId,
                        FirstName = x.Customer.FirstName,
                        LastName = x.Customer.LastName,
                        Email = x.Customer.Email,
                        Username = x.Customer.UserName,
                        PhoneNumber = x.Customer.PhoneNumber,
                    }

                }).OrderByDescending(x => x.Status == OrderStatus.Pending)
                .ThenByDescending(x => x.Status == OrderStatus.Approved)
                .ThenByDescending(x => x.Status == OrderStatus.Completed).ToList();

            return Json(orders);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateOrder(OrderDTO dto)
        {
            try
            {
                var order = orderService.CreateOrder(DateTime.Now, dto.Username);

                foreach (var orderedProductDto in dto.OrderedProducts)
                {
                    orderService.CreateOrderedProduct(orderedProductDto.ProductId, order.Id, orderedProductDto.OrderedPrice, orderedProductDto.Count);
                }

                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
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

                return BadRequest("Order cannot be edited!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
