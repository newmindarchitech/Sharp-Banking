
using System.ComponentModel.DataAnnotations;

namespace BankingVault.Models
{
    public class TransactionRecord
    {
        [Key]
        public Guid RecordID { get; set; }

        public Guid AccountContextID { get; set; }
        public AccountType? AccountContext;

        public Guid OwnerID { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
    }
}
