using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace HiveCard.PdfParser.Helpers
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    public static class ParserUtils
    {
        public class ParsedSections
        {
            public List<string> HeaderLines { get; set; } = new();
            public List<string> TransactionLines { get; set; } = new();
            public List<string> FooterLines { get; set; } = new();
            public List<string> RawLines { get; set; } = new();


        }

        public static ParsedSections SplitIntoSections(string[] lines, Func<string, bool> isStartOfTransactionSection)
        {
            var sections = new ParsedSections();
            bool inTransactions = false;


            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                sections.RawLines.Add(trimmed);

                if (!inTransactions && isStartOfTransactionSection(trimmed))
                {
                    inTransactions = true;
                    continue;
                }

                if (!inTransactions)
                    sections.HeaderLines.Add(trimmed);
                else
                    sections.TransactionLines.Add(trimmed);
   
                   
            }

            return sections;
        }


        public static string GetValueAfterLabel(List<string> lines, string label)
        {
            for (int i = 0; i < lines.Count - 1; i++)
            {
                if (lines[i].ToUpper().Contains(label.ToUpper()))
                {
                    var value = lines[i + 1].Trim();
                    if (!string.IsNullOrWhiteSpace(value))
                        return value;
                }
            }

            return string.Empty;
        }
    }
}

