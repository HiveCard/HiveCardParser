using HiveCard.PdfParser.Parsers;

class Program
{
    static void Main(string[] args)
    {
        var parser = new MyBankOcrParser();
        parser.Run("ESOA/sample.pdf");
    }
}
