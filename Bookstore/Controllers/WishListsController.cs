using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.WishListMoodels;
using ModelLayer.Models;
using RepositoryLayer.Entities;
using ServiceLayer.Interfaces;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishListsController : ControllerBase
    {
        private readonly IWishListsService _wishListsService;
        private readonly ILogger<WishListsController> _logger;

        public WishListsController(IWishListsService wishListsService, ILogger<WishListsController> logger)
        {
            _wishListsService = wishListsService;
            _logger = logger;
        }

        [HttpPost("add-to-wishlist")]
        public IActionResult AddItemToWishList([FromBody] AddWishListItemModel addWishListItemModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResponseModel<string>
                    {
                        IsSuccess = false,
                        Message = "Validation errors occurred",
                        Data = "Invalid model state"
                    });
                }

                WishListEntity wishListItem = _wishListsService.AddItemToWishList(addWishListItemModel);
                if (wishListItem != null)
                {
                    return Ok(new ResponseModel<WishListEntity>
                    {
                        IsSuccess = true,
                        Message = "Item added to wishlist successfully.",
                        Data = wishListItem
                    });
                }
                else
                {
                    return BadRequest(new ResponseModel<string>
                    {
                        IsSuccess = false,
                        Message = "Failed to add item to wishlist.",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding item to wishlist: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetUserWishlistDetails(int userId)
        {
            try
            {
                var wishlist = _wishListsService.GetUserWishlistDetails(userId);
                return Ok(new ResponseModel<IEnumerable<FetchWishListModel>>
                {
                    IsSuccess = true,
                    Message = "User wishlist retrieved successfully.",
                    Data = wishlist
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving user wishlist: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet("all")]
        public IActionResult GetAllUsersWishlistDetails()
        {
            try
            {
                var wishlist = _wishListsService.GetAllUsersWishlistDetails();
                return Ok(new ResponseModel<IEnumerable<FetchWishListModel>>
                {
                    IsSuccess = true,
                    Message = "All users wishlist retrieved successfully.",
                    Data = wishlist
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving all users wishlist: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPut("remove/{userId}/{bookId}")]
        public IActionResult RemoveItemFromWishlist(int userId, int bookId)
        {
            try
            {
                bool success = _wishListsService.RemoveItemFromWishlist(userId, bookId);
                if (success)
                {
                    return Ok(new ResponseModel<string>
                    {
                        IsSuccess = true,
                        Message = "Item removed from wishlist successfully.",
                        Data = null
                    });
                }
                else
                {
                    return StatusCode(500, new ResponseModel<string>
                    {
                        IsSuccess = false,
                        Message = "Failed to remove item from wishlist.",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing item from wishlist: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
