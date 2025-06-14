using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveCard.PdfParser.Models
{
    public class BankStatement
    {
        public string AccountNumber { get; set; } = "";
        public string StatementDate { get; set; } = "";
        public string PaymentDueDate { get; set; } = "";
        public string TotalAmount { get; set; } = "";
        public string MinimumAmountDue { get; set; } = "";
        public List<CardActivities> Activities { get; set; } = new();
    }
}
