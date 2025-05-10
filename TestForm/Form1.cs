using PdfExtractor;
using PdfExtractor.Utilities;
using PdfExtractor.Model;
using System.Windows;
using System.Threading;

namespace TestForm
{
    public partial class Form1 : Form
    {
        Thread thread = null;

        public Form1()
        {
            InitializeComponent();
            thread = Thread.CurrentThread;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedItem = null;
            comboBox1.SelectedText = "--Select Bank--";
            lblProgress.Text = "";
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            openFileDialog1.AddExtension = true;
            openFileDialog1.Filter = "PDF files (*.pdf)|*.pdf";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.tboxURL.Text = openFileDialog1.FileName;
            }
        }

        private void tboxURL_TextChanged(object sender, EventArgs e)
        {
            ValueChanged();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValueChanged();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
            tboxURL.Text = "";
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                Clear();
                CleanupDirectory(this.tboxURL.Text);

                if (File.Exists(this.tboxURL.Text))
                {
                    this.lblProgress.Text = "Extraction In Progress...";
                    var extFactory = ExtractorFactory.Create(this.tboxURL.Text, txtPassword.Text, (Bank)this.comboBox1.SelectedIndex);
                    extFactory.ProcessCompleted += ExtFactory_ProcessCompleted;
                    extFactory.BeginExtraction();
                }
                else
                    MessageBox.Show("File not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExtFactory_ProcessCompleted(object sender, BankStatement e)
        {
            UIHelper.RunOnUIThread(this, () =>
            {
                txtCustomerNumber.Text = e.AccountNumber;
                txtStatementDate.Text = e.StatementDate;
                txtPaymentDueDate.Text = e.PaymentDueDate;
                txtTotalAmountDue.Text = e.TotalAmount;
                txtMinimumAmountDue.Text = e.MinimumAmountDue;

                foreach (var act in e.Activities)
                {
                    dataGridView1.Rows.Add(act.TransactionDate, act.PostDate, act.Description, act.Amount);
                }

                this.lblProgress.Text = "Done...";
            });
        }

        private void Clear()
        {
            this.txtCustomerNumber.Text = "";
            this.txtMinimumAmountDue.Text = "";
            this.txtPaymentDueDate.Text = "";
            this.txtStatementDate.Text = "";
            this.txtTotalAmountDue.Text = "";
            this.lblProgress.Text = "";
            this.dataGridView1.Rows.Clear();
        }

        private void CleanupDirectory(string target_dir)
        {
            target_dir = target_dir.getFilePath() + target_dir.getFileBaseName() + "\\";

            if (!Directory.Exists(target_dir))
            {
                Directory.CreateDirectory(target_dir);
                return;
            }

            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                CleanupDirectory(dir);
            }
        }

        private void ValueChanged()
        {
            if (!string.IsNullOrEmpty(this.tboxURL.Text) && comboBox1.SelectedIndex != -1)
                btnSubmit.Enabled = true;
            else
                btnSubmit.Enabled = false;
        }
    }
}
