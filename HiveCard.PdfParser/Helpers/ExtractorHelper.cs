using HiveCard.PdfParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HiveCard.PdfParser.Helpers
{
    public class ExtractorHelper
    {
        protected string CommonExtract(string str, int numSpace)
        {
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    str = str.Replace("  ", " ");
                    var token = str.Split(' ');
                    var tmp = token.ToList();
                    for (int i = numSpace - 1; i >= 0; i--)
                        tmp.RemoveAt(i);
                    return string.Join(" ", tmp.ToArray());
                }
                catch { }
            }
            return str;
        }
    }

    public static class SectionSplitter
    {
        public static ParsedSections SplitIntoSections(string[] lines)
        {
            var result = new ParsedSections();
            bool inTransactions = false;
            bool inFooter = false;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.Contains("END OF STATEMENT", StringComparison.OrdinalIgnoreCase) ||
                    line.Contains("Total Statement Balance", StringComparison.OrdinalIgnoreCase))
                {
                    inFooter = true;
                }

                if (inFooter)
                {
                    result.FooterLines.Add(line);
                    continue;
                }

                // crude detection of start of transaction data
                if (Regex.IsMatch(line, @"^(JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)[ ]?\d{1,2}", RegexOptions.IgnoreCase))
                {
                    inTransactions = true;
                }

                if (inTransactions)
                    result.TransactionLines.Add(line);
                else
                    result.HeaderLines.Add(line);
            }

            return result;
        }
    }
}
