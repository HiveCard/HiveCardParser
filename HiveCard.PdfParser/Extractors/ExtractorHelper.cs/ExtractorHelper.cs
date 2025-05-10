using HiveCard.PdfParser.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;

namespace HiveCard.PdfParser.Extractors
{
    [SupportedOSPlatform("windows")]
    public class ExtractorHelper
    {
        protected async Task<List<string>> ConvertPdfToImage(string fileName, string password, int[] skipPages)
        {
            List<string> ImageNames = new List<string>();
            try
            {
                // Convert PDF to Image
                int count = 1;
                byte[] bytes = await File.ReadAllBytesAsync(fileName);
                string pdfBase64 = Convert.ToBase64String(bytes);
                await foreach (var bitmap in PDFtoImage.Conversion.ToImagesAsync(pdfBase64, Range.All, password))
                {
                    if (skipPages.Contains(count))
                    {
                        count++;
                        continue;
                    }

                    using var encodedImage = new MemoryStream();
                    bitmap.Encode(encodedImage, SkiaSharp.SKEncodedImageFormat.Png, 100);

                    byte[] byteArray = encodedImage.ToArray();
                    Image img = Image.FromStream(new MemoryStream(byteArray));

                    // Generate temporary filename for the image
                    string dir = fileName.getFilePath() + fileName.getFileBaseName() + "\\";
                    string imageFilename = string.Format("{0}{1}_{2}.png", dir, fileName.getFileBaseName(), count);

                    // Save Image
                    img.Save(imageFilename);
                    ImageNames.Add(imageFilename);
                    count++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ImageNames;
        }

        protected void ExecuteTesseractOCR(List<string> imageNames, string dir, int psm)
        {
            if (imageNames != null && imageNames.Count() > 0)
            {
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\windows\system32\cmd.exe",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                p.Start();

                // Generate OCR Result for each Image
                using (StreamWriter sw = p.StandardInput)
                {
                    sw.WriteLine(string.Format("cd {0}", dir));

                    for (int i = 1; i <= imageNames.Count(); i++)
                        sw.WriteLine(string.Format("tesseract {0} {1} --psm {2}", imageNames[i - 1].getFileName(), imageNames[i - 1].getFileBaseName(), psm));
                }
                p.WaitForExit();
            }
        }

        protected void CropImage(string fullFileName, int[] arr)
        {
            string croppedName;
            // Create a new image at the cropped size
            Bitmap cropped = new Bitmap(arr[0], arr[1]);

            //Load image from file
            using (Image image = Image.FromFile(fullFileName))
            {
                // Create a Graphics object to do the drawing, *with the new bitmap as the target*
                using (Graphics g = Graphics.FromImage(cropped))
                {
                    g.DrawImage(image, new Rectangle(arr[2], arr[3], arr[4], arr[5]), new Rectangle(arr[6], arr[7], arr[8], arr[9]), GraphicsUnit.Pixel);

                    croppedName = fullFileName.getFilePath() + "cropped_" + fullFileName.getFileBaseName() + ".png";
                    cropped.Save(croppedName);
                }
            }

            if (File.Exists(fullFileName))
            {
                File.Delete(fullFileName);
                File.Move(croppedName, fullFileName);
            }
        }

        protected void CropImages(List<string> imageNames, int[] summaryCoordinates, int[] activityCoordinates)
        {
            for (int i = 0; i < imageNames.Count; i++)
            {
                if (i == 0)
                {
                    CropImage(imageNames[i], summaryCoordinates); // Crop the part of Account Summary
                }
                else
                {
                    CropImage(imageNames[i], activityCoordinates); // Crop the part of Account Summary
                }
            }
        }

        /// <summary>
        /// Gets the total pages of the pdf
        /// </summary>
        protected int GetPDFTotalPages(string filePath)
        {
            using (PdfDocument document = PdfDocument.Open(filePath))
            {
                return document.GetPages().Count();
            }
        }

        protected string CommonExtract(string str, int numSpace)
        {
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    str = str.Replace("  ", " ");// remove double spacing just in case...
                    var token = str.Split(' ');
                    var tmp = token.ToList();
                    for (int i = numSpace - 1; i >= 0; i--)
                        tmp.RemoveAt(i);
                    str = string.Join(" ", tmp.ToArray());
                    return str;
                }
                catch (Exception)
                { }
            }
            return str;
        }
    }
}
