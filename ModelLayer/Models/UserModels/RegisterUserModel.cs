using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models.UserModels
{
    public class RegisterUserModel
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
