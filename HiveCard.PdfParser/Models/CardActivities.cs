using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveCard.PdfParser.Models
{

    public class CardActivities
    {
        private string _transactionDate;
        private string _postDate;
        private string _description;
        private string _amount;

        public string TransactionDate { get => _transactionDate; set => _transactionDate = value; }
        public string PostDate { get => _postDate; set => _postDate = value; }
        public string Description { get => _description; set => _description = value; }
        public string Amount { get => _amount; set => _amount = value; }
    }


}
