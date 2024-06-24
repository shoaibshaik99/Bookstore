using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.OrderModels
{
    public class CancelOrderRequestModel
    {
        public int UserId { get; set; }
        public int OrderId { get; set; }
    }
}
