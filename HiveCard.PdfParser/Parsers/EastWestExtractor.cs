using HiveCard.PdfParser.Helpers;
using HiveCard.PdfParser.Models;
using HiveCard.PdfParser.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HiveCard.PdfParser.Parsers
{
    public class EastWestExtractor : ExtractorHelper, IExtractor
    {
        #region NOTE: THESE VALUES ARE SUBJECT TO CHANGE
        private int[] _accSummaryCoordinates = new int[] { 850, 600, 0, 0, 850, 600, 1160, 360, 850, 600 };
        private int[] _breakDownListCoordinates = new int[] { 1850, 1550, 0, 0, 1850, 1550, 175, 570, 1850, 1550 };
        #endregion


        public BankStatement Run(string pdfPath)
        {
            var imagePaths = PdfToImageHelper.ConvertPdfToImages(pdfPath);


            // Define exactly which pages you want to parse (0-based index)
            var pagesToScan = new List<int> { 0, 1 };

           // var skipPages = new List<int> { imagePaths.Count - 1, imagePaths.Count -2 }; // Example: skip page 0

            var pagesToParse = imagePaths
                    .Select((path, index) => new { path, index })
                    //.Where(x => !skipPages.Contains(x.index))
                    .Where(x => pagesToScan.Contains(x.index))
                    .ToList();


            // Crop images
            var croppedPaths = ImageCropper.CropImages(pagesToParse.Select(x => x.path).ToList(), _accSummaryCoordinates, _breakDownListCoordinates);

            var bankStatement = new BankStatement();

            for (int i = 0; i < croppedPaths.Count; i++)
            {
                var croppedImage = croppedPaths[i];
                var text = OcrHelper.ExtractText(croppedImage);
                File.WriteAllText(Path.ChangeExtension(croppedImage, ".txt"), text);

                //var lines = text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None)
                //                .Where(l => !string.IsNullOrWhiteSpace(l))
                //                .ToList();

                string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                //Debugging: Uncomment to see the lines being processed
                for (int x = 0; x < lines.Length; x++)
                {
                    Console.WriteLine($"{x}: {lines[x]}");
                }


                if (i == 0)
                {
                    // First page - extract summary
                  //  bankStatement.AccountNumber = GetAccountNumber(lines[38].Trim());
                    bankStatement.StatementDate = GetStatementDate(lines[0].Trim());
                    bankStatement.PaymentDueDate = GetPaymentDueDate(lines[2].Trim());
                    bankStatement.TotalAmount = GetTotalAmount(lines[10].Trim());
                    bankStatement.MinimumAmountDue = GetMinimumAmountDue(lines[12].Trim());
                }
                else
                {
                    bankStatement.Activities.AddRange(GetCardActivities(lines));
                }
            }
            // Optional: write JSON output
            File.WriteAllText("Output/extracted.json", JsonConvert.SerializeObject(bankStatement, Formatting.Indented));
            return bankStatement;
        }

        private string GetAccountNumber(string str) => str;
        private string GetStatementDate(string str) => str;
        private string GetPaymentDueDate(string str) => str;
        private string GetTotalAmount(string str) => str;
        private string GetMinimumAmountDue(string str) => str;

        private List<CardActivities> GetCardActivities(string[] rawText)
        {
            var lines = rawText
             //.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)
             .Where(l => !string.IsNullOrWhiteSpace(l))
             .ToList();

            // Step 1: Determine how many transaction lines we have (e.g., 17)
            int sectionSize = 17;

            var transactionDates = lines.Take(sectionSize).ToList();
            var postDates = lines.Skip(sectionSize).Take(sectionSize).ToList();
            var descriptions = lines.Skip(sectionSize * 2).Take(sectionSize).ToList();
            var amounts = lines.Skip(sectionSize * 3).Take(sectionSize).ToList();

            List<CardActivities> activities = new();

            for (int i = 0; i < sectionSize; i++)
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
    }
}
