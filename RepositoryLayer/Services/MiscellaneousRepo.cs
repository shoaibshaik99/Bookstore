using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;
using ModelLayer.Models.BookModels;
using ModelLayer.Models.Miscelleaneous;

namespace RepositoryLayer.Services
{
    public class MiscellaneousRepo : IMiscellaneousRepo
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public MiscellaneousRepo(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("BookstoreDBConnection");
        }

        public BookEntity GetBookByTitleAndAuthor(string title, string author)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("usp_get_book_by_Title_and_Author", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@Author", author);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new BookEntity
                                {
                                    BookId = (int)reader["BookId"],
                                    Title = reader["Title"].ToString(),
                                    Author = reader["Author"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    Rating = (decimal)reader["Rating"],
                                    RatingCount = (int)reader["RatingCount"],
                                    OriginalPrice = (int)reader["OriginalPrice"],
                                    DiscountPercentage = (int)reader["DiscountPercentage"],
                                    Price = (int)reader["Price"],
                                    Quantity = (int)reader["Quantity"],
                                    Image = reader["Image"].ToString()
                                };
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        // Handle SQL exception
                        throw new Exception($"SQL Error: {ex.Message}", ex);
                    }
                    catch (Exception ex)
                    {
                        // Handle general exception
                        throw new Exception($"Error: {ex.Message}", ex);
                    }
                }
            }
        }

        public BookEntity UpsertBook(UpsertBookModel bookModel)
        {
            BookEntity newBook = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand upsertCommand = new SqlCommand("usp_upsert_book", connection))
                {
                    upsertCommand.CommandType = CommandType.StoredProcedure;
                    upsertCommand.Parameters.AddWithValue("@BookId", bookModel.BookId);
                    upsertCommand.Parameters.AddWithValue("@Title", bookModel.Title);
                    upsertCommand.Parameters.AddWithValue("@Author", bookModel.Author);
                    upsertCommand.Parameters.AddWithValue("@Description", bookModel.Description);
                    upsertCommand.Parameters.AddWithValue("@OriginalPrice", bookModel.OriginalPrice);
                    upsertCommand.Parameters.AddWithValue("@DiscountPercentage", bookModel.DiscountPercentage);
                    upsertCommand.Parameters.AddWithValue("@Quantity", bookModel.Quantity);
                    upsertCommand.Parameters.AddWithValue("@Image", bookModel.Image);

                    try
                    {
                        using (SqlDataReader reader = upsertCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                newBook = new BookEntity
                                {
                                    BookId = Convert.ToInt32(reader["BookId"]),
                                    Title = bookModel.Title,
                                    Author = bookModel.Author,
                                    Description = bookModel.Description,
                                    Rating = Convert.ToInt32(reader["Rating"]),
                                    RatingCount = Convert.ToInt32(reader["RatingCount"]),
                                    OriginalPrice = bookModel.OriginalPrice,
                                    DiscountPercentage = bookModel.DiscountPercentage,
                                    Price = Convert.ToInt32(reader["Price"]),
                                    Quantity = bookModel.Quantity,
                                    Image = bookModel.Image
                                };
                            }
                        }

                    }
                    catch (SqlException ex)
                    {
                        // Handle SQL exception
                        throw new Exception($"SQL Error: {ex.Message}", ex);
                    }
                    catch (Exception ex)
                    {
                        // Handle general exception
                        throw new Exception("Error upserting book: " + ex.Message, ex);
                    }
                }
            }
            return newBook;            
        }

        public List<WishlistDetailModel> GetWishlistDetails()
        {
            List<WishlistDetailModel> wishlistDetails = new List<WishlistDetailModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("usp_get_wishlist_details", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var user = new UserModel
                                {
                                    UserId = Convert.ToInt32(reader["UserId"]),
                                    FullName = reader["UserName"].ToString()
                                };

                                var book = new BookModel
                                {
                                    BookId = Convert.ToInt32(reader["BookId"]),
                                    Title = reader["BookTitle"].ToString(),
                                    Author = reader["BookAuthor"].ToString(),
                                    Image = reader["BookImage"].ToString(),
                                    OriginalPrice = Convert.ToDecimal(reader["OriginalPrice"]),
                                    FinalPrice = Convert.ToDecimal(reader["FinalPrice"])
                                };

                                var wishlistDetail = new WishlistDetailModel
                                {
                                    WishListId = Convert.ToInt32(reader["wishListId"]),
                                    User = user,
                                    Book = book,
                                    CreatedAt = Convert.ToDateTime(reader["createdAt"]),
                                    UpdatedAt = Convert.ToDateTime(reader["updatedAt"])
                                };

                                wishlistDetails.Add(wishlistDetail);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        // Handle SQL exception
                        throw new Exception($"SQL Error: {ex.Message}", ex);
                    }
                    catch (Exception ex)
                    {
                        // Handle general exception
                        throw new Exception($"Error fetching wishlist details: {ex.Message}", ex);
                    }
                }
            }

            return wishlistDetails;
        }

    }
}
