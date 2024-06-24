using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;
using ModelLayer.Models.BookModels;
using RepositoryLayer.Entities;
using ServiceLayer.Interfaces;
using ServiceLayer.Services;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBooksService _booksService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBooksService booksService, ILogger<BooksController> logger)
        {
            _booksService = booksService;
            _logger = logger;
        }

        [HttpPost("add")]
        public IActionResult AddBook([FromBody] Add_or_Update_BookModel bookModel)
        {
            try
            {
                var addedBook = _booksService.AddBook(bookModel);

                if (addedBook != null)
                {
                    _logger.LogInformation($"Book added successfully: {addedBook.Title}");
                    return Ok(new ResponseModel<BookEntity>
                    {
                        IsSuccess = true,
                        Message = "Book added successfully",
                        Data = addedBook
                    });
                }
                else
                {
                    _logger.LogError("Failed to add book.");
                    return BadRequest(new ResponseModel<string>
                    {
                        IsSuccess = false,
                        Message = "Failed to add book",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding book.");
                return StatusCode(500, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "Internal server error occurred",
                    Data = "Exception details logged"
                });
            }
        }

        [HttpGet("{bookId}")]
        public IActionResult GetBookById(int bookId)
        {
            try
            {
                var book = _booksService.GetBookById(bookId);
                if (book != null)
                {
                    return Ok(new ResponseModel<FetchBookModel>
                    {
                        IsSuccess = true,
                        Message = "Book found.",
                        Data = book
                    });
                }
                return NotFound(new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "Book not found.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving book with ID {bookId}: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        [HttpGet("all-books")]
        public IActionResult GetAllBooks()
        {
            try
            {
                var books = _booksService.GetAllBooks();
                if (books != null && books.Count > 0)
                {
                    return Ok(new ResponseModel<List<FetchBookModel>>
                    {
                        IsSuccess = true,
                        Message = "Books retrieved successfully.",
                        Data = books
                    });
                }
                return NotFound(new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "No books found.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving books: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        [HttpPut("update-book/{bookId}")]
        public IActionResult UpdateBook(int bookId, Add_or_Update_BookModel updateBookModel)
        {
            try
            {
                bool isUpdated = _booksService.UpdateBook(bookId, updateBookModel);

                if (isUpdated)
                {
                    return Ok(new ResponseModel<string>
                    {
                        IsSuccess = true,
                        Message = "Book updated successfully.",
                        Data = null
                    });
                }
                else
                {
                    return NotFound(new ResponseModel<string>
                    {
                        IsSuccess = false,
                        Message = $"Book with ID {bookId} not found.",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating book with ID {bookId}: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }

        // Put api/books/delete/{bookId}
        [HttpPut("delete-book/{bookId}")]
        public IActionResult DeleteBook(int bookId)
        {
            try
            {
                bool deleted = _booksService.DeleteBook(bookId);

            if (deleted)
                {
                    return Ok(new ResponseModel<string>
                    {
                        IsSuccess = true,
                        Message = "Book deleted successfully.",
                        Data = null
                    });
                }
                else
                {
                    return NotFound(new ResponseModel<string>
                    {
                        IsSuccess = false,
                        Message = "Book not found or already deleted.",
                        Data = null
                    });
                }
        }
            catch (Exception ex)
            {
                _logger.LogError($"Error soft deleting book with ID {bookId}: {ex.Message}");
                return StatusCode(500, new ResponseModel<string>
                {
                    IsSuccess = false,
                    Message = "Internal server error.",
                    Data = null
                });
            }
        }
    }
}
