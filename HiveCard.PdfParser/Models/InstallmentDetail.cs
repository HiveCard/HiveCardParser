using System;

namespace HiveCard.PdfParser.Models
{
    public class InstallmentDetail
    {
        public int Id { get; set; }
        public int CreditCardAccountId { get; set; }
        public string Description { get; set; } = null!;
        public decimal RemainingBalance { get; set; }
        public int RemainingMonths { get; set; }
        public DateTime? StartDate { get; set; }
        public decimal? UnbilledAmount { get; set; }
        public string? EstablishmentName { get; set; }
        public CreditCardAccount CreditCardAccount { get; set; } = null!;
    }
}
