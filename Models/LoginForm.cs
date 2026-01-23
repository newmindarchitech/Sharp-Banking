using System.ComponentModel.DataAnnotations;

namespace BankingVault.Models
{
    public class LoginForm
    {
        [Required(ErrorMessage = "Your Email Address is required"), EmailAddress]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Your password is required")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
