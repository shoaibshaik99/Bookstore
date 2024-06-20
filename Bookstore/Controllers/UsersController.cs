using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.UserModels;
using RepositoryLayer.Entities;
using ServiceLayer.Interfaces;
using NLog;
using Microsoft.AspNetCore.Authorization;
using RepositoryLayer.Interfaces;
using MassTransit;

namespace Bookstore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IUserRepo _userRepo;

        private readonly IBus _bus;

        public UsersController(IUserService userService, ILogger<UsersController> logger, IUserRepo userRepo, IBus bus)
        {
            _userService = userService;
            _logger = logger;
            _userRepo = userRepo;
            _bus = bus;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterUserModel userModel)
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

                //Handling already registered email
                if (_userService.IsRegisteredEmail(userModel.Email))
                {
                    _logger.LogError("User registration failed.");
                    return BadRequest(new ResponseModel<string>
                    {
                        IsSuccess = false,
                        Message = "This email is already registered",
                        Data = "Failed to register user"
                    });
                }

                // Attempt user registration
                UserEntity result = _userService.RegisterUser(userModel);

                // Handle registration result
                if (result != null)
                {
                    _logger.LogInformation("User registered successfully.");
                    return Ok(new ResponseModel<UserEntity>
                    {
                        IsSuccess = true,
                        Message = "User registration successful",
                        Data = result
                    });
                }
                else
                {
                    return BadRequest(new ResponseModel<string>
                    {
                        IsSuccess = false,
                        Message = "User registration failed",
                        Data = "Failed to register user"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user.");

                // Return generic error response
                return StatusCode(500, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "Internal server error occurred",
                    Data = "Exception details logged"
                });
            }
        }

        [HttpGet("{userId}")]
        public IActionResult GetUserById(int userId)
        {
            try
            {
                var user = _userService.GetUserById(userId);
                if (user != null)
                {
                    return Ok(new ResponseModel<FetchUserModel>
                    {
                        IsSuccess = true,
                        Message = "User found.",
                        Data = user
                    });
                }
                return NotFound(new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "User not found.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving user with ID {userId}: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        [HttpGet("email/{email}")]
        public IActionResult GetUserByEmail(string email)
        {
            try
            {
                var user = _userService.GetUserByEmail(email);
                if (user != null)
                {
                    return Ok(new ResponseModel<FetchUserModel>
                    {
                        IsSuccess = true,
                        Message = "User found.",
                        Data = user
                    });
                }
                return NotFound(new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "User not found.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving user with email {email}: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        [HttpGet("all-users")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _userService.GetAllUsers();
                if (users != null && users.Any())
                {
                    return Ok(new ResponseModel<List<FetchUserModel>>
                    {
                        IsSuccess = true,
                        Message = "Users retrieved successfully.",
                        Data = users
                    });
                }
                return NotFound(new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "No users found.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving users: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        [HttpPut("{userId}")]
        //[Authorize]
        public IActionResult UpdateUser(int userId, [FromBody] UpdateUserModel updateUserModel)
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

            var result = _userService.UpdateUser(userId, updateUserModel);

            if (result)
            {
                _logger.LogInformation($"User with ID {userId} updated successfully.");
                return Ok(new ResponseModel<string>
                {
                    IsSuccess = true,
                    Message = "User updated successfully",
                    Data = null
                });
            }
            else
            {
                _logger.LogWarning($"User with ID {userId} not found.");
                    return NotFound(new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "User not found",
                    Data = null
                });
            }
        }

        //[Authorize]
        [HttpDelete("{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            try
            {
                var result = _userService.DeleteUser(userId);

                if (result)
                {
                    _logger.LogInformation($"User with ID {userId} deleted successfully.");
                    return Ok(new ResponseModel<string>
                    {
                        IsSuccess = true,
                        Message = "User deleted successfully",
                        Data = null
                    });
                }
                else
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    return NotFound(new ResponseModel<string>
                    {
                        IsSuccess = false,
                        Message = "User not found",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user.");
                return StatusCode(500, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "Internal server error occurred",
                    Data = "Exception details logged"
                });
            }
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginModel loginModel)
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

            var token = _userRepo.LoginUser(loginModel);
            if (token == null)
            {
                _logger.LogError("Login failed for user: {Email}", loginModel.Email);
                return Unauthorized(new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "Invalid email or password",
                    Data = null
                });
            }

            return Ok(new ResponseModel<string>
            {
                IsSuccess = true,
                Message = "Login successful",
                Data = token
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordRequestModel forgotPasswordRequestModel)
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

            var forgotPasswordModel = _userService.ForgotPassword(forgotPasswordRequestModel.Email);
            if (forgotPasswordModel == null)
            {
                _logger.LogError("Forgot password failed for email: {Email}", forgotPasswordRequestModel.Email);
                return NotFound(new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "User with this email does not exist",
                    Data = null
                });
            }

            // Here, you might send an email with the reset link/token
            SendEmailService send = new SendEmailService();
            send.SendMail(forgotPasswordModel.Email, forgotPasswordModel.Token);

            Uri uri = new Uri("rabbitmq://localhost/ticketQueue");
            var endPoint = await _bus.GetSendEndpoint(uri);

            await endPoint.Send(forgotPasswordModel);

            return Ok(new ResponseModel<string>
            {
                IsSuccess = true,
                Message = "Password reset instructions have been sent",
                Data = forgotPasswordModel.Token
            });
        }

        [HttpPost("reset-password")]
        [Authorize]
        public IActionResult ResetPassword([FromBody] ResetPasswordModel resetPasswordModel)
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

            string email = User.FindFirst("Email").Value;

            var result = _userService.ResetPassword(email, resetPasswordModel);
            

            if (!result)
            {
                _logger.LogError("Password reset failed for email: {Email}", email);
                return BadRequest(new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "Password reset failed",
                    Data = null
                });
            }

            return Ok(new ResponseModel<string>
            {
                IsSuccess = true,
                Message = "Password reset successful",
                Data = null
            });
        }
    }
}
