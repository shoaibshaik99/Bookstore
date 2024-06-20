using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public class ForgotPasswordRequestModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
