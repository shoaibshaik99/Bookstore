using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.BookModels;
using RepositoryLayer.Entities;
using ServiceLayer.Interfaces;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MiscellaneousController : ControllerBase
    {
        private readonly IMiscellaneousService _miscellaneousService;
        private readonly ILogger<MiscellaneousController> _logger;

        public MiscellaneousController(IMiscellaneousService miscellaneousService, ILogger<MiscellaneousController> logger)
        {
            _miscellaneousService = miscellaneousService;
            _logger = logger;
        }

        [HttpGet("GetBookByTitleAndAuthor")]
        public IActionResult GetBookByTitleAndAuthor(string title, string author)
        {
            try
            {
                var book = _miscellaneousService.GetBookByTitleAndAuthor(title, author);
                if (book == null)
                {
                    return NotFound(new { message = "No book found with the specified Author and Title." });
                }
                return Ok(new ResponseModel<BookEntity>
                {
                    IsSuccess = true,
                    Message = "Book details fetched successfully",
                    Data = book
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Fetching book by author and title.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("upsert-book")]
        public IActionResult UpsertBook(UpsertBookModel bookModel)
        {
            try
            {
                var newBook = _miscellaneousService.UpsertBook(bookModel);
                if (newBook != null)
                {
                    return Ok(newBook);
                }
                else
                {
                    return BadRequest("Failed to upsert book.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("wishlist-details")]
        public IActionResult GetWishlistDetails()
        {
            try
            {
                var wishlistDetails = _miscellaneousService.GetWishlistDetails();
                return Ok(wishlistDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
