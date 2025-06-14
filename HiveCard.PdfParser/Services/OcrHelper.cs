using System;
using Tesseract;

namespace HiveCard.PdfParser.Services
{
    public static class OcrHelper
    {
        public static string ExtractText(string imagePath)
        {
            using var engine = new TesseractEngine("./Tesseract/tessdata", "eng", EngineMode.Default);
            using var img = Pix.LoadFromFile(imagePath);
            using var page = engine.Process(img);
            return page.GetText();
        }
    }
}
