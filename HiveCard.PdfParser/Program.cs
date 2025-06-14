using HiveCard.PdfParser.Parsers;

class Program
{
    static void Main(string[] args)
    {
        IExtractor extractor = new BPIExtractor();
        extractor.Run("ESOA/sample.pdf");
    }
}
