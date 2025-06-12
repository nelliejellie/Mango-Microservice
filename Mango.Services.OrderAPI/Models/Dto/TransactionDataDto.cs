using System.Net;

namespace Mango.Services.OrderAPI.Models.Dto
{
    public class TransactionDataDto
    {
        public long Id { get; set; }
        public string Domain { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public string ReceiptNumber { get; set; }
        public int Amount { get; set; }
        public string Message { get; set; }
        public string GatewayResponse { get; set; }
        public DateTime PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Channel { get; set; }
        public string Currency { get; set; }
        public string IpAddress { get; set; }
        public string Metadata { get; set; }
        public TransactionLogDto Log { get; set; }
        public int Fees { get; set; }
        public object FeesSplit { get; set; }
        public Authorization Authorization { get; set; }
        public CustomerDto Customer { get; set; }
        public object Plan { get; set; }
        public object Split { get; set; }
        public string OrderId { get; set; }
        public DateTime PaidAtAlt { get; set; }
        public DateTime CreatedAtAlt { get; set; }
        public int RequestedAmount { get; set; }
        public object PosTransactionData { get; set; }
        public object Source { get; set; }
        public object FeesBreakdown { get; set; }
        public object Connect { get; set; }
        public DateTime TransactionDate { get; set; }
        public object PlanObject { get; set; }
        public object Subaccount { get; set; }
    }
}
