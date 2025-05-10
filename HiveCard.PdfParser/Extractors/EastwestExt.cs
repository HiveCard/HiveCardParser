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
{
    public class EastWestExt : ExtractorHelper, IExtractor
    {
        #region NOTE: THESE VALUES ARE SUBJECT TO CHANGE
        private int[] _accSummaryCoordinates = new int[] { 850, 600, 0, 0, 850, 600, 1160, 360, 850, 600 };
        private int[] _breakDownListCoordinates = new int[] { 1850, 1550, 0, 0, 1850, 1550, 175, 570, 1850, 1550 };
        #endregion

        List<string> _imageNames;
        private string _filePath;
        private string _password;
        private CreditCardAccount _bankStatement;
        private List<TransactionDetail> _activities;

        public event ProcessCompletedEventHandler ProcessCompleted;

        public EastWestExt(string filePath, string password)
        {
            _filePath = filePath;
            _password = password;
            _bankStatement = new CreditCardAccount();
            _activities = new List<TransactionDetail>();
        }

        public CreditCardAccount Result
        {
            get
            {
                if (_bankStatement != null)
                    return _bankStatement;
                return null;
            }
        }

        public async void BeginExtraction()
        {
            Initialize();
        }

        private async Task Initialize()
        {
            try
            {
                int totalPages = GetPDFTotalPages(_filePath);
                // skip the last two pages, still need to review if still needed to remove the third image from last page.
                int[] _skipPages = new int[] { totalPages, totalPages - 1, totalPages - 2 };
                _imageNames = await ConvertPdfToImage(_filePath, _password, _skipPages);
                CropImages(_imageNames, _accSummaryCoordinates, _breakDownListCoordinates);
                ExecuteTesseractOCR(_imageNames, _filePath.getFilePath() + _filePath.getFileBaseName(), 6);
                GenerateResult();
            }
            catch (Exception)
            { }
        }

        private void GenerateResult()
        {
            bool accNumExist = false;
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
                    if (AllTxt.Length >= 10)
                    {
                        _bankStatement.StatementDate = DateTime.Parse(GetStatementDate(AllTxt[0]) ?? "January 1, 2000");
                        _bankStatement.DueDate = DateTime.Parse(GetPaymentDueDate(AllTxt[1]) ?? "January 1, 2000");
                        _bankStatement.TotalAmountDue = Decimal.Parse(GetTotalAmount(AllTxt[9]) ?? "0.00");
                        _bankStatement.MinimumAmountDue = Decimal.Parse(GetTotalAmount(AllTxt[10]) ?? "0.00");
                    }
                }
                else
                {
                    if (!accNumExist)
                    {
                        _bankStatement.CardNumber = GetAccountNumber(AllTxt[1]) ?? "";
                        accNumExist = true;
                    }

                    _bankStatement.Details.AddRange(GetCardActivities(AllTxt));
                }
            }

            // Raise the event
            OnProcessCompleted(_bankStatement);
        }

        public string GetAccountNumber(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    // Remove all spacing as we only need to get the account number
                    str = str.Replace(" ", "");
                    for (int i = 0; i < str.Length; i++)
                    {
                        char c = str[i];
                        if (Int32.TryParse(c.ToString(), out _))
                            return str.Substring(i, str.Length - i);
                    }
                }
                catch (Exception)
                { }
            }
            return str;
        }

        public string GetStatementDate(string str)
        {
            return CommonExtract(str, 2);
        }

        public string GetPaymentDueDate(string str)
        {
            return CommonExtract(str, 3);
        }

        public string GetTotalAmount(string str)
        {
            return CommonExtract(str, 3);
        }

        public string GetMinimumAmountDue(string str)
        {
            return CommonExtract(str, 3);
        }

        public List<TransactionDetail> GetCardActivities(string[] str)
        {
            List<TransactionDetail> activities = new List<TransactionDetail>();
            if (str != null && str.Length > 3)
            {
                try
                {
                    Regex r1 = new Regex(@"[a-zA-Z]{3,}\s[0-9]{1,2}");
                    Regex r2 = new Regex(@"[a-zA-Z]{3,}[0-9]{1,2}");
                    for (int i = 0; i < str.Length; i++)
                    {
                        TransactionDetail ca = new TransactionDetail();
                        string act = str[i];

                        act = act.Replace("  ", " "); // remove all double spacing
                        if (act.StartsWith("Total", StringComparison.InvariantCultureIgnoreCase))
                            break;

                        // will be used for validation
                        string dateValid1 = act.Substring(0, 5);
                        string dateValid2 = act.Substring(0, 6);

                        if ((r1.IsMatch(dateValid1) || r1.IsMatch(dateValid2)) ||
                            (r2.IsMatch(dateValid2) || r2.IsMatch(dateValid2)))
                        {
                            var arr = act.Split(' ').ToList();

                            #region GET TRANSACTION DATE
                            if (r2.IsMatch(arr[0]))
                            {
                                ca.TransactionDate = DateTime.Parse(arr[0]);
                                arr.RemoveAt(0);
                            }
                            else
                            {
                                ca.TransactionDate = DateTime.Parse(string.Join(" ", arr[0], arr[1]));
                                arr.RemoveAt(1);
                                arr.RemoveAt(0);
                            }
                            #endregion

                            #region GET POST DATE
                            if (r2.IsMatch(arr[0]))
                            {
                                ca.PostingDate = DateTime.Parse(arr[0]);
                                arr.RemoveAt(0);
                            }
                            else
                            {
                                ca.PostingDate = DateTime.Parse(string.Join(" ", arr[0], arr[1]));
                                arr.RemoveAt(1);
                                arr.RemoveAt(0);
                            }
                            #endregion

                            #region GET AMOUNT
                            ca.Amount = Decimal.Parse(arr.Last());
                            arr.RemoveAt(arr.Count() - 1);
                            #endregion

                            #region GET DESCRIPTION
                            ca.Description = string.Join(" ", arr.ToArray());
                            #endregion


                            activities.Add(ca);
                        }
                    }
                }
                catch (Exception)
                { }
            }
            return activities;
        }

        public virtual void OnProcessCompleted(CreditCardAccount e)
        {
            ProcessCompleted?.Invoke(this, e);
        }
    }
}
