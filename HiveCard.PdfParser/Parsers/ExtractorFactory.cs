using HiveCard.PdfParser.Interfaces;
using HiveCard.PdfParser.Parsers.BPI;
using HiveCard.PdfParser.Parsers.Eastwest;
using HiveCard.PdfParser.Parsers.Metrobank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveCard.PdfParser.Parsers
{
    public static class ExtractorFactory
    {
        public static IExtractor GetExtractor(string bankName)
        {
            return bankName switch
            {
                "BPI"       => new BPIExtractor(),
                "EastWest"  => new EastWestExtractor(),
                "Metrobank" => new MetrobankExtractor(),
                _           => throw new NotSupportedException("Bank not supported")
            };
        }
    }
}
