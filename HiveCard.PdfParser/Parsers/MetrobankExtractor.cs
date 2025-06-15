using HiveCard.PdfParser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveCard.PdfParser.Models;
using HiveCard.PdfParser.Helpers;

namespace HiveCard.PdfParser.Parsers
{
    //private readonly int[] _accSummaryCoordinates = new int[] { /* update if needed */ };
   // private readonly int[] _breakDownListCoordinates = new int[] { /* update if needed */ };
    internal class MetrobankExtractor : ExtractorHelper, IExtractor
    {
        public BankStatement Run(string pdfPath)
        {
            var bankStatement = new BankStatement();

            return bankStatement;
        }

    }
}
