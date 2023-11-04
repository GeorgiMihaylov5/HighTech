﻿using HighTech.Abstraction;
using HighTech.Data;
using HighTech.Models;
using HighTech.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace HighTech.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext context;

        public OrderService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public bool CreateOrder(DateTime orderedOn, string customerId)
        {
            context.Orders.Add(new Order()
            {
                OrderedOn = orderedOn,
                CustomerId = customerId,
                Status = OrderStatus.Pending,
            });

            return context.SaveChanges() != 0;
        }

        public bool CreateOrderedProduct(string productId, string orderId, decimal price, int count)
        {
            context.OrderedProducts.Add(new OrderedProduct()
            {
                ProductId = productId,
                OrderId = orderId,
                OrderedPrice = price,
                Count = count
            });

            return context.SaveChanges() != 0;
        }

        public bool EditOrder(string id, OrderStatus status, string notes)
        {
            var order = context.Orders.FirstOrDefault(o => o.Id == id);

            if (order is null)
            {
                return false;
            }

            order.Status = status;
            order.Notes = notes;

            return context.SaveChanges() != 0;
        }

        public bool EditOrderedProduct(string id, int count)
        {
            var orderedProduct = context.OrderedProducts.FirstOrDefault(o => o.Id == id);

            if (orderedProduct is null)
            {
                return false;
            }

            orderedProduct.Count = count;

            return context.SaveChanges() != 0;
        }

        public Order GetOrder(string id)
        {
            return context.Orders
                .Include(o => o.OrderedProducts)
                .FirstOrDefault(o => o.Id == id);
        }

        public ICollection<Order> GetOrders()
        {
            return context.Orders
                .Include(o => o.OrderedProducts)
                .ToList();
        }
    }
}