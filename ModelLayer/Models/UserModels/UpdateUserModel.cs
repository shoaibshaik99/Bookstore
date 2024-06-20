using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models.UserModels
{
    public class UpdateUserModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string Password { get; set; }
    }
}
