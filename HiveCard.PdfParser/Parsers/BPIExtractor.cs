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
using HiveCard.PdfParser.Models;
using HiveCard.PdfParser.Helpers;

namespace HiveCard.PdfParser.Parsers
{
    public class BPIExtractor : ExtractorHelper, IExtractor
    {

        #region NOTE: THESE VALUES ARE SUBJECT TO CHANGE
        private int[] _accSummaryCoordinates = new int[] { 1050, 460, 0, 0, 1050, 460, 1340, 530, 1050, 460 };
        private int[] _breakDownListCoordinates = new int[] { 2060, 2300, 0, 0, 2060, 2300, 200, 940, 2060, 2300 };
        private int _skipPage = 2;
        #endregion


        public BankStatement Run(string pdfPath)
        {
           

            var imagePaths = PdfToImageHelper.ConvertPdfToImages(pdfPath);

            // Define pages to skip if needed
            var skipPages = new List<int> { 1 , imagePaths.Count - 1}; // Example: skip page 0
            var pagesToParse = imagePaths
                .Select((path, index) => new { path, index })
                .Where(x => !skipPages.Contains(x.index))
                .ToList();

            // Crop images
            var croppedPaths = ImageCropper.CropImages(pagesToParse.Select(x => x.path).ToList(), _accSummaryCoordinates, _breakDownListCoordinates);

            var bankStatement = new BankStatement();

            for (int i = 0; i < croppedPaths.Count; i++)
            {
                var croppedImage = croppedPaths[i];
                string text = OcrHelper.ExtractText(croppedImage);
                File.WriteAllText(Path.ChangeExtension(croppedImage, ".txt"), text);

                string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                //Debugging: Uncomment to see the lines being processed
                for (int x = 0; x < lines.Length; x++)
                {
                    Console.WriteLine($"{x}: {lines[x]}");
                }



                if (i == 0 && lines.Length >= 6)
                {
                    // First page - extract summary
                    bankStatement.AccountNumber = GetAccountNumber(lines[10].Trim());
                    bankStatement.StatementDate = GetStatementDate(lines[11].Trim());
                    bankStatement.PaymentDueDate = GetPaymentDueDate(lines[12].Trim());
                    bankStatement.TotalAmount = GetTotalAmount(lines[14].Trim());
                    bankStatement.MinimumAmountDue = GetMinimumAmountDue(lines[16].Trim());

                }
                else
                {
                    // Other pages - extract transactions
                    bankStatement.Activities.AddRange(GetCardActivities(lines));
                }




            }

            // Optional: write JSON output
            File.WriteAllText("Output/extracted.json", JsonConvert.SerializeObject(bankStatement, Formatting.Indented));
            return bankStatement;
        }


        private string GetValueAfter(List<string> lines, string keyword)
        {
            int index = lines.FindIndex(l => l.Equals(keyword, StringComparison.OrdinalIgnoreCase));
            if (index >= 0 && index + 1 < lines.Count)
                return lines[index + 1].Trim();
            return string.Empty;
        }


        private List<ExtractedTransaction> ParseText(string ocrText)
        {
            var transactions = new List<ExtractedTransaction>();
            var lines = ocrText.Split('\n');

            foreach (var line in lines)
            {
                // Example logic: 04/23/2024 MERALCO -1234.56
                var parts = line.Trim().Split(' ', 3);
                if (parts.Length < 3) continue;

                if (DateTime.TryParse(parts[0], out var date) && decimal.TryParse(parts[2], out var amount))
                {
                    transactions.Add(new ExtractedTransaction
                    {
                        Date = parts[0],
                        Description = parts[1],
                        Amount = amount
                    });
                }
            }

            return transactions;
        }

        //private string GetAccountNumber(string str) => CommonExtract(str, 2);
        //private string GetStatementDate(string str) => CommonExtract(str, 2);
        //private string GetPaymentDueDate(string str) => CommonExtract(str, 3);
        //private string GetTotalAmount(string str) => CommonExtract(str, 3);
        //private string GetMinimumAmountDue(string str) => CommonExtract(str, 3);
        private string GetAccountNumber(string str) => str;
        private string GetStatementDate(string str) => str;
        private string GetPaymentDueDate(string str) => str;
        private string GetTotalAmount(string str) => str;
        private string GetMinimumAmountDue(string str) => str;

        //private string CommonExtract(string str, int numSpace)
        //{
        //    if (!string.IsNullOrEmpty(str))
        //    {
        //        try
        //        {
        //            str = str.Replace("  ", " ");
        //            var token = str.Split(' ');
        //            var tmp = token.ToList();
        //            for (int i = numSpace - 1; i >= 0 && i < tmp.Count; i--)
        //                tmp.RemoveAt(i);
        //            return string.Join(" ", tmp);
        //        }
        //        catch { }
        //    }
        //    return str;
        //}

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

        private Dictionary<string, string> ExtractLabeledValues(string[] lines)
        {
            var result = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            var labels = new[] {
        "CUSTOMER NUMBER", "STATEMENT DATE", "PAYMENT DUE DATE",
        "CREDIT LIMIT", "TOTAL AMOUNT DUE", "MINIMUM AMOUNT DUE"
    };

            for (int i = 0; i < lines.Length - 1; i++)
            {
                var line = lines[i].Trim().ToUpperInvariant();
                if (labels.Contains(line))
                {
                    var nextLine = lines[i + 1].Trim();
                    result[line] = nextLine;
                }
            }

            return result;
        }
    }

}
