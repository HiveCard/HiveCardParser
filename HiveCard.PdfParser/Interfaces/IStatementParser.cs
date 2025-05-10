using System.Collections.Generic;
using HiveCard.PdfParser.Models;

namespace HiveCard.PdfParser.Interfaces
{
    /// <summary>
    /// Defines a parser that can extract a full statement result from a PDF file.
    /// </summary>
    public interface IStatementParser
    {
        /// <summary>
        /// Parses the given credit-card statement PDF and returns all extracted data
        /// (account info, transactions, installments) in a single result object.
        /// </summary>
        /// <param name="pdfPath">Local file path to the PDF to parse.</param>
        /// <returns>A <see cref="StatementResult"/> containing parsed fields and lists.</returns>
        StatementResult ParseResult(string pdfPath);
    }
}
