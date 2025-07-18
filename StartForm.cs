using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndexPDF2
{
    public partial class StartForm : Form
    {
        public StartForm()
        {
            InitializeComponent();
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

            if (string.IsNullOrWhiteSpace(inputPath) || string.IsNullOrWhiteSpace(outputPath))
            {
                MessageBox.Show("Morate uneti oba foldera!");
                return;
            }

            Form1 mainForm = new Form1(inputPath, outputPath);
            mainForm.Show();
            this.Hide(); // sakrij start formu
        }
    }
}
