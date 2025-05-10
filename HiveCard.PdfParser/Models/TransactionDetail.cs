using System;

namespace HiveCard.PdfParser.Models
{
    public class TransactionDetail
    {
        public int Id { get; set; }
        public int CreditCardAccountId { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime? PostingDate { get; set; }
        public string Description { get; set; } = null!;
        public decimal Amount { get; set; }
        public CreditCardAccount CreditCardAccount { get; set; } = null!;
    }
}
