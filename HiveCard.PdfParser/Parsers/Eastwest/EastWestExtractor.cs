using HiveCard.PdfParser.Helpers;
using HiveCard.PdfParser.Interfaces;
using HiveCard.PdfParser.Models;
using HiveCard.PdfParser.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HiveCard.PdfParser.Parsers.Eastwest
{
    public class EastWestExtractor : ExtractorHelper, IExtractor
    {
        #region NOTE: THESE VALUES ARE SUBJECT TO CHANGE
        private int[] _accSummaryCoordinates = new int[] { 850, 600, 0, 0, 850, 600, 1160, 360, 850, 600 };
        private int[] _breakDownListCoordinates = new int[] { 1850, 1550, 0, 0, 1850, 1550, 175, 570, 1850, 1550 };
        #endregion


        public BankStatement Run(string pdfPath)
        {

            var summaryCrop = new CropArea(1160, 335, 847, 615);
            var detailsCrop = new CropArea(102, 458, 2019, 1670);


            var imagePaths = PdfToImageHelper.ConvertPdfToImages(pdfPath);


            // Define exactly which pages you want to parse (0-based index)
            var pagesToScan = new List<int> { 0, 1 };

            var pagesToParse = imagePaths
                    .Select((path, index) => new { path, index })
                    .Where(x => pagesToScan.Contains(x.index))
                    .ToList();


            // Crop images
            var croppedPaths = ImageCropper.CropImages(pagesToParse.Select(x => x.path).ToList(), summaryCrop, detailsCrop);

            var bankStatement = new BankStatement();

            for (int i = 0; i < croppedPaths.Count; i++)
            {
                var croppedImage = croppedPaths[i];
                var text = OcrHelper.ExtractText(croppedImage);
                File.WriteAllText(Path.ChangeExtension(croppedImage, ".txt"), text);

                string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                var sections = ParserUtils.SplitIntoSections(lines, line => line.Contains("SALE", StringComparison.OrdinalIgnoreCase));

                ////Debugging: Uncomment to see the lines being processed
                //for (int x = 0; x < lines.Length; x++)
                //{
                //    Console.WriteLine($"{x}: {lines[x]}");
                //}



                if (i == 0)
                {
                    // First page - extract summary
                    
                    bankStatement.StatementDate = GetStatementDate(sections.HeaderLines);
                    bankStatement.PaymentDueDate = GetPaymentDueDate(sections.HeaderLines);
                    bankStatement.TotalAmount = GetTotalAmount(sections.HeaderLines);
                    bankStatement.MinimumAmountDue = GetMinimumAmountDue(sections.HeaderLines);
                    
                }
                else
                {
                    GetCardHolderWithNumber(lines, bankStatement);
                    bankStatement.Activities.AddRange(GetCardActivities(sections.TransactionLines));
                }
            }
            // Optional: write JSON output
            File.WriteAllText("Output/eastwest_extracted.json", JsonConvert.SerializeObject(bankStatement, Formatting.Indented));
            return bankStatement;
        }


        private string GetStatementDate(List<string> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Contains("Statement Date", StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(lines[i], @"\b[A-Z]{3}\s+\d{2}\s+\d{4}\b", RegexOptions.IgnoreCase);
                    return match.Success ? match.Value : "";
                }
            }

            return "";
        }

        private string GetPaymentDueDate(List<string> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Contains("Payment Due Date", StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(lines[i], @"\b[A-Z]{3}\s+\d{2}\s+\d{4}\b", RegexOptions.IgnoreCase);
                    return match.Success ? match.Value : "";
                }
            }

            return "";
        }

        private string GetTotalAmount(List<string> lines)
        {
            foreach (var line in lines)
            {
                if (line.Contains("Total Statement Balance", StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(line, @"\d{1,3}(?:,\d{3})*\.\d{2}");
                    if (match.Success)
                        return match.Value;
                }
            }

            return "";
        }
        private string GetMinimumAmountDue(List<string> lines)
        {
            foreach (var line in lines)
            {
                if (line.Contains("Minimum Payment Due", StringComparison.OrdinalIgnoreCase))
                {
                    var match = Regex.Match(line, @"\d{1,3}(?:,\d{3})*\.\d{2}");
                    if (match.Success)
                        return match.Value;
                }
            }

            return "";
        }

        private void GetCardHolderWithNumber(string[] lines, BankStatement bankStatement)
        {
            var pattern = new Regex(@"^(?<name>.+?)\s+(?<number>\d{4}-\d{4}-\d{4}-\d{4})$");

            foreach (var line in lines)
            {
                var match = pattern.Match(line.Trim());
                if (match.Success)
                {
                    bankStatement.AccountNumber = match.Groups["number"].Value;

                    break;
                }
            }
        }

        private List<CardActivities> GetCardActivities(List<string> lines)
        {
            var cleaned = lines
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => l.Trim())
                .ToList();

            // Remove headers and irrelevant labels
            var skipLabels = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "SALE", "DATE", "POST", "TRANSACTION DESCRIPTION", "PESO", "AMOUNT",
        "PREVIOUS BALANCE", "TOTAL STATEMENT BALANCE", "**END OF STATEMENT***"
    };

            cleaned = cleaned.Where(line => !skipLabels.Contains(line.ToUpper())).ToList();

            int total = cleaned.Count;
            int approxSectionSize = total / 4;

            var transactionDates = new List<string>();
            var postDates = new List<string>();
            var descriptions = new List<string>();
            var amounts = new List<string>();

            for (int i = 0; i < total; i++)
            {
                if (i < approxSectionSize)
                    transactionDates.Add(cleaned[i]);
                else if (i < approxSectionSize * 2)
                    postDates.Add(cleaned[i]);
                else if (i < approxSectionSize * 3)
                    descriptions.Add(cleaned[i]);
                else
                    amounts.Add(cleaned[i]);
            }

            // Use the shortest column length to avoid IndexOutOfRange
            int activityCount = new[] { transactionDates.Count, postDates.Count, descriptions.Count, amounts.Count }.Min();

            var activities = new List<CardActivities>();
            for (int i = 0; i < activityCount; i++)
            {
                activities.Add(new CardActivities
                {
                    TransactionDate = transactionDates[i],
                    PostDate = postDates[i],
                    Description = descriptions[i],
                    Amount = amounts[i]
                });
            }

            return activities;
        }

        private List<string> PreprocessTransactionLines(List<string> rawLines)
        {
            var cleaned = new List<string>();

            var ignoreKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "SALE", "DATE", "POST", "TRANSACTION DESCRIPTION", "PESO", "AMOUNT",
        "**END OF STATEMENT**", "Total Statement Balance"
    };

            foreach (var line in rawLines)
            {
                var trimmed = line.Trim();

                // Skip empty or known headers
                if (string.IsNullOrWhiteSpace(trimmed) || ignoreKeywords.Contains(trimmed))
                    continue;

    

                cleaned.Add(trimmed);
            }

            return cleaned;
        }
    }
}
