using HiveCard.PdfParser.Models;
using ImageMagick;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Rectangle = SixLabors.ImageSharp.Rectangle;

namespace HiveCard.PdfParser.Services
{
    public static class ImageCropper
    {
        //private static readonly int[] _accSummaryCoordinates = new int[] { 1050, 460, 0, 0, 1050, 460, 1340, 530, 1050, 460 };
        //private static readonly int[] _breakDownListCoordinates = new int[] { 2060, 2300, 0, 0, 2060, 2300, 200, 940, 2060, 2300 };

        public static List<string> CropImages(List<string> imagePaths, CropArea summaryArea, CropArea transactionArea)
        {
            var croppedPaths = new List<string>();

            for (int i = 0; i < imagePaths.Count; i++)
            {

                var cropArea = (i == 0) ? summaryArea : transactionArea;

                using var image = Image.Load<Rgba32>(imagePaths[i]);

                var rect = new Rectangle(cropArea.X, cropArea.Y, cropArea.Width, cropArea.Height);

                image.Mutate(x => x.Crop(rect));


                string croppedPath = Path.ChangeExtension(imagePaths[i], $".cropped.{i}.png");
                image.Save(croppedPath);
                croppedPaths.Add(croppedPath);

                //int[] coords = (i == 0) ? accountCoor : transacCoor;

                //var croppedPath = CropImage(imagePaths[i], coords);
                //if (croppedPath != null)
                //    croppedPaths.Add(croppedPath);
            }

            return croppedPaths;
        }

        private static string CropImage(string imagePath, int[] arr)
        {
            string croppedPath = Path.Combine(
                Path.GetDirectoryName(imagePath),
                Path.GetFileNameWithoutExtension(imagePath) + "-cropped.png"
            );

            using var image = new MagickImage(imagePath);

            var cropArea = new MagickGeometry(arr[6], arr[7], arr[8], arr[9])
            {
                IgnoreAspectRatio = true
            };

            image.Crop(cropArea);
            image.RePage(); // Clean up canvas metadata after cropping
            image.Write(croppedPath);

            return croppedPath;
        }
    }
}
