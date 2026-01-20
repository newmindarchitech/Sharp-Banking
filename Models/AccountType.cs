using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BankingVault.Models
{
    public class AccountType
    {
        [Key]
        public Guid AccountID { get; set; }
        [Required]
        public Guid OwnerID { get; set; }

        public AccountContext Context {  get; set; }

        [Precision(3,2)]
        public decimal InterestRate {  get; set; }

        public int? WithDrawalLimits { get; set; }

        public int? PenaltyFees { get; set; }

        public int? DepositFee { get; set; }
        [Precision(16,2)]
        public decimal Balance { get; set; }

        public DateTime CreatedDate {  get; set; }
        public Guid AccountTransactionRecordID { get; set; }
        public required TransactionRecord Record;

    }
    public enum AccountContext
    {
        Deposit,
        MoneyMarket,
        CD
    }
}
