using System.Text;
using Tesseract;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Rendering.Skia;
using SkiaSharp;
using HiveCard.PdfParser.Interfaces;
using HiveCard.PdfParser.Models;
using System;

namespace HiveCard.PdfParser.Parsers
{
    public class OcrParser : IStatementParser
    {
        private readonly string _tessdataPath;
        public OcrParser(string tessdataPath) => _tessdataPath = tessdataPath;

        // <-- Rename & change return type to match the interface:
        public StatementResult ParseResult(string pdfPath)
        {
            var sb = new StringBuilder();
            using var doc = PdfDocument.Open(pdfPath);
            // register the Skia factory so we can get SKBitmap pages
            doc.AddSkiaPageFactory();

            using var engine = new TesseractEngine(_tessdataPath, "eng", EngineMode.Default);

            foreach (var page in doc.GetPages())
            {
                // get the rendered page as a SKBitmap
                using SKBitmap bmp = doc.GetPage<SKBitmap>(page.Number);

                // convert SKBitmap to Tesseract Pix
                using var pix = PixConverter.ToPix(bmp);
                using var ocrPage = engine.Process(pix);

                rawText.AppendLine(ocrPage.GetText());
            }

            // Now build your StatementResult:
            var text = sb.ToString();
            var transactions = TextToTransactions(text);

            return new StatementResult
            {
                HasData = transactions.Any(),
                Transactions = transactions,
                // other fields left null/default for OCR fallback
            };
        }

        private List<ExtractedTransaction> TextToTransactions(string text)
        {
            var list = new List<ExtractedTransaction>();
            foreach (var line in text.Split('\n'))
            {
                if (line.Length < 10) continue;
                if (DateTime.TryParse(line.Substring(0, 10), out var dt))
                {
                    var parts = line.Substring(11).Trim().Split(' ', 2);
                    if (parts.Length == 2 && decimal.TryParse(parts[1], out var amt))
                    {
                        list.Add(new ExtractedTransaction
                        {
                            Date = dt,
                            PostDate = dt,
                            Description = parts[0],
                            Amount = amt
                        });
                    }
                }
            }
            return list;
        }
    }
}