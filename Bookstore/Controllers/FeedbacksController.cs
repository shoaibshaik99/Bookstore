using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models.FeedbackModels;
using ModelLayer.Models;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;
using ServiceLayer.Interfaces;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private IFeedbacksService _feedbacksService;
        private readonly ILogger<FeedbacksController> _logger;

        public FeedbacksController(IFeedbacksService feedbacksService, ILogger<FeedbacksController> logger)
        {
            _feedbacksService = feedbacksService;
            _logger = logger;
        }

        [HttpPost]
        [Route("giveFeedback")]
        public IActionResult GiveFeedback(int userId, [FromBody] GiveFeedbackModel feedbackModel)
        {
            try
            {
                var result = _feedbacksService.GiveFeedback(userId, feedbackModel);

                return Ok(new ResponseModel<FeedbackEntity>
                {
                    IsSuccess = true,
                    Message = "Feedback given successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error giving feedback: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet]
        [Route("viewAllFeedbacks")]
        public IActionResult ViewAllFeedbacks()
        {
            try
            {
                var result = _feedbacksService.ViewAllFeedbacks();

                return Ok(new ResponseModel<List<FeedbackEntity>>
                {
                    IsSuccess = true,
                    Message = "Feedbacks retrieved successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving feedbacks: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet]
        [Route("viewFeedbacksForBook/{bookId}")]
        public IActionResult ViewFeedbacksForBook(int bookId)
        {
            try
            {
                var result = _feedbacksService.ViewAllFeedbacksForBook(bookId);

                return Ok(new ResponseModel<List<FeedbackEntity>>
                {
                    IsSuccess = true,
                    Message = "Feedbacks for the book retrieved successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving feedbacks for book: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPut]
        [Route("editFeedback")]
        public IActionResult EditFeedback(int userId, [FromBody] EditFeedbackModel editFeedbackModel)
        {
            try
            {
                var result = _feedbacksService.EditFeedback(userId, editFeedbackModel);

                return Ok(new ResponseModel<FeedbackEntity>
                {
                    IsSuccess = true,
                    Message = "Feedback edited successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error editing feedback: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpDelete]
        [Route("deleteFeedback/{userId}/{feedbackId}")]
        public IActionResult DeleteFeedback(int userId, int feedbackId)
        {
            try
            {
                var result = _feedbacksService.DeleteFeedback(userId, feedbackId);

                if (result)
                {
                    return Ok(new ResponseModel<string>
                    {
                        IsSuccess = true,
                        Message = "Feedback deleted successfully.",
                        Data = null
                    });
                }
                else
                {
                    return NotFound(new ResponseModel<string>
                    {
                        IsSuccess = false,
                        Message = "Feedback not found.",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting feedback: {ex.Message}");
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
