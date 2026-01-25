using System.ComponentModel.DataAnnotations;

namespace BankingVault.Models
{
    public class BankRecord
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserAccountID { get; set; }
        public UserAccount? OwnerRecord { get; set; }

        public ICollection<TransactionRecord>? BankContextRecords { get; set; }
    }
}
