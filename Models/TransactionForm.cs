using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BankingVault.Models
{
    public class TransactionForm
    {
        [ReadOnly(true)]
        public Guid RecordID { get; set; }

        [Required(ErrorMessage ="Transaction Amount cannot be empty")]
        public decimal TransactionAmount { get; set; }

        [Required(ErrorMessage ="Transaction Context Needs to be clarified")]
        public TransactionType TransactionContext {  get; set; }
    }
}
