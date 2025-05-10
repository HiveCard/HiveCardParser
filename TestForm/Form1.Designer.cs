namespace TestForm
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnUpload = new Button();
            btnSubmit = new Button();
            btnClear = new Button();
            tboxURL = new TextBox();
            comboBox1 = new ComboBox();
            openFileDialog1 = new OpenFileDialog();
            tboxPassword = new TextBox();
            txtPassword = new Label();
            lblProgress = new Label();
            dataGridView1 = new DataGridView();
            transDate = new DataGridViewTextBoxColumn();
            postDate = new DataGridViewTextBoxColumn();
            description = new DataGridViewTextBoxColumn();
            amount = new DataGridViewTextBoxColumn();
            groupBox1 = new GroupBox();
            txtMinimumAmountDue = new TextBox();
            txtTotalAmountDue = new TextBox();
            txtPaymentDueDate = new TextBox();
            txtStatementDate = new TextBox();
            txtCustomerNumber = new TextBox();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // btnUpload
            // 
            btnUpload.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUpload.Location = new Point(632, 12);
            btnUpload.Name = "btnUpload";
            btnUpload.Size = new Size(75, 23);
            btnUpload.TabIndex = 0;
            btnUpload.Text = "Upload";
            btnUpload.UseVisualStyleBackColor = true;
            btnUpload.Click += btnUpload_Click;
            // 
            // btnSubmit
            // 
            btnSubmit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSubmit.Enabled = false;
            btnSubmit.Location = new Point(713, 13);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new Size(75, 52);
            btnSubmit.TabIndex = 1;
            btnSubmit.Text = "Submit";
            btnSubmit.UseVisualStyleBackColor = true;
            btnSubmit.Click += btnSubmit_Click;
            // 
            // btnClear
            // 
            btnClear.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClear.Location = new Point(632, 41);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(75, 23);
            btnClear.TabIndex = 2;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // tboxURL
            // 
            tboxURL.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tboxURL.Location = new Point(338, 13);
            tboxURL.Name = "tboxURL";
            tboxURL.Size = new Size(288, 23);
            tboxURL.TabIndex = 3;
            tboxURL.TextChanged += tboxURL_TextChanged;
            // 
            // comboBox1
            // 
            comboBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "BPI", "EastWest", "Metrobank" });
            comboBox1.Location = new Point(338, 41);
            comboBox1.Name = "comboBox1";
            comboBox1.RightToLeft = RightToLeft.No;
            comboBox1.Size = new Size(288, 23);
            comboBox1.TabIndex = 4;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // tboxPassword
            // 
            tboxPassword.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tboxPassword.Location = new Point(338, 70);
            tboxPassword.Name = "tboxPassword";
            tboxPassword.Size = new Size(288, 23);
            tboxPassword.TabIndex = 6;
            // 
            // txtPassword
            // 
            txtPassword.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            txtPassword.AutoSize = true;
            txtPassword.Location = new Point(632, 73);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(117, 15);
            txtPassword.TabIndex = 7;
            txtPassword.Text = "Password: (Optional)";
            // 
            // lblProgress
            // 
            lblProgress.AutoSize = true;
            lblProgress.Location = new Point(338, 96);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new Size(135, 15);
            lblProgress.TabIndex = 8;
            lblProgress.Text = "asdfasdfadsfadsfdasfdas";
            // 
            // dataGridView1
            // 
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { transDate, postDate, description, amount });
            dataGridView1.Location = new Point(11, 193);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.Size = new Size(776, 299);
            dataGridView1.TabIndex = 9;
            // 
            // transDate
            // 
            transDate.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            transDate.FillWeight = 203.045685F;
            transDate.HeaderText = "Transaction Date";
            transDate.Name = "transDate";
            transDate.ReadOnly = true;
            transDate.Width = 109;
            // 
            // postDate
            // 
            postDate.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            postDate.FillWeight = 142.624268F;
            postDate.HeaderText = "Post Due Date";
            postDate.Name = "postDate";
            postDate.ReadOnly = true;
            postDate.Width = 97;
            // 
            // description
            // 
            description.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            description.FillWeight = 27.1650162F;
            description.HeaderText = "Description";
            description.Name = "description";
            description.ReadOnly = true;
            // 
            // amount
            // 
            amount.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            amount.FillWeight = 27.1650162F;
            amount.HeaderText = "Amount";
            amount.Name = "amount";
            amount.ReadOnly = true;
            amount.Width = 76;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.Control;
            groupBox1.Controls.Add(txtMinimumAmountDue);
            groupBox1.Controls.Add(txtTotalAmountDue);
            groupBox1.Controls.Add(txtPaymentDueDate);
            groupBox1.Controls.Add(txtStatementDate);
            groupBox1.Controls.Add(txtCustomerNumber);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(320, 175);
            groupBox1.TabIndex = 10;
            groupBox1.TabStop = false;
            groupBox1.Text = "Account Summary";
            // 
            // txtMinimumAmountDue
            // 
            txtMinimumAmountDue.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtMinimumAmountDue.Location = new Point(162, 137);
            txtMinimumAmountDue.Name = "txtMinimumAmountDue";
            txtMinimumAmountDue.Size = new Size(148, 23);
            txtMinimumAmountDue.TabIndex = 9;
            // 
            // txtTotalAmountDue
            // 
            txtTotalAmountDue.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtTotalAmountDue.Location = new Point(162, 89);
            txtTotalAmountDue.Name = "txtTotalAmountDue";
            txtTotalAmountDue.Size = new Size(150, 23);
            txtTotalAmountDue.TabIndex = 8;
            // 
            // txtPaymentDueDate
            // 
            txtPaymentDueDate.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPaymentDueDate.Location = new Point(6, 137);
            txtPaymentDueDate.Name = "txtPaymentDueDate";
            txtPaymentDueDate.Size = new Size(150, 23);
            txtPaymentDueDate.TabIndex = 7;
            // 
            // txtStatementDate
            // 
            txtStatementDate.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtStatementDate.Location = new Point(4, 89);
            txtStatementDate.Name = "txtStatementDate";
            txtStatementDate.Size = new Size(150, 23);
            txtStatementDate.TabIndex = 6;
            // 
            // txtCustomerNumber
            // 
            txtCustomerNumber.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtCustomerNumber.Location = new Point(6, 41);
            txtCustomerNumber.Name = "txtCustomerNumber";
            txtCustomerNumber.Size = new Size(304, 23);
            txtCustomerNumber.TabIndex = 5;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label5.Location = new Point(159, 119);
            label5.Name = "label5";
            label5.Size = new Size(137, 15);
            label5.TabIndex = 4;
            label5.Text = "Minimum Amount Due:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label4.Location = new Point(162, 71);
            label4.Name = "label4";
            label4.Size = new Size(111, 15);
            label4.TabIndex = 3;
            label4.Text = "Total Amount Due:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label3.Location = new Point(6, 119);
            label3.Name = "label3";
            label3.Size = new Size(115, 15);
            label3.TabIndex = 2;
            label3.Text = "Payment Due Date:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label2.Location = new Point(6, 71);
            label2.Name = "label2";
            label2.Size = new Size(100, 15);
            label2.TabIndex = 1;
            label2.Text = "Statement Date:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(8, 23);
            label1.Name = "label1";
            label1.Size = new Size(113, 15);
            label1.TabIndex = 0;
            label1.Text = "Customer Number:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 504);
            Controls.Add(groupBox1);
            Controls.Add(dataGridView1);
            Controls.Add(lblProgress);
            Controls.Add(txtPassword);
            Controls.Add(tboxPassword);
            Controls.Add(comboBox1);
            Controls.Add(tboxURL);
            Controls.Add(btnClear);
            Controls.Add(btnSubmit);
            Controls.Add(btnUpload);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "HiveCard PDF Parser";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnUpload;
        private Button btnSubmit;
        private Button btnClear;
        private TextBox tboxURL;
        private ComboBox comboBox1;
        private OpenFileDialog openFileDialog1;
        private TextBox tboxPassword;
        private Label txtPassword;
        private Label lblProgress;
        private DataGridView dataGridView1;
        private GroupBox groupBox1;
        private TextBox txtCustomerNumber;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private TextBox txtMinimumAmountDue;
        private TextBox txtTotalAmountDue;
        private TextBox txtPaymentDueDate;
        private TextBox txtStatementDate;
        private DataGridViewTextBoxColumn transDate;
        private DataGridViewTextBoxColumn postDate;
        private DataGridViewTextBoxColumn description;
        private DataGridViewTextBoxColumn amount;
    }
}
