using HiveCard.PdfParser.Models;
using HiveCard.PdfParser.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace HiveCard.PdfParser.Parsers
{
    public class MyBankOcrParser
    {
        public void Run(string pdfPath)
        {
            var imagePaths = PdfToImageHelper.ConvertPdfToImages(pdfPath);
            var croppedPaths = ImageCropper.CropImages(imagePaths);
            var allText = new List<string>();
            foreach (var cropped in croppedPaths)
            {
                var text = OcrHelper.ExtractText(cropped);
                var txtPath = Path.ChangeExtension(cropped, ".txt");
                File.WriteAllText(txtPath, text);
                allText.Add(text);
            }

            var transactions = ParseText(allText);
            File.WriteAllText("Output/extracted.json", JsonConvert.SerializeObject(transactions, Formatting.Indented));
        }

        private List<ExtractedTransaction> ParseText(List<string> texts)
        {
            // Dummy logic: Return empty list
            return new List<ExtractedTransaction>();
        }
    }
}
