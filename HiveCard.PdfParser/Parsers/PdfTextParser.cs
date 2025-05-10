using System;
using System.Collections.Generic;
using HiveCard.PdfParser.Interfaces;
using HiveCard.PdfParser.Models;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Rendering;

namespace HiveCard.PdfParser.Parsers
{
    public class PdfTextParser : IStatementParser
    {
        public StatementResult Parse(string pdfPath)
        {
            var transactions = new List<Transaction>();
            using var document = PdfDocument.Open(pdfPath);
            foreach (var page in document.GetPages())
            {
                var lines = page.Text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                bool inTable = false;
                foreach (var line in lines)
                {
                    if (!inTable && line.StartsWith("Date", StringComparison.OrdinalIgnoreCase) && line.Contains("Amount"))
                    {
                        inTable = true;
                        continue;
                    }
                    if (inTable)
                    {
                        if (line.StartsWith("Total", StringComparison.OrdinalIgnoreCase)) break;
                        var parts = line.Split(' ', 4, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 4) continue;
                        if (!DateTime.TryParse(parts[0], out var date)) continue;
                        var desc = parts[1];
                        var amtText = parts[3].Replace(",", "").Replace("₱", "").Trim();
                        if (!decimal.TryParse(amtText, out var amount)) continue;
                        transactions.Add(new Transaction { Date = date, Description = desc, Amount = amount });
                    }
                }
            }
            return transactions;
        }
    }
}
