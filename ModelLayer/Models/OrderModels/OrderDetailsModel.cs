using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.OrderModels
{
    public class OrderDetailsModel
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public int TotalOriginalBookPrice { get; set; }
        public int TotalFinalBookPrice { get; set; }
        public DateTime OrderDateTime { get; set; }
    }
}
