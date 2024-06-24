using ModelLayer.Models.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interfaces
{
    public interface IOrdersRepo
    {
        public List<OrderDetailsModel> PlaceOrder(int userId, bool isDirectOrder, int? bookId = null, int? quantity = null);

        public List<FetchOrderDetailsModel> GetAllOrders();

        public List<FetchOrderDetailsModel> GetOrdersByUser(int userId);

        public CancelledOrderDetailsModel CancelOrder(int userId, int orderId);
    }
}
