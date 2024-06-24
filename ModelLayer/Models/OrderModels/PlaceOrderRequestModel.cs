using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.OrderModels
{
    public class PlaceOrderRequest
    {
        public int UserId { get; set; }
        public bool IsDirectOrder { get; set; }
        public int? BookId { get; set; }
        public int? Quantity { get; set; }
    }
}
