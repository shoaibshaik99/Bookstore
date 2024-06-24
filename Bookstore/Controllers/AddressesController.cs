using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.AddressModels;
using ModelLayer.Models;
using ServiceLayer.Interfaces;
using ServiceLayer.Services;
using RepositoryLayer.Entities;

namespace Bookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressesService _addressesService;
        private readonly ILogger<AddressesController> _logger;

        public AddressesController(IAddressesService addressesService, ILogger<AddressesController> logger)
        {
            _addressesService = addressesService;
            _logger = logger;
        }

        [HttpPost]
        [Route("add")]
        public IActionResult AddAddress([FromBody] AddAddressModel address)
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

                var addressId = _addressesService.InsertAddress(address);

                return Ok(new ResponseModel<int>
                {
                    IsSuccess = true,
                    Message = "Address added successfully.",
                    Data = addressId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding address: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet]
        [Route("user/{userId}")]
        public IActionResult GetAddressesByUserId(int userId)
        {
            try
            {
                var addresses = _addressesService.GetAddressesByUserId(userId);

                return Ok(new ResponseModel<List<AddressEntity>>
                {
                    IsSuccess = true,
                    Message = "Addresses retrieved successfully.",
                    Data = addresses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving addresses: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPut]
        [Route("update")]
        public IActionResult UpdateAddress([FromBody] UpdateAddressModel address)
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

                _addressesService.UpdateAddress(address);

                return Ok(new ResponseModel<string>
                {
                    IsSuccess = true,
                    Message = "Address updated successfully.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating address: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPost]
        [Route("delete/{addressId}")]
        public IActionResult DeleteAddress(int addressId)
        {
            try
            {
                _addressesService.DeleteAddress(addressId);

                return Ok(new ResponseModel<string>
                {
                    IsSuccess = true,
                    Message = "Address deleted successfully.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting address: {ex.Message}");
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
