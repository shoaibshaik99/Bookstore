using ModelLayer.Models;
using ModelLayer.Models.UserModels;
using RepositoryLayer.Entities;

namespace ServiceLayer.Interfaces
{
    public interface IUsersService
    {
        public bool IsRegisteredEmail(string email);
        public UserEntity RegisterUser(RegisterUserModel userModel);
        public FetchUserModel GetUserById(int userId);
        public FetchUserModel GetUserByEmail(string email);
        public List<FetchUserModel> GetAllUsers();
        public bool UpdateUser(int userId, UpdateUserModel user);
        public bool DeleteUser(int userId);

        public string LoginUser(UserLoginModel loginModel);
        public ForgotPasswordModel ForgotPassword(string email);
        public bool ResetPassword(string email, ResetPasswordModel resetPasswordModel);
    }
}
