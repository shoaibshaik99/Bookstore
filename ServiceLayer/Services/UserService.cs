using ServiceLayer.Interfaces;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Entities;
using ModelLayer.Models.UserModels;
using ModelLayer.Models;

namespace ServiceLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;

        public UserService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public bool IsRegisteredEmail(string email)
        {
            return _userRepo.IsRegisteredEmail(email);
        }

        public UserEntity RegisterUser(RegisterUserModel userModel)
        {
            return _userRepo.RegisterUser(userModel);
        }

        public FetchUserModel GetUserById(int userId)
        {
            return _userRepo.GetUserById(userId);
        }

        public FetchUserModel GetUserByEmail(string email)
        {
            return _userRepo.GetUserByEmail(email);
        }

        public List<FetchUserModel> GetAllUsers()
        {
            return _userRepo.GetAllUsers();
        }

        public bool UpdateUser(int userId, UpdateUserModel user)
        {
            return _userRepo.UpdateUser(userId, user);
        }

        public bool DeleteUser(int userId)
        {
            return _userRepo.DeleteUser(userId);
        }

        public string LoginUser(UserLoginModel loginModel)
        {
            return _userRepo.LoginUser(loginModel);
        }

        public ForgotPasswordModel ForgotPassword(string email)
        {
            return _userRepo.ForgotPassword(email);
        }

        public bool ResetPassword(string email, ResetPasswordModel resetPasswordModel)
        {
            return _userRepo.ResetPassword(email, resetPasswordModel);
        }
    }
}
