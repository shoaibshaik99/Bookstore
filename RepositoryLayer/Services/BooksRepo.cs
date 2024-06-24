using Microsoft.Extensions.Configuration;
using ModelLayer.Models.BookModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.BookModels;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Entities;

namespace RepositoryLayer.Services
{
    public class BooksRepo : IBooksRepo
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public BooksRepo(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("BookstoreDBConnection");
        }

        public BookEntity AddBook(Add_or_Update_BookModel bookModel)
        {
            BookEntity newBook = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand addCommand = new SqlCommand("usp_add_book", connection))
                {
                    addCommand.CommandType = CommandType.StoredProcedure;
                    addCommand.Parameters.AddWithValue("@Title", bookModel.Title);
                    addCommand.Parameters.AddWithValue("@Author", bookModel.Author);
                    addCommand.Parameters.AddWithValue("@Description", bookModel.Description);
                    addCommand.Parameters.AddWithValue("@OriginalPrice", bookModel.OriginalPrice);
                    addCommand.Parameters.AddWithValue("@DiscountPercentage", bookModel.DiscountPercentage);
                    addCommand.Parameters.AddWithValue("@Quantity", bookModel.Quantity);
                    addCommand.Parameters.AddWithValue("@Image", bookModel.Image);
                    using (SqlDataReader reader = addCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            newBook = new BookEntity
                            {
                                BookId = Convert.ToInt32(reader["BookId"]),
                                Title = bookModel.Title,
                                Author = bookModel.Author,
                                Description = bookModel.Description,
                                OriginalPrice = bookModel.OriginalPrice,
                                DiscountPercentage = bookModel.DiscountPercentage,
                                Quantity = bookModel.Quantity,
                                Image = bookModel.Image
                            };
                        }
                    }
                }
            }
            return newBook;
        }

        public FetchBookModel GetBookById(int bookId)
        {
            FetchBookModel book = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand getBookCommand = new SqlCommand("usp_get_book_by_id", connection))
                {
                    getBookCommand.CommandType = CommandType.StoredProcedure;
                    getBookCommand.Parameters.AddWithValue("@bookId", bookId);

                    using (SqlDataReader reader = getBookCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            book = new FetchBookModel
                            {
                                BookId = Convert.ToInt32(reader["BookId"]),
                                Title = reader["Title"].ToString(),
                                Author = reader["Author"].ToString(),
                                Description = reader["Description"].ToString(),
                                Rating = Convert.ToDecimal(reader["Rating"]),
                                RatingCount = Convert.ToInt32(reader["RatingCount"]),
                                OriginalPrice = Convert.ToInt32(reader["OriginalPrice"]),
                                DiscountPercentage = Convert.ToInt32(reader["DiscountPercentage"]),
                                Price = Convert.ToInt32(reader["Price"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                Image = reader["Image"].ToString(),
                            };
                        }
                    }
                }
            }
            return book;
        }

        public List<FetchBookModel> GetAllBooks()
        {
            List<FetchBookModel> books = new List<FetchBookModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand getAllBooksCommand = new SqlCommand("usp_get_all_books", connection))
                {
                    getAllBooksCommand.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = getAllBooksCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var book = new FetchBookModel
                            {
                                BookId = Convert.ToInt32(reader["BookId"]),
                                Title = reader["Title"].ToString(),
                                Author = reader["Author"].ToString(),
                                Description = reader["Description"].ToString(),
                                Rating = Convert.ToDecimal(reader["Rating"]),
                                RatingCount = Convert.ToInt32(reader["RatingCount"]),
                                OriginalPrice = Convert.ToInt32(reader["OriginalPrice"]),
                                DiscountPercentage = Convert.ToInt32(reader["DiscountPercentage"]),
                                Price = Convert.ToInt32(reader["Price"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                Image = reader["Image"].ToString()
                            };
                            books.Add(book);
                        }
                    }
                }
            }

            return books;
        }

        public bool UpdateBook(int bookId, Add_or_Update_BookModel bookModel)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("usp_update_book", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@BookId", bookId);
                command.Parameters.AddWithValue("@Title", bookModel.Title);
                command.Parameters.AddWithValue("@Author", bookModel.Author);
                command.Parameters.AddWithValue("@Description", bookModel.Description);
                command.Parameters.AddWithValue("@OriginalPrice", bookModel.OriginalPrice);
                command.Parameters.AddWithValue("@DiscountPercentage", bookModel.DiscountPercentage);
                command.Parameters.AddWithValue("@Quantity", bookModel.Quantity);
                command.Parameters.AddWithValue("@Image", bookModel.Image);

                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

        public bool DeleteBook(int bookId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("usp_delete_book", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@BookId", bookId);

                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0; // Return true if rows were affected (book soft deleted), otherwise false
                }
            }
        }

    }
}
