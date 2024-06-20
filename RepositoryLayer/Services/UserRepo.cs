using Microsoft.Extensions.Configuration;
using RepositoryLayer.Entities;
using RepositoryLayer.Interfaces;
using System.Data;
using System.Data.SqlClient;
using UtilityLayer;
using ModelLayer.Models.UserModels;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ModelLayer.Models;

namespace RepositoryLayer.Services
{
    public class UserRepo : IUserRepo
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public UserRepo(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("BookstoreDBConnection");
        }

        public bool IsRegisteredEmail(string email)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand checkEmailCommand = new SqlCommand("SELECT COUNT(1) FROM users WHERE email = @Email", connection))
                {
                    checkEmailCommand.Parameters.AddWithValue("@Email", email);
                    return (int)checkEmailCommand.ExecuteScalar() > 0;
                }
            }
        }

        public UserEntity RegisterUser(RegisterUserModel userModel)
        {
            if (IsRegisteredEmail(userModel.Email))
            {
                throw new InvalidOperationException("A user with this email already exists.");
            }

            // Hash the password before storing
            var hashedPassword = PasswordHelper.HashPassword(userModel.Password);

            UserEntity registeredUser = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand createCommand = new SqlCommand("usp_create_user", connection))
                {
                    createCommand.CommandType = CommandType.StoredProcedure;
                    createCommand.Parameters.AddWithValue("@fullName", userModel.FullName);
                    createCommand.Parameters.AddWithValue("@Email", userModel.Email);
                    createCommand.Parameters.AddWithValue("@contactNumber", userModel.ContactNumber);
                    createCommand.Parameters.AddWithValue("@hashedPassword", hashedPassword);

                    using (SqlDataReader reader = createCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            registeredUser = new UserEntity
                            {
                                UserId = Convert.ToInt32(reader["userId"]),
                                FullName = userModel.FullName,
                                Email = userModel.Email,
                                ContactNumber = userModel.ContactNumber,
                                HashedPassword = hashedPassword
                            };
                        }
                    }
                }
            }

            return registeredUser;
        }

        public FetchUserModel GetUserById(int userId)
        {
            FetchUserModel user = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand getUserCommand = new SqlCommand("usp_get_user_by_id", connection))
                {
                    getUserCommand.CommandType = CommandType.StoredProcedure;
                    getUserCommand.Parameters.AddWithValue("@userId", userId);

                    using (SqlDataReader reader = getUserCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new FetchUserModel
                            {
                                UserId = Convert.ToInt32(reader["userId"]),
                                FullName = reader["fullName"].ToString(),
                                Email = reader["email"].ToString(),
                                ContactNumber = reader["contactNumber"].ToString(),
                            };
                        }
                    }
                }
            }

            return user;
        }

        public FetchUserModel GetUserByEmail(string email)
        {
            FetchUserModel user = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand getUserCommand = new SqlCommand("usp_get_user_by_email", connection))
                {
                    getUserCommand.CommandType = CommandType.StoredProcedure;
                    getUserCommand.Parameters.AddWithValue("@Email", email);

                    using (SqlDataReader reader = getUserCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new FetchUserModel
                            {
                                UserId = Convert.ToInt32(reader["userId"]),
                                FullName = reader["fullName"].ToString(),
                                Email = reader["email"].ToString(),
                                ContactNumber = reader["contactNumber"].ToString()
                            };
                        }
                    }
                }
            }

            return user;
        }

        public List<FetchUserModel> GetAllUsers()
        {
            List<FetchUserModel> users = new List<FetchUserModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand getAllUsersCommand = new SqlCommand("usp_get_all_users", connection))
                {
                    getAllUsersCommand.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = getAllUsersCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new FetchUserModel
                            {
                                UserId = Convert.ToInt32(reader["userId"]),
                                FullName = reader["fullName"].ToString(),
                                Email = reader["email"].ToString(),
                                ContactNumber = reader["contactNumber"].ToString(),
                            };
                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }

        public bool UpdateUser(int userId, UpdateUserModel user)
        {
            // Hash the password before updating
            var hashedPassword = PasswordHelper.HashPassword(user.Password);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand updateUserCommand = new SqlCommand("usp_update_user", connection))
                {
                    updateUserCommand.CommandType = CommandType.StoredProcedure;
                    updateUserCommand.Parameters.AddWithValue("@userId", userId);
                    updateUserCommand.Parameters.AddWithValue("@fullName", user.FullName);
                    updateUserCommand.Parameters.AddWithValue("@Email", user.Email);
                    updateUserCommand.Parameters.AddWithValue("@contactNumber", user.ContactNumber);
                    updateUserCommand.Parameters.AddWithValue("@hashedPassword", hashedPassword);

                    int rowsAffected = updateUserCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool DeleteUser(int userId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand deleteUserCommand = new SqlCommand("usp_delete_user", connection))
                {
                    deleteUserCommand.CommandType = CommandType.StoredProcedure;
                    deleteUserCommand.Parameters.AddWithValue("@userId", userId);

                    int rowsAffected = deleteUserCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public string LoginUser(UserLoginModel loginModel)
        {
            FetchUserModel user = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand loginUserCommand = new SqlCommand("usp_login_user", connection))
                {
                    loginUserCommand.CommandType = CommandType.StoredProcedure;
                    loginUserCommand.Parameters.AddWithValue("@Email", loginModel.Email);
                    loginUserCommand.Parameters.AddWithValue("@hashedPassword", PasswordHelper.HashPassword(loginModel.Password));

                    using (SqlDataReader reader = loginUserCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new FetchUserModel
                            {
                                UserId = Convert.ToInt32(reader["userId"]),
                                FullName = reader["fullName"].ToString(),
                                Email = reader["email"].ToString(),
                                ContactNumber = reader["contactNumber"].ToString()
                            };
                        }
                        else
                        {
                            return null; // No user found with the provided details
                        }
                    }
                }
            }
            // Generate and return the token
            return GenerateToken(user.Email, user.UserId);
        }

        private string GenerateToken(string email, int userId)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new Claim[]
            {
                new Claim("Email",email),
                new Claim("UserId",userId.ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                //expires: DateTime.Now.AddMinutes(15),
                expires: DateTime.Now.AddMonths(1),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public ForgotPasswordModel ForgotPassword(string email)
        {
            ForgotPasswordModel forgotPasswordModel = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand forgotPasswordCommand = new SqlCommand("usp_forgot_password", connection))
                {
                    forgotPasswordCommand.CommandType = CommandType.StoredProcedure;
                    forgotPasswordCommand.Parameters.AddWithValue("@Email", email);

                    using (SqlDataReader reader = forgotPasswordCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            forgotPasswordModel = new ForgotPasswordModel
                            {
                                UserId = Convert.ToInt32(reader["userId"]),
                                Email = email,
                                Token = GenerateToken(email, Convert.ToInt32(reader["userId"]))
                            };
                        }
                    }
                }
            }

            return forgotPasswordModel;
        }

        public bool ResetPassword(string email, ResetPasswordModel resetPasswordModel)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand resetPasswordCommand = new SqlCommand("usp_reset_password", connection))
                {
                    resetPasswordCommand.CommandType = CommandType.StoredProcedure;
                    resetPasswordCommand.Parameters.AddWithValue("@Email", email);
                    resetPasswordCommand.Parameters.AddWithValue("@NewHashedPassword", PasswordHelper.HashPassword(resetPasswordModel.NewPassword));

                    int rowsAffected = resetPasswordCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

    }
}
