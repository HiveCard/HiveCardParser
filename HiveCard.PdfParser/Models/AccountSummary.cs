using System;

namespace HiveCard.PdfParser.Models
{
    public class AccountSummary
    {
        public int Id { get; set; }
        public int CreditCardAccountId { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal Purchases { get; set; }
        public decimal Payments { get; set; }
        public decimal? ServiceCharges { get; set; }
        public decimal? FinanceCharges { get; set; }
        public decimal? FinanceChargeRate { get; set; }
        public decimal? LateCharges { get; set; }
        public decimal? OtherDebits { get; set; }
        public CreditCardAccount CreditCardAccount { get; set; } = null!;
    }
}
