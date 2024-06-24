using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.OrderModels;
using ServiceLayer.Interfaces;
using ServiceLayer.Services;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;
        private readonly ILogger<CartsController> _logger;

        public OrdersController(IOrdersService ordersService, ILogger<CartsController> logger)
        {
            _ordersService = ordersService;
            _logger = logger;
        }

        [HttpPost]
        [Route("placeOrder")]
        public IActionResult PlaceOrder([FromBody] PlaceOrderRequest request)
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

                var orderDetails = _ordersService.PlaceOrder(request.UserId, request.IsDirectOrder, request.BookId, request.Quantity);

                return Ok(new ResponseModel<List<OrderDetailsModel>>
                {
                    IsSuccess = true,
                    Message = "Order placed successfully. Following are the order details.",
                    Data = orderDetails
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error placing order: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });

            }
        }

        [HttpGet]
        [Route("getAllOrders")]
        public IActionResult GetAllOrders()
        {
            try
            {
                var orders = _ordersService.GetAllOrders();

                return Ok(new ResponseModel<List<FetchOrderDetailsModel>>
                {
                    IsSuccess = true,
                    Message = "Orders retrieved successfully",
                    Data = orders
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching all orders: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet]
        [Route("getOrdersByUser/{userId}")]
        public IActionResult GetOrdersByUser(int userId)
        {
            try
            {
                var orders = _ordersService.GetOrdersByUser(userId);

                return Ok(new ResponseModel<List<FetchOrderDetailsModel>>
                {
                    IsSuccess = true,
                    Message = "Orders retrieved successfully",
                    Data = orders
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching orders by user: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPost]
        [Route("cancelOrder")]
        public IActionResult CancelOrder([FromBody] CancelOrderRequestModel request)
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

                var canceledOrder = _ordersService.CancelOrder(request.UserId, request.OrderId);

                return Ok(new ResponseModel<CancelledOrderDetailsModel>
                {
                    IsSuccess = true,
                    Message = "Order canceled successfully.",
                    Data = canceledOrder
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error canceling order: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
