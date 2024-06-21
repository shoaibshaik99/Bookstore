using ModelLayer.Models.WishListMoodels;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IWishListsService
    {
        public WishListEntity AddItemToWishList(AddWishListItemModel addWishListItemModel);

        public IEnumerable<FetchWishListModel> GetUserWishlistDetails(int userId);

        public IEnumerable<FetchWishListModel> GetAllUsersWishlistDetails();

        public bool RemoveItemFromWishlist(int userId, int bookId);
    }
}
