using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using ModelLayer.Models.AddressModels;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Entities;

public class AddressesRepo : IAddressesRepo
{
    private readonly IConfiguration _config;
    private readonly string _connectionString;

    public AddressesRepo(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("BookstoreDBConnection");
    }

    public int InsertAddress(AddAddressModel address)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command = new SqlCommand("usp_insert_address", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", address.UserId);
                command.Parameters.AddWithValue("@Street", address.Street);
                command.Parameters.AddWithValue("@City", address.City);
                command.Parameters.AddWithValue("@State", address.State);
                command.Parameters.AddWithValue("@ZipCode", address.ZipCode);
                command.Parameters.AddWithValue("@Country", address.Country);
                command.Parameters.AddWithValue("@AddressType", address.AddressType);
                try
                {             
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
                catch (SqlException ex)
                {
                    throw new Exception($"SQL Error: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while adding address: " + ex.Message, ex);
                }
            }
        }
    }

    public List<AddressEntity> GetAddressesByUserId(int userId)
    {
        var addresses = new List<AddressEntity>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand("usp_get_addresses_by_userid", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserId", userId);
                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            addresses.Add(new AddressEntity
                            {
                                AddressId = Convert.ToInt32(reader["AddressId"]),
                                UserId = Convert.ToInt32(reader["UserId"]),
                                Street = reader["Street"].ToString(),
                                City = reader["City"].ToString(),
                                State = reader["State"].ToString(),
                                ZipCode = reader["ZipCode"].ToString(),
                                Country = reader["Country"].ToString(),
                                AddressType = reader["AddressType"].ToString(),
                                IsDeleted = Convert.ToBoolean(reader["IsDeleted"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"])
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
                    throw new Exception($"Error while fetching all addresses of user with userId {userId}: " + ex.Message, ex);
                }
                return addresses;
            }
        }
    }

    public bool UpdateAddress(UpdateAddressModel address)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand("usp_update_address", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@AddressId", address.AddressId);
                command.Parameters.AddWithValue("@Street", address.Street);
                command.Parameters.AddWithValue("@City", address.City);
                command.Parameters.AddWithValue("@State", address.State);
                command.Parameters.AddWithValue("@ZipCode", address.ZipCode);
                command.Parameters.AddWithValue("@Country", address.Country);
                command.Parameters.AddWithValue("@AddressType", address.AddressType);
                try
                {
                    return command.ExecuteNonQuery()>0;
                }
                catch (SqlException ex)
                {
                    throw new Exception($"SQL Error: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while updating address: " + ex.Message, ex);
                }
            }
        }
    }

    public bool DeleteAddress(int addressId)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand("usp_delete_address", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@AddressId", addressId);

                try
                {
                    return command.ExecuteNonQuery() > 0;
                }
                catch (SqlException ex)
                {
                    throw new Exception($"SQL Error: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while deleting address: " + ex.Message, ex);
                }
            }
        }
    }
}
