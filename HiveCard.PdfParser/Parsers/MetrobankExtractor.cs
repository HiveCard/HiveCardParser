using HiveCard.PdfParser.Helpers;
using HiveCard.PdfParser.Interfaces;
using HiveCard.PdfParser.Models;
using HiveCard.PdfParser.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HiveCard.PdfParser.Parsers
{

    public class MetrobankExtractor : ExtractorHelper, IExtractor
    {
        public BankStatement Run(string pdfPath)
        {
            var summaryCrop = new CropArea(1055, 25, 927, 584);
            var detailsCrop = new CropArea(666, 229, 1386, 2513);

            var imagePaths = PdfToImageHelper.ConvertPdfToImages(pdfPath);
            
            // Define exactly which pages you want to parse (0-based index)
            var pagesToScan = new List<int> { 0, 1 };

            var pagesToParse = imagePaths
                .Select((path, index) => new { path, index })
                .Where(x => pagesToScan.Contains(x.index))
                .ToList();


            var croppedPaths = ImageCropper.CropImages(pagesToParse.Select(x => x.path).ToList(), summaryCrop, detailsCrop);

            var bankStatement = new BankStatement();

            for (int i = 0; i < croppedPaths.Count; i++)
            {
                var croppedImage = croppedPaths[i];
                var text = OcrHelper.ExtractText(croppedImage);
                File.WriteAllText(Path.ChangeExtension(croppedImage, ".txt"), text);

                string[] lines = text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

                var sections = ParserUtils.SplitIntoSections(lines, line => line.Contains("PESO ACCOUNT DETAILS", StringComparison.OrdinalIgnoreCase));


                ////Debugging: Uncomment to see the lines being processed
                //for (int x = 0; x < lines.Length; x++)
                //{
                //    Console.WriteLine($"{x}: {lines[x]}");
                //}



                if (i == 0)
                {
                    // Header parsing
                    bankStatement.AccountNumber = GetAccountNumber(sections.HeaderLines);
                    bankStatement.StatementDate = GetStatementDate(sections.HeaderLines);
                    bankStatement.PaymentDueDate = GetPaymentDueDate(sections.HeaderLines);
                    bankStatement.TotalAmount = GetTotalAmount(sections.HeaderLines);
                    bankStatement.MinimumAmountDue = GetMinimumAmountDue(sections.HeaderLines);
                }
                else
                {
                    // Transaction parsing
                    bankStatement.Activities.AddRange(GetCardActivities(sections.TransactionLines));
                }
            }

            File.WriteAllText("Output/metrobank_extracted.json", JsonConvert.SerializeObject(bankStatement, Formatting.Indented));
            return bankStatement;
        }

        private string GetAccountNumber(List<string> lines)
        {
            for (int i = 0; i < lines.Count - 1; i++)
            {
                if (lines[i].Contains("CREDIT CARD ACCOUNT", StringComparison.OrdinalIgnoreCase))
                {
                    var possibleCardLine = lines[i + 1];
                    var match = Regex.Match(possibleCardLine, @"\d{4} \d{4} \d{4} \d{4}");
                    return match.Success ? match.Value : "";
                }
            }

            return "";
        }

        private string GetStatementDate(List<string> lines)
        {
            for (int i = 0; i < lines.Count - 1; i++)
            {
                if (lines[i].Contains("Statement Date", StringComparison.OrdinalIgnoreCase))
                {
                    // The next line should contain both dates
                    var nextLine = lines[i + 1];
                    var match = Regex.Match(nextLine, @"\d{1,2}\s+[A-Za-z]+\s+\d{4}");
                    return match.Success ? match.Value : "";
                }
            }

            return "";
        }

        private string GetPaymentDueDate(List<string> lines)
        {
            for (int i = 0; i < lines.Count - 1; i++)
            {
                if (lines[i].Contains("Payment Due Date", StringComparison.OrdinalIgnoreCase))
                {
                    var nextLine = lines[i + 1];
                    var matches = Regex.Matches(nextLine, @"\d{1,2}\s+[A-Za-z]+\s+\d{4}");
                    return matches.Count > 1 ? matches[1].Value : "";
                }
            }

            return "";
        }

        private string GetTotalAmount(List<string> lines)
        {
            for (int i = 0; i < lines.Count - 1; i++)
            {
                if (lines[i].Contains("Total Amount Due", StringComparison.OrdinalIgnoreCase))
                {
                    var nextLine = lines[i + 1];
                    var matches = Regex.Matches(nextLine, @"PHP\s?[\d,]+\.\d{2}");
                    return matches.Count > 0 ? matches[0].Value : "";
                }
            }

            return "";
        }

        private string GetMinimumAmountDue(List<string> lines)
        {
            for (int i = 0; i < lines.Count - 1; i++)
            {
                if (lines[i].Contains("Minimum Amount Due", StringComparison.OrdinalIgnoreCase))
                {
                    var nextLine = lines[i + 1];
                    var matches = Regex.Matches(nextLine, @"PHP\s?[\d,]+\.\d{2}");
                    return matches.Count > 1 ? matches[1].Value : "";
                }
            }

            return "";
        }

        private List<CardActivities> GetCardActivities(List<string> lines)
        {
            var activities = new List<CardActivities>();

            // Match lines with this pattern:
            // e.g., 03/25 10/25 INSTL 30/36 C2G VIA WEBSITE 3,457.77
            var pattern = new Regex(@"^(\d{2}/\d{2})\s+(\d{2}/\d{2})\s+(.+?)\s+([\d,]+\.\d{2}[C]?)$");

            foreach (var line in lines)
            {
                var match = pattern.Match(line.Trim());
                if (match.Success)
                {
                    activities.Add(new CardActivities
                    {
                        TransactionDate = match.Groups[1].Value,
                        PostDate = match.Groups[2].Value,
                        Description = match.Groups[3].Value.Trim(),
                        Amount = match.Groups[4].Value.Replace("C", "").Trim() // Remove 'C' for credit reversal
                    });
                }
            }

            return activities;
        }

        public List<CardActivities> GetCardActivities(string[] lines)
        {
            var activities = new List<CardActivities>();

            if (lines == null || lines.Length < 4)
                return activities;

            try
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].Replace("  ", " "); // Normalize spacing

                    if (line.StartsWith("Installment ", StringComparison.InvariantCultureIgnoreCase))
                    {
                        for (int j = i + 1; j < lines.Length; j++)
                        {
                            line = lines[j].Replace("  ", " ");
                            line = line.Replace("=:", ":");
                            line = line.Replace("??? :", " :");

                            if (line.StartsWith("S.I.P", StringComparison.InvariantCultureIgnoreCase))
                                break;

                            var tokens = line.Split(' ').ToList();

                            if (tokens.Count < 6)
                                continue; // Skip malformed lines

                            var activity = new CardActivities();

                            // Get Transaction Date (first 2 tokens)
                            activity.TransactionDate = $"{tokens[0]} {tokens[1]}";
                            tokens.RemoveAt(0);
                            tokens.RemoveAt(0);

                            // Get Post Date (next 2 tokens)
                            activity.PostDate = $"{tokens[0]} {tokens[1]}";
                            tokens.RemoveAt(0);
                            tokens.RemoveAt(0);

                            // Get Amount (last token)
                            activity.Amount = tokens[^1];
                            tokens.RemoveAt(tokens.Count - 1);

                            // Remaining tokens form the Description
                            activity.Description = string.Join(" ", tokens);

                            activities.Add(activity);
                        }

                        break; // Exit outer loop after processing activities
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing card activities: {ex.Message}");
            }

            return activities;
        }

    }
}
