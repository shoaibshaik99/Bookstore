using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Models
{
    public class ForgotPasswordModel
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
