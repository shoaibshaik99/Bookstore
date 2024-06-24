using Microsoft.Extensions.Configuration;
using RepositoryLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models.FeedbackModels;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using RepositoryLayer.Interfaces;
using System.Net;

namespace RepositoryLayer.Services
{
    public class FeedbacksRepo : IFeedbacksRepo
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public FeedbacksRepo(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("BookstoreDBConnection");
        }

        public FeedbackEntity GiveFeedback(int userId, GiveFeedbackModel feedbackModel)
        {
            FeedbackEntity feedback = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("usp_give_feedback", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@BookId", feedbackModel.BookId);
                    command.Parameters.AddWithValue("@Rating", feedbackModel.Rating);
                    command.Parameters.AddWithValue("@Review", feedbackModel.Review);
                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                feedback = new FeedbackEntity()
                                {
                                    FeedbackId = reader.GetInt32(reader.GetOrdinal("FeedbackId")),
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                                    Rating = reader.GetInt32(reader.GetOrdinal("Rating")),
                                    Review = reader.GetString(reader.GetOrdinal("Review")),
                                    ReviewedAt = reader.GetDateTime(reader.GetOrdinal("ReviewedAt")),
                                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
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
                        throw new Exception($"Error while fetching all addresses of user with userId {userId}: " + ex.Message, ex);
                    }
                }
            }
            return feedback;
        }

        public List<FeedbackEntity> ViewAllFeedbacks()
        {
            var feedbacks = new List<FeedbackEntity>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("usp_view_all_feedbacks", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                feedbacks.Add(new FeedbackEntity()
                                {
                                    FeedbackId = reader.GetInt32(reader.GetOrdinal("FeedbackId")),
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                                    Rating = reader.GetInt32(reader.GetOrdinal("Rating")),
                                    Review = reader.GetString(reader.GetOrdinal("Review")),
                                    ReviewedAt = reader.GetDateTime(reader.GetOrdinal("ReviewedAt")),
                                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                                });
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception($"SQL Error: {ex.Message}", ex);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error while fetching all the feedbacks: " + ex.Message, ex);
                    }
                }
            }
            return feedbacks;
        }

        public List<FeedbackEntity> ViewAllFeedbacksForBook(int bookId)
        {
            var feedbacks = new List<FeedbackEntity>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("usp_view_feedbacks_for_book", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@BookId", bookId);
                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                feedbacks.Add(new FeedbackEntity()
                                {
                                    FeedbackId = reader.GetInt32(reader.GetOrdinal("FeedbackId")),
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                                    Rating = reader.GetInt32(reader.GetOrdinal("Rating")),
                                    Review = reader.GetString(reader.GetOrdinal("Review")),
                                    ReviewedAt = reader.GetDateTime(reader.GetOrdinal("ReviewedAt")),
                                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                                });
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception($"SQL Error: {ex.Message}", ex);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error while fetching all the feedbacks: " + ex.Message, ex);
                    }
                }
            }
            return feedbacks;
        }


        public FeedbackEntity EditFeedback(int userId, EditFeedbackModel editFeedbackModel)
        {
            var feedback = new FeedbackEntity();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("usp_edit_feedback", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@FeedbackId", editFeedbackModel.FeedbackId);
                    command.Parameters.AddWithValue("@Rating", editFeedbackModel.Rating);
                    command.Parameters.AddWithValue("@Review", editFeedbackModel.Review);
                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                feedback = new FeedbackEntity()
                                {
                                    FeedbackId = reader.GetInt32(reader.GetOrdinal("FeedbackId")),
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    BookId = reader.GetInt32(reader.GetOrdinal("BookId")),
                                    Rating = reader.GetInt32(reader.GetOrdinal("Rating")),
                                    Review = reader.GetString(reader.GetOrdinal("Review")),
                                    ReviewedAt = reader.GetDateTime(reader.GetOrdinal("ReviewedAt")),
                                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
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
                        throw new Exception($"Error while fetching all the feedbacks: " + ex.Message, ex);
                    }
                }
            }
            return feedback;
        }

        public bool DeleteFeedback(int userId, int feedbackId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("usp_delete_feedback", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@FeedbackId", feedbackId);
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
