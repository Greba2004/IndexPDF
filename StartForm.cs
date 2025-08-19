using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IndexPDF2.Properties;

namespace IndexPDF2
{
    public partial class StartForm : Form
    {
        public StartForm()
        {
            InitializeComponent();
            TextBoxInput.Text = Properties.Settings.Default.InputFolder;
            textBoxOutput.Text = Properties.Settings.Default.OutputFolder;
            textBoxOperater.Text = Properties.Settings.Default.Operater;
        }

        private void btnInputBrowse_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog fbd = new();
            if (fbd.ShowDialog() == DialogResult.OK)
                TextBoxInput.Text = fbd.SelectedPath;
        }

        private void btnOutputBrowse_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog fbd = new();
            if (fbd.ShowDialog() == DialogResult.OK)
                textBoxOutput.Text = fbd.SelectedPath;
        }

        private void btnDalje_Click(object sender, EventArgs e)
        {
            string inputPath = TextBoxInput.Text;
            string outputPath = textBoxOutput.Text;
            string imeOperatera = textBoxOperater.Text;


            if (string.IsNullOrWhiteSpace(inputPath) || string.IsNullOrWhiteSpace(outputPath) || string.IsNullOrWhiteSpace(imeOperatera))
            {
                MessageBox.Show("Morate popuniti sva tri polja: Input folder, Output folder i ime operatera!", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            string[] pdfFajlovi = Directory.GetFiles(inputPath, "*.pdf", SearchOption.TopDirectoryOnly);
            if (pdfFajlovi.Length == 0)
            {
                MessageBox.Show("Input folder ne sadrži nijedan PDF fajl!", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Properties.Settings.Default.InputFolder = inputPath;
            Properties.Settings.Default.OutputFolder = outputPath;
            Properties.Settings.Default.Operater = imeOperatera;
            Properties.Settings.Default.Save();

            Form1 mainForm = new Form1(inputPath, outputPath, imeOperatera);
            mainForm.Show();
            this.Hide();
        }
        public string ImeOperatera => textBoxOperater.Text;

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBoxOperater_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnDalje.PerformClick(); // Simulira klik na dugme
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}
