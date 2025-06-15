using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
}
