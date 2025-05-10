using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveCard.PdfParser.Interfaces;
using HiveCard.PdfParser.Models;
using UglyToad.PdfPig;

namespace HiveCard.PdfParser.BankParsers.PNB
{
    public class PnbZelo : IStatementParser
    {
        public StatementResult ParseResult(string pdfPath)
        {
            // 1) Use PdfPig to extract raw text, detect table, then map into StatementResult
            using var doc = PdfDocument.Open(pdfPath);
            var firstPageText = string.Join("\n", doc.GetPage(1).Text.Split('\n').Take(5));

            // 2) (Pseudo) call to your existing PNB extractor logic
            var result = YourPnbExtractor.Parse(pdfPath);
            result.HasData = true;
            return result;
        }
    }
}
