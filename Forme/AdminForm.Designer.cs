namespace IndexPDF2.Forme
{
    partial class AdminForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.groupFilter = new System.Windows.Forms.GroupBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnPrimeni = new System.Windows.Forms.Button();
            this.dateDo = new System.Windows.Forms.DateTimePicker();
            this.dateOd = new System.Windows.Forms.DateTimePicker();
            this.comboOperater = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvPodaci = new System.Windows.Forms.DataGridView();
            this.txtPretraga = new System.Windows.Forms.TextBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.groupFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPodaci)).BeginInit();
            this.SuspendLayout();
            // 
            // groupFilter
            // 
            this.groupFilter.Controls.Add(this.btnReset);
            this.groupFilter.Controls.Add(this.btnPrimeni);
            this.groupFilter.Controls.Add(this.dateDo);
            this.groupFilter.Controls.Add(this.dateOd);
            this.groupFilter.Controls.Add(this.comboOperater);
            this.groupFilter.Controls.Add(this.label3);
            this.groupFilter.Controls.Add(this.label2);
            this.groupFilter.Controls.Add(this.label1);
            this.groupFilter.Controls.Add(this.txtPretraga);
            this.groupFilter.Controls.Add(this.btnExport);
            this.groupFilter.Location = new System.Drawing.Point(12, 12);
            this.groupFilter.Name = "groupFilter";
            this.groupFilter.Size = new System.Drawing.Size(776, 140);
            this.groupFilter.TabIndex = 0;
            this.groupFilter.TabStop = false;
            this.groupFilter.Text = "Filteri";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(642, 70);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(112, 27);
            this.btnReset.TabIndex = 7;
            this.btnReset.Text = "Resetuj filter";
            this.btnReset.UseVisualStyleBackColor = true;
            // 
            // btnPrimeni
            // 
            this.btnPrimeni.Location = new System.Drawing.Point(642, 30);
            this.btnPrimeni.Name = "btnPrimeni";
            this.btnPrimeni.Size = new System.Drawing.Size(112, 27);
            this.btnPrimeni.TabIndex = 6;
            this.btnPrimeni.Text = "Primeni filter";
            this.btnPrimeni.UseVisualStyleBackColor = true;
            // 
            // dateDo
            // 
            this.dateDo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateDo.Location = new System.Drawing.Point(417, 72);
            this.dateDo.Name = "dateDo";
            this.dateDo.Size = new System.Drawing.Size(134, 22);
            this.dateDo.TabIndex = 5;
            // 
            // dateOd
            // 
            this.dateOd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateOd.Location = new System.Drawing.Point(417, 32);
            this.dateOd.Name = "dateOd";
            this.dateOd.Size = new System.Drawing.Size(134, 22);
            this.dateOd.TabIndex = 4;
            // 
            // comboOperater
            // 
            this.comboOperater.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboOperater.FormattingEnabled = true;
            this.comboOperater.Location = new System.Drawing.Point(91, 32);
            this.comboOperater.Name = "comboOperater";
            this.comboOperater.Size = new System.Drawing.Size(160, 24);
            this.comboOperater.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(328, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 23);
            this.label3.TabIndex = 2;
            this.label3.Text = "Datum do:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(328, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "Datum od:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Operater:";
            // 
            // txtPretraga
            // 
            this.txtPretraga.Location = new System.Drawing.Point(280, 105);
            this.txtPretraga.Name = "txtPretraga";
            this.txtPretraga.Size = new System.Drawing.Size(350, 22);
            this.txtPretraga.TabIndex = 8;
            this.txtPretraga.PlaceholderText = "Pretraga po nazivu fajla...";
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(642, 105);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(112, 27);
            this.btnExport.TabIndex = 9;
            this.btnExport.Text = "Export u Excel";
            this.btnExport.UseVisualStyleBackColor = true;
            // 
            // dgvPodaci
            // 
            this.dgvPodaci.AllowUserToAddRows = false;
            this.dgvPodaci.AllowUserToDeleteRows = false;
            this.dgvPodaci.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPodaci.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPodaci.Location = new System.Drawing.Point(12, 160);
            this.dgvPodaci.Name = "dgvPodaci";
            this.dgvPodaci.ReadOnly = true;
            this.dgvPodaci.RowHeadersVisible = false;
            this.dgvPodaci.RowTemplate.Height = 24;
            this.dgvPodaci.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPodaci.Size = new System.Drawing.Size(776, 278);
            this.dgvPodaci.TabIndex = 1;
            // 
            // AdminForm
            // 
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dgvPodaci);
            this.Controls.Add(this.groupFilter);
            this.Name = "AdminForm";
            this.Text = "Admin panel";
            this.groupFilter.ResumeLayout(false);
            this.groupFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPodaci)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupFilter;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnPrimeni;
        private System.Windows.Forms.DateTimePicker dateDo;
        private System.Windows.Forms.DateTimePicker dateOd;
        private System.Windows.Forms.ComboBox comboOperater;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvPodaci;
        private System.Windows.Forms.TextBox txtPretraga;
        private System.Windows.Forms.Button btnExport;
    }
}