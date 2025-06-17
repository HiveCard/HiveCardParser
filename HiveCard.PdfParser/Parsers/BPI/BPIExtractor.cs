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
using HiveCard.PdfParser.Interfaces;

namespace HiveCard.PdfParser.Parsers.BPI
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
            var summaryCrop = new CropArea(1338, 540, 1046, 444);
            var detailsCrop = new CropArea(130, 497, 2360, 2129);

            var imagePaths = PdfToImageHelper.ConvertPdfToImages(pdfPath);


            var pagesToScan = new List<int> { 0, 2 };

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
                string text = OcrHelper.ExtractText(croppedImage);
                File.WriteAllText(Path.ChangeExtension(croppedImage, ".txt"), text);

                string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);


                var sections = ParserUtils.SplitIntoSections(lines, line => line.Contains("Statement of Account", StringComparison.OrdinalIgnoreCase));
                ////Debugging: Uncomment to see the lines being processed
                //for (int x = 0; x < lines.Length; x++)
                //{
                //    Console.WriteLine($"{x}: {lines[x]}");
                //}


                if (i == 0 && sections.HeaderLines.Count >= 12)
                {
                    var cleaned = sections.HeaderLines
                        .Where(l => !string.IsNullOrWhiteSpace(l))
                        .Select(l => l.Trim())
                        .ToList();

                    bankStatement.StatementDate = cleaned[7];  // MARCH 27, 2025
                    bankStatement.PaymentDueDate = cleaned[8];  // APRIL 16, 2025
                    bankStatement.TotalAmount = cleaned[10]; // 149,838.74
                    bankStatement.MinimumAmountDue = cleaned[11]; // 5,351.38
                }
                else
                {
                    // Other pages - extract transactions
                   bankStatement.AccountNumber = GetAccountNumber(sections.TransactionLines);
                    bankStatement.Activities.AddRange(GetCardActivities(sections.TransactionLines));
                }




            }

            // Optional: write JSON output
            File.WriteAllText("Output/bpi_extracted.json", JsonConvert.SerializeObject(bankStatement, Formatting.Indented));
            return bankStatement;
        }


        private string GetValueAfterLabel(List<string> lines, string label)
        {
            var cleaned = lines
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => l.Trim())
                .ToList();

            for (int i = 0; i < cleaned.Count - 1; i++)
            {
                if (cleaned[i].Equals(label, StringComparison.OrdinalIgnoreCase))
                {
                    return cleaned[i + 1];
                }
            }

            return string.Empty;
        }



        private string GetAccountNumber(List<string> lines)
        {
            // Match format: ######-#-##-########
            var pattern = new Regex(@"\b(\d{6})-(\d)-(\d{2})-(\d{7})\b");

            foreach (var line in lines)
            {
                var match = pattern.Match(line);
                if (match.Success)
                {
                    // Combine groups into a 16-digit credit card style format
                    string rawDigits = $"{match.Groups[1].Value}{match.Groups[2].Value}{match.Groups[3].Value}{match.Groups[4].Value}";
                    return Regex.Replace(rawDigits, @"(.{4})", "$1 ").Trim();  // e.g. 4608 8878 0524 7200
                }
            }

            return string.Empty;
        }

        private string GetStatementDate(List<string> headerLines)
        {
            return ParserUtils.GetValueAfterLabel(headerLines, "STATEMENT DATE");
        }

        private string GetPaymentDueDate(List<string> headerLines)
        {
            return ParserUtils.GetValueAfterLabel(headerLines, "PAYMENT DUE DATE");
        }

        private string GetTotalAmountDue(List<string> headerLines)
        {
            return ParserUtils.GetValueAfterLabel(headerLines, "TOTAL AMOUNT DUE");
        }

        private string GetMinimumAmountDue(List<string> headerLines)
        {
            return ParserUtils.GetValueAfterLabel(headerLines, "MINIMUM AMOUNT DUE");
        }

        private List<CardActivities> GetCardActivities(List<string> lines)
        {
            var activities = new List<CardActivities>();

            // Matches BPI style: "March 27 March 27 Description 9,041.42"
            var pattern = new Regex(
                @"^([A-Za-z]+ \d{1,2})\s+([A-Za-z]+ \d{1,2})\s+(.+?)\s+([-]?\d{1,3}(?:,\d{3})*\.\d{2})$",
                RegexOptions.Compiled);

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
                        Amount = match.Groups[4].Value.Trim()
                    });
                }
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
