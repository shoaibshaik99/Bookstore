using ModelLayer.Models.CartModels;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface ICartsService
    {
        public CartEntity AddItemToCart(AddCartItemModel addCartItemModel);

        public IEnumerable<FetchCartModel> GetUserCartDetails(int userId);

        public IEnumerable<FetchCartModel> GetAllUsersCartDetails();

        public CartEntity UpdateCartItem(UpdateCartItemModel updateCartItemModel);

        public bool RemoveCartItem(int userId, int cartItemId);
    }
}
