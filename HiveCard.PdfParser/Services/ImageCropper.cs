using ImageMagick;
using System.Collections.Generic;
using System.IO;

namespace HiveCard.PdfParser.Services
{
    public static class ImageCropper
    {
        public static List<string> CropImages(List<string> imagePaths)
        {
            var croppedPaths = new List<string>();
            foreach (var path in imagePaths)
            {
                using var image = new MagickImage(path);
                var cropped = image.Clone();
                cropped.Crop(new MagickGeometry(100, 100, 800, 600)); // dummy crop area
                var croppedPath = Path.Combine("Output", Path.GetFileNameWithoutExtension(path) + "-cropped.png");
                cropped.Write(croppedPath);
                croppedPaths.Add(croppedPath);
            }
            return croppedPaths;
        }
    }
}
