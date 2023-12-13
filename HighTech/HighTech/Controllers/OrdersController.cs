﻿using HighTech.Abstraction;
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
                    Status = x.Status.ToString(),
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

                }).OrderByDescending(x => x.Status == OrderStatus.Pending.ToString())
                .ThenByDescending(x => x.Status == OrderStatus.Approved.ToString())
                .ThenByDescending(x => x.Status == OrderStatus.Completed.ToString()).ToList();

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
                    Status = x.Status.ToString(),
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

                }).OrderByDescending(x => x.Status == OrderStatus.Pending.ToString())
                .ThenByDescending(x => x.Status == OrderStatus.Approved.ToString())
                .ThenByDescending(x => x.Status == OrderStatus.Completed.ToString()).ToList();

            return Json(orders);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateOrder(OrderDTO dto)
        {
            var order = orderService.CreateOrder(new DateTime(int.Parse(dto.OrderedOn)), dto.User.UserId);

            foreach (var orderedProductDto in dto.OrderedProducts)
            {
                orderService.CreateOrderedProduct(orderedProductDto.ProductId, order.Id, orderedProductDto.OrderedPrice, orderedProductDto.Count);
            }

            return Ok();
        }
    }
}
