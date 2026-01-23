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

        [Precision(5,2)]
        public decimal? PenaltyFees { get; set; }
        [Precision(16,2)]
        public decimal? DepositFee { get; set; }
        [Precision(16,2)]
        public decimal Balance { get; set; }

        public DateTime CreatedDate {  get; set; }

        public DateTime DeductionDate { get; set; }
        public Guid AccountTransactionRecordID { get; set; }
        public TransactionRecord? Record;

    }
    public enum AccountContext
    {
        Checking,
        Saving,
        MoneyMarket,
        CertificateOfDeposit
    }
}
