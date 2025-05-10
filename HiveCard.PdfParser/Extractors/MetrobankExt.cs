using HiveCard.PdfParser.Interfaces;
using HiveCard.PdfParser.Models;
using HiveCard.PdfParser.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HiveCard.PdfParser.Extractors
{    public class MetrobankExt : ExtractorHelper, IExtractor
    {
        #region NOTE: THESE VALUES ARE SUBJECT TO CHANGE
        private int[] _accSummaryCoordinates = new int[] { 900, 600, 0, 0, 900, 600, 1110, 0, 900, 600 };
        private int[] _breakDownListCoordinates = new int[] { 1380, 2500, 0, 0, 1380, 2500, 680, 240, 1380, 2500 };
        #endregion

        List<string> _imageNames;
        private string _filePath;
        private string _password;
        private CreditCardAccount _bankStatement;

        public event ProcessCompletedEventHandler ProcessCompleted;

        public MetrobankExt(string filePath, string password)
        {
            _filePath = filePath;
            _password = password;
        }

        public void BeginExtraction()
        {
            Initialize();
        }

        public CreditCardAccount Result
        {
            get { return _bankStatement; }
        }

        private async void Initialize()
        {
            try
            {
                // Calculate the pages that needs to be skip from image Extraction
                int totalPages = GetPDFTotalPages(_filePath);
                int[] _skipPages = new int[] { totalPages, totalPages-- }; // skip the last two pages,
                _imageNames = await ConvertPdfToImage(_filePath, _password, _skipPages);
                CropImages(_imageNames, _accSummaryCoordinates, _breakDownListCoordinates);
                ExecuteTesseractOCR(_imageNames, _filePath.getFilePath() + _filePath.getFileBaseName(), 6);
                GenerateResult();
            }
            catch (Exception ex)
            { }
        }

        private void GenerateResult()
        {
            _bankStatement = new CreditCardAccount();

            // loop through all the extracted text files
            for (int i = 0; i < _imageNames.Count(); i++)
            {
                string path = _imageNames[i].getFilePath() + "\\" + _imageNames[i].getFileBaseName() + ".txt";
                string[] AllTxt = File.ReadAllLines(path, System.Text.Encoding.ASCII);
                // First page will always contain the Account Summary,
                // but the Account Number will be get from the next page.
                if (i == 0)
                {
                    if (AllTxt.Length >= 9)
                    {
                        AllTxt = AllTxt.Where(x => !string.IsNullOrEmpty(x)).ToArray();  // remove all empty lines
                        _bankStatement.CardNumber = GetAccountNumber(AllTxt[2]) ?? "";
                        _bankStatement.StatementDate = DateTime.Parse(GetStatementDate(AllTxt[7]) ?? "");
                        _bankStatement.DueDate = DateTime.Parse(GetPaymentDueDate(AllTxt[7]) ?? "");
                        _bankStatement.TotalAmountDue = Decimal.Parse(GetTotalAmount(AllTxt[9]) ?? "");
                        _bankStatement.MinimumAmountDue = Decimal.Parse(GetMinimumAmountDue(AllTxt[9]) ?? "");
                    }
                }
                else
                {
                    _bankStatement.Details.AddRange(GetCardActivities(AllTxt));
                }
            }

            // Raise the event
            OnProcessCompleted(_bankStatement);
        }

        public string GetAccountNumber(string str)
        {
            return CommonExtract(str, 2);
        }

        public string GetStatementDate(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    str = str.Replace("  ", " ");// remove double spacing just in case...
                    var token = str.Split(' ');
                    str = String.Join(" ", new string[] { token[0], token[1], token[2] });
                    return str;
                }
                catch (Exception)
                { }
            }
            return str;
        }

        public string GetPaymentDueDate(string str)
        {
            return CommonExtract(str, 3);
        }

        public string GetTotalAmount(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    str = str.Replace("  ", " ");// remove double spacing just in case...
                    var token = str.Split(' ');
                    str = String.Join(" ", new string[] { token[0], token[1] });
                    return str;
                }
                catch (Exception)
                { }
            }
            return str;
        }

        public string GetMinimumAmountDue(string str)
        {
            return CommonExtract(str, 2);
        }

        public List<TransactionDetail> GetCardActivities(string[] str)
        {
            List<TransactionDetail> activities = new List<TransactionDetail>();
            try
            {
                Regex regex = new Regex(@"[0-9]{1,2}(/)[0-9]{1,2}\s");
                for (int i = 0; i < str.Length; i++) // always start with index 2 
                {
                    string act = str[i];
                    act = act.Replace("  ", " "); // remove all double spacing
                    if (regex.IsMatch(act.Split(' ')[0] + " "))
                    {
                        TransactionDetail ac = new TransactionDetail();
                        var arr = act.Split(' ').ToList();

                        #region GET TRANSACTION DATE
                        ac.TransactionDate = DateTime.Parse(arr[0]);
                        arr.RemoveAt(0);
                        #endregion

                        #region GET POST DATE
                        ac.PostingDate = DateTime.Parse((arr[0]));
                        arr.RemoveAt(0);
                        #endregion

                        #region GET AMOUNT DATE
                        ac.Amount = Decimal.Parse(arr.Last());
                        arr.RemoveAt(arr.Count() - 1);
                        #endregion

                        #region GET DESCRIPTION
                        ac.Description = String.Join(" ", arr.ToArray());
                        #endregion

                        activities.Add(ac);
                    }

                    if (act.StartsWith("TOTAL", StringComparison.InvariantCultureIgnoreCase))
                        break;
                }
            }
            catch (Exception)
            { }
            return activities;
        }

        public virtual void OnProcessCompleted(CreditCardAccount e)
        {
            ProcessCompleted?.Invoke(this, e);
        }
    }
}
