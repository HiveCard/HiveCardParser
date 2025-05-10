using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveCard.PdfParser.BankParsers.BPI;
using HiveCard.PdfParser.BankParsers.Metrobank;
using HiveCard.PdfParser.BankParsers.PNB;
using HiveCard.PdfParser.Enums;
using HiveCard.PdfParser.Interfaces;
using HiveCard.PdfParser.Services;
using HiveCard.PdfParser.Services.Metrobank;
using HiveCard.PdfParser.Services.Parser;
using HiveCard.PdfParser.Services.PNB;
using HiveCard.PdfParser.StatementParser.BPI;
using UglyToad.PdfPig;
//using static System.Net.Mime.MediaTypeNames;

namespace HiveCard.PdfParser.Parsers
{
    public class ParserFactory
    {
        public static IStatementParser Create(string pdfPath)

        {
            // Open the PDF and get some text to detect which bank
            using var doc = PdfDocument.Open(pdfPath);
            var firstPageText = doc.GetPage(1).Text;

            // Local variable only—no instance field
            Bank detectedBank;
            if (firstPageText.Contains("BPI", StringComparison.OrdinalIgnoreCase))
                detectedBank = Bank.BPI;
            else if (firstPageText.Contains("Metrobank", StringComparison.OrdinalIgnoreCase))
                detectedBank = Bank.METROBANK;
            else if (firstPageText.Contains("PNB ZELO", StringComparison.OrdinalIgnoreCase))
                detectedBank = Bank.PNB;
            else
                detectedBank = Bank.Unknown;

            // Return the appropriate parser
            return detectedBank switch
            {
                Bank.BPI => new BPIVisaSignature(),
                Bank.METROBANK => new MetrobankMFree(),
                Bank.PNB => new PnbZelo(),
                _ => new PdfTextParser()
            };
        }
    }

}
