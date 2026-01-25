
using System.ComponentModel.DataAnnotations;

namespace BankingVault.Models
{
    public class TransactionRecord
    {
        [Key]
        public Guid AccountRecordID { get; set; }

        public Guid AccountContextID { get; set; }
        public AccountType? AccountContext;

        public Guid OwnerRecordID { get; set; }
        public BankRecord? BankRecord { get; set; }

        public ICollection<Transaction>? Transactions { get; set; }
    }
}
