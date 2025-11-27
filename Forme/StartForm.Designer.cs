namespace IndexPDF2
{
    partial class StartForm
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
            TextBoxInput = new System.Windows.Forms.TextBox();
            textBoxOutput = new System.Windows.Forms.TextBox();
            btnInputBrowse = new System.Windows.Forms.Button();
            btnOutputBrowse = new System.Windows.Forms.Button();
            btnDalje = new System.Windows.Forms.Button();
            textBoxOperater = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();

            dateTimePickerDatum = new System.Windows.Forms.DateTimePicker();
            btnPrikazi = new System.Windows.Forms.Button();

            SuspendLayout();

            // TextBoxInput  
            TextBoxInput.Font = new System.Drawing.Font("Segoe UI", 15F);
            TextBoxInput.Location = new System.Drawing.Point(10, 71);
            TextBoxInput.Multiline = true;
            TextBoxInput.Name = "TextBoxInput";
            TextBoxInput.Size = new System.Drawing.Size(498, 82);
            TextBoxInput.TabIndex = 0;

            // textBoxOutput  
            textBoxOutput.Font = new System.Drawing.Font("Segoe UI", 15F);
            textBoxOutput.Location = new System.Drawing.Point(10, 179);
            textBoxOutput.Multiline = true;
            textBoxOutput.Name = "textBoxOutput";
            textBoxOutput.Size = new System.Drawing.Size(498, 82);
            textBoxOutput.TabIndex = 1;

            // btnInputBrowse  
            btnInputBrowse.BackColor = System.Drawing.Color.MidnightBlue;
            btnInputBrowse.Font = new System.Drawing.Font("Segoe UI", 12F);
            btnInputBrowse.ForeColor = System.Drawing.SystemColors.Control;
            btnInputBrowse.Location = new System.Drawing.Point(514, 71);
            btnInputBrowse.Name = "btnInputBrowse";
            btnInputBrowse.Size = new System.Drawing.Size(224, 82);
            btnInputBrowse.TabIndex = 2;
            btnInputBrowse.Text = "BROWSE";
            btnInputBrowse.UseVisualStyleBackColor = false;
            btnInputBrowse.Click += btnInputBrowse_Click;

            // btnOutputBrowse  
            btnOutputBrowse.BackColor = System.Drawing.Color.MidnightBlue;
            btnOutputBrowse.Font = new System.Drawing.Font("Segoe UI", 12F);
            btnOutputBrowse.ForeColor = System.Drawing.SystemColors.Control;
            btnOutputBrowse.Location = new System.Drawing.Point(514, 179);
            btnOutputBrowse.Name = "btnOutputBrowse";
            btnOutputBrowse.Size = new System.Drawing.Size(224, 82);
            btnOutputBrowse.TabIndex = 3;
            btnOutputBrowse.Text = "BROWSE";
            btnOutputBrowse.UseVisualStyleBackColor = false;
            btnOutputBrowse.Click += btnOutputBrowse_Click;

            // btnDalje  
            btnDalje.BackColor = System.Drawing.Color.MidnightBlue;
            btnDalje.Font = new System.Drawing.Font("Segoe UI", 16F);
            btnDalje.ForeColor = System.Drawing.SystemColors.Control;
            btnDalje.Location = new System.Drawing.Point(10, 342);
            btnDalje.Name = "btnDalje";
            btnDalje.Size = new System.Drawing.Size(730, 85);
            btnDalje.TabIndex = 4;
            btnDalje.Text = "NASTAVITE DALJE";
            btnDalje.UseVisualStyleBackColor = false;
            btnDalje.Click += btnDalje_Click;

            // textBoxOperater  
            textBoxOperater.Font = new System.Drawing.Font("Segoe UI", 15F);
            textBoxOperater.Location = new System.Drawing.Point(10, 288);
            textBoxOperater.Multiline = true;
            textBoxOperater.Name = "textBoxOperater";
            textBoxOperater.Size = new System.Drawing.Size(498, 48);
            textBoxOperater.TabIndex = 5;
            textBoxOperater.KeyDown += textBoxOperater_KeyDown;

            // label1  
            label1.AutoSize = true;
            label1.BackColor = System.Drawing.Color.MidnightBlue;
            label1.Font = new System.Drawing.Font("Segoe UI", 11F);
            label1.ForeColor = System.Drawing.SystemColors.Control;
            label1.Location = new System.Drawing.Point(10, 48);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(107, 20);
            label1.TabIndex = 6;
            label1.Text = "INPUT FOLDER";

            // label2  
            label2.AutoSize = true;
            label2.BackColor = System.Drawing.Color.MidnightBlue;
            label2.Font = new System.Drawing.Font("Segoe UI", 11F);
            label2.ForeColor = System.Drawing.SystemColors.Control;
            label2.Location = new System.Drawing.Point(10, 156);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(121, 20);
            label2.TabIndex = 7;
            label2.Text = "OUTPUT FOLDER";

            // label3  
            label3.AutoSize = true;
            label3.BackColor = System.Drawing.Color.MidnightBlue;
            label3.Font = new System.Drawing.Font("Segoe UI", 13F);
            label3.ForeColor = System.Drawing.SystemColors.Control;
            label3.Location = new System.Drawing.Point(215, 20);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(338, 25);
            label3.TabIndex = 8;
            label3.Text = "UNESITE VASE INPUT I OUTPUT FOLDERE";

            // label4  
            label4.AutoSize = true;
            label4.BackColor = System.Drawing.Color.MidnightBlue;
            label4.Font = new System.Drawing.Font("Segoe UI", 11F);
            label4.ForeColor = System.Drawing.SystemColors.Control;
            label4.Location = new System.Drawing.Point(12, 264);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(118, 20);
            label4.TabIndex = 9;
            label4.Text = "IME OPERATERA";

            // dateTimePickerDatum  
            dateTimePickerDatum.Location = new System.Drawing.Point(10, 450);
            dateTimePickerDatum.Name = "dateTimePickerDatum";
            dateTimePickerDatum.Size = new System.Drawing.Size(250, 23);
            dateTimePickerDatum.TabIndex = 10;
            checkedListBoxDatumi = new System.Windows.Forms.CheckedListBox();
            checkedListBoxDatumi.CheckOnClick = true;
            checkedListBoxDatumi.Font = new System.Drawing.Font("Segoe UI", 10F);
            checkedListBoxDatumi.FormattingEnabled = true;
            checkedListBoxDatumi.Location = new System.Drawing.Point(450, 440);
            checkedListBoxDatumi.Name = "checkedListBoxDatumi";
            checkedListBoxDatumi.Size = new System.Drawing.Size(150, 200);
            checkedListBoxDatumi.TabIndex = 12;
          

            // btnPrikazi  
            btnPrikazi.BackColor = System.Drawing.Color.MidnightBlue;
            btnPrikazi.Font = new System.Drawing.Font("Segoe UI", 12F);
            btnPrikazi.ForeColor = System.Drawing.SystemColors.Control;
            btnPrikazi.Location = new System.Drawing.Point(270, 450);
            btnPrikazi.Name = "btnPrikazi";
            btnPrikazi.Size = new System.Drawing.Size(150, 25);
            btnPrikazi.TabIndex = 11;
            btnPrikazi.Text = "PRIKAŽI BAZU";
            btnPrikazi.UseVisualStyleBackColor = false;
            btnPrikazi.Click += btnPrikazi_Click;

            // StartForm  
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 500);
            Controls.Add(btnPrikazi);
            Controls.Add(dateTimePickerDatum);
            Controls.Add(label4);
            Controls.Add(textBoxOperater);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnDalje);
            Controls.Add(btnOutputBrowse);
            Controls.Add(btnInputBrowse);
            Controls.Add(textBoxOutput);
            Controls.Add(TextBoxInput);
            Name = "StartForm";
            Text = "StartForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox TextBoxInput;
        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.Button btnInputBrowse;
        private System.Windows.Forms.Button btnOutputBrowse;
        private System.Windows.Forms.Button btnDalje;
        private System.Windows.Forms.TextBox textBoxOperater;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;

        private System.Windows.Forms.DateTimePicker dateTimePickerDatum;
        private System.Windows.Forms.CheckedListBox checkedListBoxDatumi;
        private System.Windows.Forms.Button btnPrikazi;
    }

}