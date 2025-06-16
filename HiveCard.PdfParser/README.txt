# HiveCard.PdfParser (OCR Version)

## Requirements
- .NET 8.0
- Ghostscript installed (needed by Magick.NET to read PDFs)
- Tesseract trained data (`eng.traineddata`) placed in: `Tesseract/tessdata/`

## How to Run
1. Put a scanned PDF in the root directory as `sample.pdf`
2. Run the project
3. OCR results will be saved in `Output/` folder:
   - Full page images
   - Cropped images
   - OCR text files
   - Final extracted.json (empty for now)

## NuGet Packages Required
- Tesseract
- Magick.NET-Q16-AnyCPU
- Newtonsoft.Json




HiveCard.PdfParser
*.Net 8.0 Framework
*Tesseract 5.20
*Magick.Net-Q16-AnyCPU 13.6.0
*ghostscript 10.05.1 (https://ghostscript.com/releases/gsdnld.html)
*Newtonsoft.Json 13.0.3
*eng.traineddata (tesseract/tessdata/) from https://github.com/tesseract-ocr/tessdata
*SixLabors.ImageSharp 3.1.10
