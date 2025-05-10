using System;
using System.Collections.Generic;

namespace HiveCard.PdfParser.Models
{
    public class CreditCardAccount
    {
        public int Id { get; set; }
        public string BankName { get; set; } = null!;
        public string CardNumber { get; set; } = null!;
        public DateTime StatementDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal? CreditLimit { get; set; }
        public decimal TotalAmountDue { get; set; }
        public decimal MinimumAmountDue { get; set; }
        public AccountSummary? Summary { get; set; }
        public List<TransactionDetail> Details { get; set; } = new();
        public List<InstallmentDetail> Installments { get; set; } = new();
    }
}
