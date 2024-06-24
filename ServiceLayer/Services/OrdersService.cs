using ModelLayer.Models.OrderModels;
using RepositoryLayer.Interfaces;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly IOrdersRepo _ordersRepo;

        public OrdersService(IOrdersRepo ordersRepo)
        {
            _ordersRepo = ordersRepo;
        }

        public List<OrderDetailsModel> PlaceOrder(int userId, bool isDirectOrder, int? bookId = null, int? quantity = null)
        {
            return _ordersRepo.PlaceOrder(userId, isDirectOrder, bookId, quantity);
        }

        public List<FetchOrderDetailsModel> GetAllOrders()
        {
            return _ordersRepo.GetAllOrders();
        }

        public List<FetchOrderDetailsModel> GetOrdersByUser(int userId)
        {
            return _ordersRepo.GetOrdersByUser(userId);
        }

        public CancelledOrderDetailsModel CancelOrder(int userId, int orderId)
        {
            return _ordersRepo.CancelOrder(userId, orderId);
        }
    }
}
