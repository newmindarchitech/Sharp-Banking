
using System.ComponentModel.DataAnnotations;

namespace BankingVault.Models
{
    public class TransactionRecord
    {
        [Key]
        public Guid RecordID { get; set; }

        public Guid AccountContextID { get; set; }
        public required AccountType Account;

        public required ICollection<Transaction> Transactions { get; set; }
    }
}
