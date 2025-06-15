using ImageMagick;
using System.Collections.Generic;
using System.IO;

namespace HiveCard.PdfParser.Services
{
    public static class ImageCropper
    {
        //private static readonly int[] _accSummaryCoordinates = new int[] { 1050, 460, 0, 0, 1050, 460, 1340, 530, 1050, 460 };
        //private static readonly int[] _breakDownListCoordinates = new int[] { 2060, 2300, 0, 0, 2060, 2300, 200, 940, 2060, 2300 };

        public static List<string> CropImages(List<string> imagePaths, int[] accountCoor, int[] transacCoor)
        {
            var croppedPaths = new List<string>();

            for (int i = 0; i < imagePaths.Count; i++)
            {
                int[] coords = (i == 0) ? accountCoor : transacCoor;

                var croppedPath = CropImage(imagePaths[i], coords);
                if (croppedPath != null)
                    croppedPaths.Add(croppedPath);
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
