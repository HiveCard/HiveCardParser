using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveCard.PdfParser.Models
{
    public class StatementResult
    {
        public bool HasData { get; set; }
        public string BankName { get; set; } = null!;
        public string CardNumber { get; set; } = null!;
        public DateTime StatementDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal? CreditLimit { get; set; }
        public decimal TotalAmountDue { get; set; }
        public decimal MinimumAmountDue { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal Purchases { get; set; }
        public decimal Payments { get; set; }
        public decimal? ServiceCharges { get; set; }
        public decimal? FinanceCharges { get; set; }
        public decimal? FinanceChargeRate { get; set; }

        public List<ExtractedTransaction> Transactions { get; set; } = new();
        public List<ExtractedInstallment> Installments { get; set; } = new();

        public CreditCardAccount ToEntity()
        {
            var acc = new CreditCardAccount
            {
                BankName = BankName,
                CardNumber = CardNumber,
                StatementDate = StatementDate,
                DueDate = DueDate,
                CreditLimit = CreditLimit,
                TotalAmountDue = TotalAmountDue,
                MinimumAmountDue = MinimumAmountDue,
                Summary = new AccountSummary
                {
                    PreviousBalance = PreviousBalance,
                    Purchases = Purchases,
                    Payments = Payments,
                    ServiceCharges = ServiceCharges,
                    FinanceCharges = FinanceCharges,
                    FinanceChargeRate = FinanceChargeRate
                }
            };
            acc.Details = Transactions
                .Select(tx => new TransactionDetail
                {
                    TransactionDate = tx.Date,
                    PostingDate = tx.PostDate,
                    Description = tx.Description,
                    Amount = tx.Amount
                }).ToList();
            acc.Installments = Installments
                .Select(ins => new InstallmentDetail
                {
                    Description = ins.Description,
                    RemainingBalance = ins.RemainingBalance,
                    RemainingMonths = ins.RemainingMonths,
                    StartDate = ins.StartDate,
                    UnbilledAmount = ins.UnbilledAmount,
                    EstablishmentName = ins.EstablishmentName
                }).ToList();
            return acc;
        }
    }
}
