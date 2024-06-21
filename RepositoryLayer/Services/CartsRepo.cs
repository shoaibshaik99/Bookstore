using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ModelLayer.Models.CartModels;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;

namespace ServiceLayer.Services
{
    //bool RemoveBookFromCart(int cartId);

    public class CartsRepo : ICartsRepo
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public CartsRepo(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("BookstoreDBConnection");
        }

        public CartEntity AddItemToCart(AddCartItemModel addCartItemModel)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("usp_add_item_to_cart", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@userId", addCartItemModel.UserId);
                    command.Parameters.AddWithValue("@bookId", addCartItemModel.BookId);
                    command.Parameters.AddWithValue("@quantity", addCartItemModel.Quantity);

                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new CartEntity
                                {
                                    CartItemId = Convert.ToInt32(reader["cartItemId"]),
                                    UserId = addCartItemModel.UserId,
                                    BookId = addCartItemModel.BookId,
                                    Quantity = addCartItemModel.Quantity,
                                    UnitPrice = Convert.ToInt32(reader["unitPrice"])
                                };
                            }
                            else
                            {
                                throw new Exception("Failed to add item to cart.");
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

        public IEnumerable<FetchCartModel> GetUserCartDetails(int userId)
        {
            var cartDetails = new List<FetchCartModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("usp_get_user_cart_details", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@userId", userId);

                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var cartItem = new FetchCartModel
                                {
                                    CartItemId = Convert.ToInt32(reader["cartItemId"]),
                                    UserId = Convert.ToInt32(reader["userId"]),
                                    BookId = Convert.ToInt32(reader["bookId"]),
                                    Quantity = Convert.ToInt32(reader["quantity"]),
                                    BookPrice = Convert.ToInt32(reader["bookPrice"]),
                                    BookTitle = reader["bookTitle"].ToString(),
                                    BookAuthor = reader["bookAuthor"].ToString(),
                                    AddedAt = Convert.ToDateTime(reader["createdAt"]),
                                    ModifiedAt = Convert.ToDateTime(reader["updatedAt"])
                                };
                                cartDetails.Add(cartItem);
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

            if (cartDetails.Count == 0)
            {
                throw new Exception("No items found in the cart for the specified user.");
            }

            return cartDetails;
        }

        public IEnumerable<FetchCartModel> GetAllUsersCartDetails()
        {
            var cartDetails = new List<FetchCartModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("usp_get_all_users_cart_details", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var cartItem = new FetchCartModel
                                {
                                    CartItemId = Convert.ToInt32(reader["cartItemId"]),
                                    UserId = Convert.ToInt32(reader["userId"]),
                                    BookId = Convert.ToInt32(reader["bookId"]),
                                    Quantity = Convert.ToInt32(reader["quantity"]),
                                    BookPrice = Convert.ToInt32(reader["bookPrice"]),
                                    BookTitle = reader["bookTitle"].ToString(),
                                    BookAuthor = reader["bookAuthor"].ToString(),
                                    AddedAt = Convert.ToDateTime(reader["createdAt"]),
                                    ModifiedAt = Convert.ToDateTime(reader["updatedAt"])
                                };
                                cartDetails.Add(cartItem);
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

            if (cartDetails.Count == 0)
            {
                throw new Exception("No items found in the carts.");
            }

            return cartDetails;
        }

        public CartEntity UpdateCartItem(UpdateCartItemModel updateCartItemModel)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("usp_update_cart_item", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@userId", updateCartItemModel.UserId);
                    command.Parameters.AddWithValue("@cartItemId", updateCartItemModel.CartItemId);
                    command.Parameters.AddWithValue("@newQuantity", updateCartItemModel.Quantity);

                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new CartEntity
                                {
                                    CartItemId = Convert.ToInt32(reader["cartItemId"]),
                                    UserId = Convert.ToInt32(reader["userId"]),
                                    BookId = Convert.ToInt32(reader["bookId"]),
                                    Quantity = Convert.ToInt32(reader["quantity"]),
                                    UnitPrice = Convert.ToInt32(reader["bookPrice"])
                                };
                            }
                            else
                            {
                                throw new Exception("Failed to update cart item.");
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

        public bool RemoveCartItem(int userId, int cartItemId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("usp_remove_cart_item", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@cartItemId", cartItemId);

                    try
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
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


    }
}
