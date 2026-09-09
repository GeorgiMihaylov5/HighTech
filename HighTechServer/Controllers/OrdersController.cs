using HighTech.Abstraction;
using HighTech.Common;
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
                    City = x.City,
                    PostalCode = x.PostalCode,
                    DeliveryAddress = x.DeliveryAddress,
                    PhoneNumber = x.PhoneNumber,
                    PaymentMethod = x.PaymentMethod,
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
                .ThenByDescending(x => x.Status == OrderStatus.Confirmed)
                .ThenByDescending(x => x.Status == OrderStatus.Preparing)
                .ThenByDescending(x => x.Status == OrderStatus.Shipped)
                .ThenByDescending(x => x.Status == OrderStatus.Completed).ToList();

            return Response.ResultSuccess(orders);
        }

        [Authorize]
        public IActionResult GetMyOrders(string username)
        {
            if (username is null)
            {
                return Response.ResultFailure<List<OrderDTO>>("Username cannot be null.", 400);
            }

            var orders = orderService.GetMyOrders(username)
                .Select(x => new OrderDTO
                {
                    Id = x.Id,
                    OrderedOn = x.OrderedOn.Ticks.ToString(),
                    Status = x.Status,
                    Notes = x.Notes,
                    City = x.City,
                    PostalCode = x.PostalCode,
                    DeliveryAddress = x.DeliveryAddress,
                    PhoneNumber = x.PhoneNumber,
                    PaymentMethod = x.PaymentMethod,
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
                .ThenByDescending(x => x.Status == OrderStatus.Confirmed)
                .ThenByDescending(x => x.Status == OrderStatus.Preparing)
                .ThenByDescending(x => x.Status == OrderStatus.Shipped)
                .ThenByDescending(x => x.Status == OrderStatus.Completed)
                .ThenByDescending(x => x.OrderedOn).ToList();

            return Response.ResultSuccess(orders);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateOrder(OrderDTO dto)
        {
            if (dto?.OrderedProducts is null || !dto.OrderedProducts.Any())
            {
                return Response.ResultFailure<bool>("Order must contain at least one product.", 400);
            }

            var validationErrors = ValidateOrder(dto);

            if (validationErrors.Count > 0)
            {
                return Response.ResultFailure<bool>(string.Join(", ", validationErrors), 400);
            }

            try
            {
                var created = orderService.TryCreateOrder(dto);

                if (!created)
                {
                    return Response.ResultFailure<bool>("Order cannot be created. Check delivery details and product quantities.", 400);
                }

                return Response.ResultSuccess(true);
            }
            catch (Exception)
            {
                return Response.ResultFailure<bool>("An unexpected error occurred.", 500);
            }
        }

        [Authorize(Roles = "Administrator,Employee")]
        [HttpPut]
        public IActionResult EditStatus(OrderDTO dto)
        {
            if (dto == null)
            {
                return Response.ResultFailure<bool>("Order cannot be null.", 400);
            }

            if (dto.Notes is not null && dto.Notes.Length > StringLimits.LongText)
            {
                return Response.ResultFailure<bool>($"Notes cannot be longer than {StringLimits.LongText} characters.", 400);
            }

            try
            {
                var edited = orderService.EditOrder(dto.Id, dto.Status, dto.Notes);

                if (edited)
                {
                    return Response.ResultSuccess(true);
                }

                return Response.ResultFailure<bool>("Order cannot be edited!", 400);
            }
            catch (Exception)
            {
                return Response.ResultFailure<bool>("An unexpected error occurred.", 500);
            }
        }

        private static List<string> ValidateOrder(OrderDTO dto)
        {
            var errors = new List<string>();

            StringLimits.AddMaxLengthError(errors, "Username", dto.Username, StringLimits.Email);
            StringLimits.AddMaxLengthError(errors, "City", dto.City, StringLimits.City);
            StringLimits.AddMaxLengthError(errors, "Postal code", dto.PostalCode, StringLimits.PostalCode);
            StringLimits.AddMaxLengthError(errors, "Delivery address", dto.DeliveryAddress, StringLimits.Address);
            StringLimits.AddMaxLengthError(errors, "Phone number", dto.PhoneNumber, StringLimits.Phone);
            StringLimits.AddMaxLengthError(errors, "Notes", dto.Notes, StringLimits.LongText);

            foreach (var orderedProduct in dto.OrderedProducts ?? Enumerable.Empty<OrderedProductDTO>())
            {
                StringLimits.AddMaxLengthError(errors, "Build id", orderedProduct.BuildId, StringLimits.Guid);
            }

            return errors;
        }
    }
}
