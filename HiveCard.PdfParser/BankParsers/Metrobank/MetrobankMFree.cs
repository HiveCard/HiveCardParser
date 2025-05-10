using HiveCard.PdfParser.Interfaces;
using HiveCard.PdfParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;

namespace HiveCard.PdfParser.BankParsers.Metrobank
{
    public class MetrobankMFree : IStatementParser
    {
        public StatementResult ParseResult(string pdfPath)
        {
            using var doc = PdfDocument.Open(pdfPath);
            // detect Metrobank by some header text
            // then call your Metrobank extractor
            var result = YourMetroExtractor.Parse(pdfPath);
            result.HasData = true;
            return result;
        }
    }
}
