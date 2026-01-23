using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BankingVault.Models
{
    public class BankAccountCreationForm
    {
        [DisplayName("OwnerID")]
        [ReadOnly(true)]
        public Guid OwnerID { get; set; }

        [DisplayName("Total Balance")]
        [Required(ErrorMessage ="Total Balance Cannot be Empty")]
        public decimal? TotalBalance { get; set; }

        [Required(ErrorMessage ="Deposit Amount Is Required")]
        public decimal DepositAmount {  get; set; }

        public AccountContext AccountContext { get; set; }

    }
}
