using ModelLayer.Models.CartModels;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Services;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class CartsService : ICartsService
    {
        private readonly ICartsRepo _cartsRepo;

        public CartsService(ICartsRepo cartsRepo)
        {
            _cartsRepo = cartsRepo;
        }
        public CartEntity AddItemToCart(AddCartItemModel addCartItemModel)
        {
            return _cartsRepo.AddItemToCart(addCartItemModel);
        }

        public IEnumerable<FetchCartModel> GetUserCartDetails(int userId)
        {
            return _cartsRepo.GetUserCartDetails(userId);
        }

        public IEnumerable<FetchCartModel> GetAllUsersCartDetails()
        {
            return _cartsRepo.GetAllUsersCartDetails();
        }

        public CartEntity UpdateCartItem(UpdateCartItemModel updateCartItemModel)
        {
            return _cartsRepo.UpdateCartItem(updateCartItemModel);
        }

        public bool RemoveCartItem(int userId, int cartItemId)
        {
            return _cartsRepo.RemoveCartItem(userId, cartItemId);
        }
    }
}
