using HiveCard.PdfParser.Interfaces;
using HiveCard.PdfParser.Models;
using HiveCard.PdfParser.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveCard.PdfParser.Extractors;


namespace HiveCard.PdfParser.Extractors
{
    internal class BPIExt : ExtractorHelper, IExtractor
    {
        #region NOTE: THESE VALUES ARE SUBJECT TO CHANGE
        private int[] _accSummaryCoordinates = new int[] { 1050, 460, 0, 0, 1050, 460, 1340, 530, 1050, 460 };
        private int[] _breakDownListCoordinates = new int[] { 2060, 2300, 0, 0, 2060, 2300, 200, 940, 2060, 2300 };
        private int _skipPage = 2;
        #endregion

        List<string> _imageNames;
        private string _filePath;
        private string _password;
        private CreditCardAccount _bankStatement;

        public event ProcessCompletedEventHandler ProcessCompleted;

        public BPIExt(string filePath, string password)
        {
            _filePath = filePath;
            _password = password;
        }

        public async void BeginExtraction()
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
                int[] _skipPages = new int[] { 2, totalPages-- }; // skip second page and last page,
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
                    if (AllTxt.Length >= 5)
                    {
                        _bankStatement.CardNumber = GetAccountNumber(AllTxt[0]) ?? "";
                        _bankStatement.StatementDate = DateTime.Parse(GetStatementDate(AllTxt[1]) ?? "January 1, 2000");
                        _bankStatement.DueDate = DateTime.Parse(GetPaymentDueDate(AllTxt[2]) ?? "January 1, 2000");
                        _bankStatement.TotalAmountDue = Decimal.Parse(GetTotalAmount(AllTxt[4]) ?? "0.00");
                        _bankStatement.MinimumAmountDue = Decimal.Parse(GetTotalAmount(AllTxt[5]) ?? "0.00");
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
                    for (int i = 0; i < str.Length; i++) // always start with index 2 
                    {
                        string act = str[i];
                        act = act.Replace("  ", " "); // remove all double spacing

                        // still not sure about this, needs to be reviewed carefully
                        if (act.StartsWith("Installment ", StringComparison.InvariantCultureIgnoreCase))
                        {
                            for (int j = i + 1; j < str.Length; j++)
                            {
                                TransactionDetail ca = new TransactionDetail();
                                act = str[j];
                                act = act.Replace("  ", " "); // remove all double spacing
                                act = act.Replace("=:", ":"); // unsual characters
                                act = act.Replace("??? :", " :"); // unsual characters
                                if (act.StartsWith("S.I.P", StringComparison.InvariantCultureIgnoreCase))
                                    break;
                                var arr = act.Split(' ').ToList();

                                #region GET TRANSACTION DATE
                                ca.TransactionDate = DateTime.Parse(string.Join(" ", arr[0], arr[1]));
                                arr.RemoveAt(1);
                                arr.RemoveAt(0);
                                #endregion

                                #region GET TRANSACTION DATE
                                ca.PostingDate = DateTime.Parse(string.Join(" ", arr[0], arr[1]));
                                arr.RemoveAt(1);
                                arr.RemoveAt(0);
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
                            break;
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
