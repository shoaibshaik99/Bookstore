using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.CartModels;
using ModelLayer.Models;
using RepositoryLayer.Entities;
using ServiceLayer.Interfaces;
using RabbitMQ.Client.Framing.Impl;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartsService _cartService;
        private readonly ILogger<CartsController> _logger;

        public CartsController(ICartsService cartService, ILogger<CartsController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpPost("add-to-cart")]
        public IActionResult AddItemToCart([FromBody] AddCartItemModel addCartItemModel)
        {
            try
            {
                // Validate input model (for example, using data annotations)
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResponseModel<string>
                    {
                        IsSuccess = false,
                        Message = "Validation errors occurred",
                        Data = "Invalid model state"
                    });
                }

                CartEntity cartItem = _cartService.AddItemToCart(addCartItemModel);
                if (cartItem != null)
                {
                    return Ok(new ResponseModel<CartEntity>
                    {
                        IsSuccess = true,
                        Message = "Item added to cart successfully.",
                        Data = cartItem
                    });
                }
                else
                {
                    return BadRequest(new ResponseModel<string>
                    {
                        IsSuccess = false,
                        Message = "Failed to add item to cart.",
                        Data = null
                    });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding item to cart: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet("user-cart/{userId}")]
        public IActionResult GetUserCartDetails(int userId)
        {
            try
            {
                IEnumerable<FetchCartModel> cartDetails = _cartService.GetUserCartDetails(userId);

                if (cartDetails != null)
                {
                    return Ok(new ResponseModel<IEnumerable<FetchCartModel>>
                    {
                        IsSuccess = true,
                        Message = "Cart details fetched successfully.",
                        Data = cartDetails
                    });
                }
                else
                {
                    return NotFound(new ResponseModel<string>
                    {
                        IsSuccess = false,
                        Message = "No items found in the cart for the specified user.",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching cart details for user {userId}: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet("all-carts")]
        public IActionResult GetAllUsersCartDetails()
        {
            try
            {
                IEnumerable<FetchCartModel> cartDetails = _cartService.GetAllUsersCartDetails();

                if (cartDetails != null)
                {
                    return Ok(new ResponseModel<IEnumerable<FetchCartModel>>
                    {
                        IsSuccess = true,
                        Message = "All users' cart details fetched successfully.",
                        Data = cartDetails
                    });
                }
                else
                {
                    return NotFound(new ResponseModel<string>
                    {
                        IsSuccess = false,
                        Message = "No items found in the carts.",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching all users' cart details: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPut("update-cart-item")]
        public IActionResult UpdateCartItem([FromBody] UpdateCartItemModel updateCartItemModel)
        {
            try
            {
                // Validate input model
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResponseModel<string>
                    {
                        IsSuccess = false,
                        Message = "Validation errors occurred",
                        Data = "Invalid model state"
                    });
                }

                CartEntity updatedCartItem = _cartService.UpdateCartItem(updateCartItemModel);

                if (updatedCartItem != null)
                {
                    return Ok(new ResponseModel<CartEntity>
                    {
                        IsSuccess = true,
                        Message = "Cart item updated successfully.",
                        Data = updatedCartItem
                    });
                }
                else
                {
                    return BadRequest(new ResponseModel<string>
                    {
                        IsSuccess = false,
                        Message = "Failed to update cart item.",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating cart item: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        // DELETE api/carts/remove-cart-item/{userId}/{cartItemId}
        [HttpDelete("remove-cart-item/{userId}/{cartItemId}")]
        public IActionResult RemoveCartItem(int userId, int cartItemId)
        {
            try
            {
                bool isDeleted = _cartService.RemoveCartItem(userId, cartItemId);
                if (isDeleted)
                {
                    return Ok(new ResponseModel<string>
                    {
                        IsSuccess = true,

                        Message = "Cart item removed successfully.",
                        Data = null
                    });
                }
                else
                {
                    return BadRequest(new ResponseModel<string>
                    {
                        IsSuccess = false,
                        Message = "Failed to remove cart item. Cart item not found or user does not own the cart item.",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing cart item: {ex.Message}");
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
