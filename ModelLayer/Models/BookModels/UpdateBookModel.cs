using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.BookModels
{
    public class UpdateBookModel
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public int OriginalPrice { get; set; }
        public int DiscountPercentage { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; }
    }
}
