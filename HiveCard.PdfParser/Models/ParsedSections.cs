using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveCard.PdfParser.Models
{
    public class ParsedSections
    {
        public List<string> HeaderLines { get; set; } = new();
        public List<string> TransactionLines { get; set; } = new();
        public List<string> FooterLines { get; set; } = new();
        public List<string> RawLines { get; set; } = new();
    }
}
