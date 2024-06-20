using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public class ResetPasswordModel
    {
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
}
