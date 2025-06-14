using ImageMagick;
using System.Collections.Generic;
using System.IO;

namespace HiveCard.PdfParser.Services
{
    public static class PdfToImageHelper
    {
        public static List<string> ConvertPdfToImages(string pdfPath)
        {
            var outputPaths = new List<string>();
            using var images = new MagickImageCollection();
            var settings = new MagickReadSettings { Density = new Density(300, 300) };
            images.Read(pdfPath, settings);

            int page = 1;
            foreach (var image in images)
            {
                Directory.CreateDirectory("Output");
                var filePath = $"Output/page-{page}.png";
                image.Write(filePath);
                outputPaths.Add(filePath);
                page++;
            }

            return outputPaths;
        }
    }
}
