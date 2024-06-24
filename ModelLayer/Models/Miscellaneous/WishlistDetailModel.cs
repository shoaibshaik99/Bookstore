using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.Miscelleaneous
{
    public class WishlistDetailModel
    {
        public int WishListId { get; set; }
        public UserModel User { get; set; }
        public BookModel Book { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
