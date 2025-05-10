using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveCard.PdfParser.Utils
{
    public static class CommonUtils
    {
        /// <summary>
        /// Gets filename with extension e.g. (FileName.txt)
        /// </summary>
        public static string getFileName(this string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                string[] token = filename.Split('\\');
                return token.Last();
            }
            return "";
        }

        /// <summary>
        /// Gets filename without extension e.g. (FileName)
        /// </summary>
        public static string getFileBaseName(this string filename)
        {
            return filename.getFileName().Split('.')[0];
        }

        /// <summary>
        /// Gets filename's directory e.g. (C:\Program Files\Folder)
        /// </summary>
        public static string getFilePath(this string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                string[] token = filename.Split('\\');
                if (token.Count() > 1)
                {
                    return filename.Substring(0, filename.LastIndexOf('\\')) + "\\";
                }
            }
            return "";
        }
    }
}
