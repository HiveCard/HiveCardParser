using HiveCard.PdfParser.Models;
using HiveCard.PdfParser.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveCard.PdfParser.Interfaces
{
    public interface IExtractor
    {
        public CreditCardAccount Result { get; }
        public void BeginExtraction();
        public string GetAccountNumber(string str);
        public string GetStatementDate(string str);
        public string GetPaymentDueDate(string str);
        public string GetTotalAmount(string str);
        public string GetMinimumAmountDue(string str);
        public List<TransactionDetail> GetCardActivities(string[] str);

        public event ProcessCompletedEventHandler ProcessCompleted;
    }
}
