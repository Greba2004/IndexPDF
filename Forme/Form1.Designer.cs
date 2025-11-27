namespace IndexPDF2
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing != null && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        // Javne array-e za lakši pristup iz Form1.cs
        public Label[] PoljaLabel;
        public ComboBox[] PoljaComboBox;

        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblNazivPdfFajla = new System.Windows.Forms.Label();
            this.txtNoviNazivFajla = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label(); // Datum od
            this.label13 = new System.Windows.Forms.Label(); // Datum do
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.comboBox5 = new System.Windows.Forms.ComboBox();
            this.comboBox6 = new System.Windows.Forms.ComboBox();
            this.comboBox7 = new System.Windows.Forms.ComboBox();
            this.comboBox8 = new System.Windows.Forms.ComboBox();
            this.chkMenjasNaziv = new System.Windows.Forms.CheckBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBoxDatumOd = new System.Windows.Forms.TextBox();
            this.textBoxDatumDo = new System.Windows.Forms.TextBox();
            this.btnSacuvaj = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btn_Izvestaj = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnZameniFormu = new System.Windows.Forms.Button();
            this.textBoxPretragaPdf = new System.Windows.Forms.TextBox();
            this.btnNadjiPdf = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();

            // splitContainer1
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Size = new System.Drawing.Size(1096, 706);
            this.splitContainer1.SplitterDistance = 628;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);

            // tableLayoutPanel1
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel1.RowCount = 15;
            for (int i = 0; i < 13; i++)
                this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.672268F));
            for (int i = 13; i < 15; i++)
                this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.13F));
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;

            // Labels
            this.lblNazivPdfFajla.Text = "Naziv PDF fajla";
            this.lblNazivPdfFajla.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.lblNazivPdfFajla.Dock = System.Windows.Forms.DockStyle.Fill;

            this.txtNoviNazivFajla.Text = "Novi naziv PDF fajla";
            this.txtNoviNazivFajla.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.txtNoviNazivFajla.Dock = System.Windows.Forms.DockStyle.Fill;

            Label[] labels = new Label[] { label4, label5, label6, label7, label8, label9, label10, label11 };
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i].Text = "Polje " + (i + 1);
                labels[i].Font = new System.Drawing.Font("Segoe UI", 15F);
                labels[i].ForeColor = System.Drawing.Color.MidnightBlue;
                labels[i].Dock = System.Windows.Forms.DockStyle.Fill;
            }

            this.label12.Text = "Datum od";
            this.label12.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.label12.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label12.Dock = System.Windows.Forms.DockStyle.Fill;

            this.label13.Text = "Datum do";
            this.label13.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.label13.ForeColor = System.Drawing.Color.MidnightBlue;
            this.label13.Dock = System.Windows.Forms.DockStyle.Fill;

            // ComboBox-i
            ComboBox[] cbs = new ComboBox[] { comboBox1, comboBox2, comboBox3, comboBox4, comboBox5, comboBox6, comboBox7, comboBox8 };
            foreach (var cb in cbs)
            {
                cb.Font = new System.Drawing.Font("Segoe UI", 20F);
                cb.Dock = System.Windows.Forms.DockStyle.Fill;
                cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cb.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }

            // Snimi u javne array-e
            this.PoljaLabel = labels;
            this.PoljaComboBox = cbs;

            // CheckBox
            this.chkMenjasNaziv.Text = "Menjas naziv fajla ?";
            this.chkMenjasNaziv.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.chkMenjasNaziv.Dock = System.Windows.Forms.DockStyle.Fill;

            // TextBox
            this.textBox2.Font = new System.Drawing.Font("Segoe UI", 20F);
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDatumOd.Font = new System.Drawing.Font("Segoe UI", 20F);
            this.textBoxDatumOd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDatumOd.PlaceholderText = "dd.MM.yyyy.";
            this.textBoxDatumDo.Font = new System.Drawing.Font("Segoe UI", 20F);
            this.textBoxDatumDo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDatumDo.PlaceholderText = "dd.MM.yyyy.";

            // Buttons
            Button[] buttons = new Button[] { btnSacuvaj, button1, button2, btn_Izvestaj };
            foreach (var btn in buttons)
            {
                btn.BackColor = System.Drawing.Color.MidnightBlue;
                btn.ForeColor = System.Drawing.SystemColors.Control;
                btn.Dock = System.Windows.Forms.DockStyle.Fill;
            }
            btnSacuvaj.Text = "Sacuvaj izmene i idi na sledeci";
            button1.Text = "Predhodni";
            button2.Text = "Idi na sledeci";
            btn_Izvestaj.Text = "Izvestaj";

            // FlowLayoutPanel
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.flowLayoutPanel1.WrapContents = false;
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;

            // btnZameniFormu
            this.btnZameniFormu.Text = "Forma menice";
            this.btnZameniFormu.BackColor = System.Drawing.Color.MidnightBlue;
            this.btnZameniFormu.ForeColor = System.Drawing.Color.White;
            this.btnZameniFormu.Size = new System.Drawing.Size(120, 34);

            // textBoxPretragaPdf
            this.textBoxPretragaPdf.Size = new System.Drawing.Size(100, 34);

            // btnNadjiPdf
            this.btnNadjiPdf.Text = "Pronađi PDF";
            this.btnNadjiPdf.BackColor = System.Drawing.Color.MidnightBlue;
            this.btnNadjiPdf.ForeColor = System.Drawing.Color.White;
            this.btnNadjiPdf.Size = new System.Drawing.Size(120, 34);

            // Dodavanje u FlowLayoutPanel
            this.flowLayoutPanel1.Controls.Add(this.btnZameniFormu);
            this.flowLayoutPanel1.Controls.Add(this.textBoxPretragaPdf);
            this.flowLayoutPanel1.Controls.Add(this.btnNadjiPdf);

            // Dodavanje kontrole u TableLayoutPanel
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblNazivPdfFajla, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.chkMenjasNaziv, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtNoviNazivFajla, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBox2, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.comboBox1, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.comboBox2, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.comboBox3, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.comboBox4, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.comboBox5, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.comboBox6, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.label10, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.comboBox7, 1, 9);
            this.tableLayoutPanel1.Controls.Add(this.label11, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.comboBox8, 1, 10);
            this.tableLayoutPanel1.Controls.Add(this.label12, 0, 11); // Datum od
            this.tableLayoutPanel1.Controls.Add(this.textBoxDatumOd, 1, 11);
            this.tableLayoutPanel1.Controls.Add(this.label13, 0, 12); // Datum do
            this.tableLayoutPanel1.Controls.Add(this.textBoxDatumDo, 1, 12);
            this.tableLayoutPanel1.Controls.Add(this.button1, 0, 13);
            this.tableLayoutPanel1.Controls.Add(this.button2, 1, 13);
            this.tableLayoutPanel1.Controls.Add(this.btn_Izvestaj, 0, 14);
            this.tableLayoutPanel1.Controls.Add(this.btnSacuvaj, 1, 14);

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1096, 706);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Form1";

            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblNazivPdfFajla;
        private System.Windows.Forms.Label txtNoviNazivFajla;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.ComboBox comboBox5;
        private System.Windows.Forms.ComboBox comboBox6;
        private System.Windows.Forms.ComboBox comboBox7;
        private System.Windows.Forms.ComboBox comboBox8;
        private System.Windows.Forms.CheckBox chkMenjasNaziv;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBoxDatumOd;
        private System.Windows.Forms.TextBox textBoxDatumDo;
        private System.Windows.Forms.Button btnSacuvaj;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btn_Izvestaj;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnZameniFormu;
        private System.Windows.Forms.TextBox textBoxPretragaPdf;
        private System.Windows.Forms.Button btnNadjiPdf;
    }
}