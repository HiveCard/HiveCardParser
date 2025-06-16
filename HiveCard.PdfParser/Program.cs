using HiveCard.PdfParser.Interfaces;
using HiveCard.PdfParser.Models;
using HiveCard.PdfParser.Parsers;
using static System.Net.Mime.MediaTypeNames;

class Program
{
    static void Main(string[] args)
    {
        IExtractor extractor = new MetrobankExtractor();
        BankStatement statement  = extractor.Run("ESOA/metrobank.pdf");
    }
}
