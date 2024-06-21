using ModelLayer.Models.WishListMoodels;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace RepositoryLayer.Services
{
    public class WishListsRepo : IWishListsRepo
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public WishListsRepo(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("BookstoreDBConnection");
        }

        public WishListEntity AddItemToWishList(AddWishListItemModel addWishListItemModel)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("usp_add_item_to_wishlist", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@userId", addWishListItemModel.UserId);
                    command.Parameters.AddWithValue("@bookId", addWishListItemModel.BookId);

                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new WishListEntity
                                {
                                    WishListId = Convert.ToInt32(reader["wishListId"]),
                                    UserId = addWishListItemModel.UserId,
                                    BookId = addWishListItemModel.BookId,
                                    IsDeleted = false,
                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now
                                };
                            }
                            else
                            {
                                throw new Exception("Failed to add item to wishlist.");
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

        public IEnumerable<FetchWishListModel> GetUserWishlistDetails(int userId)
        {
            var wishlistDetails = new List<FetchWishListModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("usp_get_user_wishlist_details", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@userId", userId);

                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var wishlistItem = new FetchWishListModel
                                {
                                    WishListId = Convert.ToInt32(reader["wishListId"]),
                                    UserId = Convert.ToInt32(reader["userId"]),
                                    BookId = Convert.ToInt32(reader["bookId"]),
                                    BookTitle = reader["bookTitle"].ToString(),
                                    BookAuthor = reader["bookAuthor"].ToString(),
                                    BookPrice = Convert.ToDecimal(reader["bookPrice"]),
                                    CreatedAt = Convert.ToDateTime(reader["createdAt"]),
                                    UpdatedAt = Convert.ToDateTime(reader["updatedAt"])
                                };
                                wishlistDetails.Add(wishlistItem);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception($"SQL Error: {ex.Message}", ex);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error: {ex.Message}", ex);
                    }
                }
            }

            if (wishlistDetails.Count == 0)
            {
                throw new Exception("No items found in the wishlist for the specified user.");
            }

            return wishlistDetails;
        }

        public IEnumerable<FetchWishListModel> GetAllUsersWishlistDetails()
        {
            var wishlistDetails = new List<FetchWishListModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("usp_get_all_users_wishlist_details", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var wishlistItem = new FetchWishListModel
                                {
                                    WishListId = Convert.ToInt32(reader["wishListId"]),
                                    UserId = Convert.ToInt32(reader["userId"]),
                                    BookId = Convert.ToInt32(reader["bookId"]),
                                    BookTitle = reader["bookTitle"].ToString(),
                                    BookAuthor = reader["bookAuthor"].ToString(),
                                    BookPrice = Convert.ToDecimal(reader["bookPrice"]),
                                    CreatedAt = Convert.ToDateTime(reader["createdAt"]),
                                    UpdatedAt = Convert.ToDateTime(reader["updatedAt"])
                                };
                                wishlistDetails.Add(wishlistItem);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception($"SQL Error: {ex.Message}", ex);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error: {ex.Message}", ex);
                    }
                }
            }

            return wishlistDetails;
        }

        public bool RemoveItemFromWishlist(int userId, int bookId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("usp_remove_item_from_wishlist", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@bookId", bookId);

                    try
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception($"SQL Error: {ex.Message}", ex);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error: {ex.Message}", ex);
                    }
                }
            }
        }
    }
}
