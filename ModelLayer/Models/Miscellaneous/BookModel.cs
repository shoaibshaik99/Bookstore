using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.Miscelleaneous
{
    public class BookModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Image { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal FinalPrice { get; set; }
    }

}
