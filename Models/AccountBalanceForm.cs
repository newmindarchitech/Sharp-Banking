using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BankingVault.Models
{
    public class AccountBalanceForm
    {
        [Required]
        [ReadOnly(true)]
        public Guid UserID { get; set; }
        [Required]
        [ReadOnly(true)]
        public decimal? AccountBalance {  get; set; }
        public decimal DepositAmount {  get; set; }
    }
}
