using System.ComponentModel.DataAnnotations;

namespace BankingVault.Models
{
    public class UserAccount
    {
        [Key]
        public Guid Id { get; set; }

        [MinLength(5)]
        [Required]
        public required string UserName { get; set; }

        [Required, EmailAddress]
        public required string EmailAddress { get; set; }

        [Required]
        [MinLength(8)]
        public required string PasswordHash { get; set; }

        public required string PasswordSalt { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

    }
}
