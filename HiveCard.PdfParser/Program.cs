using System;
using System.IO;
using HiveCard.PdfParser.Services;
using HiveCard.PdfParser.Interfaces;
using HiveCard.PdfParser.Data;
using HiveCard.PdfParser.Models;
using HiveCard.PdfParser.Parsers;

namespace HiveCard.PdfParser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1 || !File.Exists(args[0]))
            {
                Console.WriteLine("Usage: HiveCard.PdfParser.exe <path-to-pdf>");
                return;
            }

            var pdfPath = args[0];

            // IStatementParser parser = new PdfTextParser();
            IStatementParser parser = ParserFactory.Create(pdfPath);
            var parsed = parser.ParseResult(pdfPath);

            if (!parsed.HasData)
            {
                Console.WriteLine("No data found via text parser; falling back to OCR...");
                parser = new OcrParser(Path.Combine(AppContext.BaseDirectory, "tessdata"));
                parsed = parser.ParseResult(pdfPath);
            }

            if (!parsed.HasData)
            {
                Console.WriteLine("ERROR: No transactions could be extracted.");
                return;
            }

            using var db = new HiveCardDbContext();
            var account = parsed.ToEntity();
            db.Accounts.Add(account);
            db.SaveChanges();

            Console.WriteLine($"Success! Parsed & saved account {account.CardNumber} ({parsed.BankName})");

            //Console.WriteLine("Date\tDescription\tAmount");
            //foreach (var t in transactions)
            //{
            //    Console.WriteLine($"{t.Date:yyyy-MM-dd}\t{t.Description}\t{t.Amount:C}");
            //}
        }
    }
}
