using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankingVault.Models
{
    public class Transaction
    {
        [Key]
        public Guid TransactionID {  get; set; }
        public Guid RecordID { get; set; }

        [JsonIgnore]
        public required TransactionRecord Record { get; set; }

        [Precision(16,2)]
        public decimal TransactionAmount {  get; set; }

        public TransactionType TransactionContext {  get; set; }
        public DateTime CreatedDate { get; set; }


    }
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        Transfer,
        Payment,

    }
}
