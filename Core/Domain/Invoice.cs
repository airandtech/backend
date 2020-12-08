using System.ComponentModel.DataAnnotations.Schema;

namespace AirandWebAPI.Core.Domain
{
    public class Invoice : BaseEntity
    {
        public Invoice(decimal amount, string requestorEmail, string status, string txnId)
        {
            Amount = amount;
            CustomerEmail = requestorEmail;
            Status = status;
            AirandTxnId = txnId;
        }

        public Invoice(){}

        [Column(TypeName = "decimal(18,4)")]
        public decimal Amount { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal AmountPaid { get; set; }
        public int OrderId { get; set; }
        public string OrderIds { get; set; }
        public string ResponseBody { get; set; }
        public string AirandTxnId {get; set; }
        public int TransactionId { get; set; }
        public string CustomerEmail { get; set; }
        public string Status { get; set; }
    }
}
