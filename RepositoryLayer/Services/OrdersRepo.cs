using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Interfaces;
using ModelLayer.Models.OrderModels;

namespace RepositoryLayer.Services
{
    public class OrdersRepo : IOrdersRepo
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public OrdersRepo(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("BookstoreDBConnection");
        }

        public List<OrderDetailsModel> PlaceOrder(int userId, bool isDirectOrder, int? bookId = null, int? quantity = null)
        {
            var orderDetails = new List<OrderDetailsModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("usp_place_order", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@IsDirectOrder", isDirectOrder);

                    if (isDirectOrder)
                    {
                        if (bookId.HasValue && quantity.HasValue)
                        {
                            command.Parameters.AddWithValue("@BookId", bookId.Value);
                            command.Parameters.AddWithValue("@Quantity", quantity.Value);
                        }
                        else
                        {
                            throw new ArgumentException("BookId and Quantity are required for direct orders.");
                        }
                    }

                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var orderDetail = new OrderDetailsModel
                                {
                                    OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                    TotalOriginalBookPrice = reader.GetInt32(reader.GetOrdinal("TotalOriginalBookPrice")),
                                    TotalFinalBookPrice = reader.GetInt32(reader.GetOrdinal("TotalFinalBookPrice")),
                                    OrderDateTime = reader.GetDateTime(reader.GetOrdinal("OrderDateTime"))
                                };
                                orderDetails.Add(orderDetail);
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
                        throw new Exception("Error placing order: " + ex.Message, ex);
                    }
                }
            }
            return orderDetails;
        }

        public List<FetchOrderDetailsModel> GetAllOrders()
        {
            var orders = new List<FetchOrderDetailsModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("usp_get_all_orders", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var order = new FetchOrderDetailsModel
                                {
                                    OrderId = Convert.ToInt32(reader["OrderId"]),
                                    UserId = Convert.ToInt32(reader["UserId"]),
                                    UserName = reader["UserName"].ToString(),
                                    BookId = Convert.ToInt32(reader["BookId"]),
                                    BookTitle = reader["BookTitle"].ToString(),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    TotalOriginalBookPrice = Convert.ToInt32(reader["TotalOriginalBookPrice"]),
                                    TotalFinalBookPrice = Convert.ToInt32(reader["TotalFinalBookPrice"]),
                                    OrderDateTime = Convert.ToDateTime(reader["OrderDateTime"]),
                                    IsDeleted = Convert.ToBoolean(reader["IsDeleted"]),
                                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                    UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                                };
                                orders.Add(order);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception($"SQL Error: {ex.Message}", ex);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error fetching all orders: " + ex.Message, ex);
                    }
                }
            }

            return orders;
        }

        public List<FetchOrderDetailsModel> GetOrdersByUser(int userId)
        {
            var orders = new List<FetchOrderDetailsModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("usp_get_orders_by_user", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);

                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var order = new FetchOrderDetailsModel
                                {
                                    OrderId = Convert.ToInt32(reader["OrderId"]),
                                    UserId = Convert.ToInt32(reader["UserId"]),
                                    UserName = reader["UserName"].ToString(),
                                    BookId = Convert.ToInt32(reader["BookId"]),
                                    BookTitle = reader["BookTitle"].ToString(),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    TotalOriginalBookPrice = Convert.ToInt32(reader["TotalOriginalBookPrice"]),
                                    TotalFinalBookPrice = Convert.ToInt32(reader["TotalFinalBookPrice"]),
                                    OrderDateTime = Convert.ToDateTime(reader["OrderDateTime"]),
                                    IsDeleted = Convert.ToBoolean(reader["IsDeleted"]),
                                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                    UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                                };
                                orders.Add(order);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception($"SQL Error: {ex.Message}", ex);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error fetching orders by user: " + ex.Message, ex);
                    }
                }
            }

            return orders;
        }

        public CancelledOrderDetailsModel CancelOrder(int userId, int orderId)
        {
            CancelledOrderDetailsModel canceledOrder = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("usp_cancel_order", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@OrderId", orderId);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                canceledOrder = new CancelledOrderDetailsModel
                                {
                                    OrderId = Convert.ToInt32(reader["OrderId"]),
                                    UserId = Convert.ToInt32(reader["UserId"]),
                                    BookId = Convert.ToInt32(reader["BookId"]),
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    TotalOriginalBookPrice = Convert.ToInt32(reader["TotalOriginalBookPrice"]),
                                    TotalFinalBookPrice = Convert.ToInt32(reader["TotalFinalBookPrice"]),
                                    OrderDateTime = Convert.ToDateTime(reader["OrderDateTime"]),
                                    IsDeleted = Convert.ToBoolean(reader["IsDeleted"]),
                                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                    UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
                                };
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception($"SQL Error: {ex.Message}", ex);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error canceling order: {ex.Message}", ex);
                    }
                }
            }

            return canceledOrder;
        }
    }
}
