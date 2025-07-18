namespace IndexPDF2
{
    partial class StartForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            TextBoxInput = new TextBox();
            textBoxOutput = new TextBox();
            btnInputBrowse = new Button();
            btnOutputBrowse = new Button();
            btnDalje = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            SuspendLayout();
            // 
            // TextBoxInput
            // 
            TextBoxInput.Location = new Point(12, 101);
            TextBoxInput.Multiline = true;
            TextBoxInput.Name = "TextBoxInput";
            TextBoxInput.Size = new Size(498, 82);
            TextBoxInput.TabIndex = 0;
            // 
            // textBoxOutput
            // 
            textBoxOutput.Location = new Point(10, 236);
            textBoxOutput.Multiline = true;
            textBoxOutput.Name = "textBoxOutput";
            textBoxOutput.Size = new Size(498, 82);
            textBoxOutput.TabIndex = 1;
            // 
            // btnInputBrowse
            // 
            btnInputBrowse.BackColor = Color.MidnightBlue;
            btnInputBrowse.Font = new Font("Segoe UI", 12F);
            btnInputBrowse.ForeColor = SystemColors.Control;
            btnInputBrowse.Location = new Point(516, 101);
            btnInputBrowse.Name = "btnInputBrowse";
            btnInputBrowse.Size = new Size(224, 82);
            btnInputBrowse.TabIndex = 2;
            btnInputBrowse.Text = "BROWSE";
            btnInputBrowse.UseVisualStyleBackColor = false;
            btnInputBrowse.Click += btnInputBrowse_Click;
            // 
            // btnOutputBrowse
            // 
            btnOutputBrowse.BackColor = Color.MidnightBlue;
            btnOutputBrowse.Font = new Font("Segoe UI", 12F);
            btnOutputBrowse.ForeColor = SystemColors.Control;
            btnOutputBrowse.Location = new Point(516, 236);
            btnOutputBrowse.Name = "btnOutputBrowse";
            btnOutputBrowse.Size = new Size(224, 82);
            btnOutputBrowse.TabIndex = 3;
            btnOutputBrowse.Text = "BROWSE";
            btnOutputBrowse.UseVisualStyleBackColor = false;
            btnOutputBrowse.Click += btnOutputBrowse_Click;
            // 
            // btnDalje
            // 
            btnDalje.BackColor = Color.MidnightBlue;
            btnDalje.Font = new Font("Segoe UI", 16F);
            btnDalje.ForeColor = SystemColors.Control;
            btnDalje.Location = new Point(10, 342);
            btnDalje.Name = "btnDalje";
            btnDalje.Size = new Size(730, 85);
            btnDalje.TabIndex = 4;
            btnDalje.Text = "NASTAVITE DALJE";
            btnDalje.UseVisualStyleBackColor = false;
            btnDalje.Click += btnDalje_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.MidnightBlue;
            label1.Font = new Font("Segoe UI", 11F);
            label1.ForeColor = SystemColors.Control;
            label1.Location = new Point(12, 70);
            label1.Name = "label1";
            label1.Size = new Size(107, 20);
            label1.TabIndex = 5;
            label1.Text = "INPUT FOLDER";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.MidnightBlue;
            label2.Font = new Font("Segoe UI", 11F);
            label2.ForeColor = SystemColors.Control;
            label2.Location = new Point(12, 205);
            label2.Name = "label2";
            label2.Size = new Size(121, 20);
            label2.TabIndex = 6;
            label2.Text = "OUTPUT FOLDER";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.MidnightBlue;
            label3.Font = new Font("Segoe UI", 13F);
            label3.ForeColor = SystemColors.Control;
            label3.Location = new Point(215, 20);
            label3.Name = "label3";
            label3.Size = new Size(338, 25);
            label3.TabIndex = 7;
            label3.Text = "UNESITE VASE INPUT I OUTPUT FOLDERE";
            // 
            // StartForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnDalje);
            Controls.Add(btnOutputBrowse);
            Controls.Add(btnInputBrowse);
            Controls.Add(textBoxOutput);
            Controls.Add(TextBoxInput);
            Name = "StartForm";
            Text = "Form2";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox TextBoxInput;
        private TextBox textBoxOutput;
        private Button button1;
        private Button button2;
        private Button button3;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button btn;
        private Button btnDalje;
        private Button btnInputBrowse;
        private Button btnOutputBrowse;
    }
}