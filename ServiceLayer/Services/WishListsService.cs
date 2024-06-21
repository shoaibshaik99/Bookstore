using ModelLayer.Models.WishListMoodels;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Services;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class WishListsService: IWishListsService
    {
        private readonly IWishListsRepo _wishListsRepo;
            
        public WishListsService(IWishListsRepo wishListsRepo)
        {
            _wishListsRepo = wishListsRepo;
        }

        public WishListEntity AddItemToWishList(AddWishListItemModel addWishListItemModel)
        {
            return _wishListsRepo.AddItemToWishList(addWishListItemModel);
        }

        public IEnumerable<FetchWishListModel> GetUserWishlistDetails(int userId)
        {
            return _wishListsRepo.GetUserWishlistDetails(userId);
        }

        public IEnumerable<FetchWishListModel> GetAllUsersWishlistDetails()
        {
            return _wishListsRepo.GetAllUsersWishlistDetails();
        }

        public bool RemoveItemFromWishlist(int userId, int bookId)
        {
            return _wishListsRepo.RemoveItemFromWishlist(userId, bookId);
        }
    }
}
