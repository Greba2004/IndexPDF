using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using IndexPDF2.Servisi;
using IndexPDF2.Modeli;
using IndexPDF2.Properties;
using IndexPDF2.Forme;

namespace IndexPDF2
{
    public partial class StartForm : Form
    {
        private DatabaseService dbService;
        private ConfigData configData;

        public StartForm()
        {
            InitializeComponent();

            TextBoxInput.Text = Properties.Settings.Default.InputFolder;
            textBoxOutput.Text = Properties.Settings.Default.OutputFolder;
            textBoxOperater.Text = Properties.Settings.Default.Operater;

            // Inicijalizacija DatabaseService  
            dbService = new DatabaseService(TextBoxInput.Text);
            configData = new ConfigData();

            // Popuni poslednjih 30 dana
            checkedListBoxDatumi.Items.Clear();

            for (int i = 0; i < 30; i++)
            {
                var d = DateTime.Today.AddDays(-i);
                checkedListBoxDatumi.Items.Add(d.ToString("dd.MM.yyyy"));
            }
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

            // 🔥 ADMIN MODE
            if (imeOperatera.Trim().Equals("/RWadmin", StringComparison.OrdinalIgnoreCase))
            {
                var adminForm = new AdminForm(dbService);
                adminForm.Show();
                this.Hide();
                return;
            }

            if (string.IsNullOrWhiteSpace(inputPath) ||
                string.IsNullOrWhiteSpace(outputPath) ||
                string.IsNullOrWhiteSpace(imeOperatera))
            {
                MessageBox.Show("Morate popuniti sva tri polja!", "Greška",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string[] pdfFajlovi = Directory.GetFiles(inputPath, "*.pdf", SearchOption.TopDirectoryOnly);
            if (pdfFajlovi.Length == 0)
            {
                MessageBox.Show("Input folder ne sadrži nijedan PDF fajl!", "Greška",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void textBoxOperater_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnDalje.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }


        // =============================
        //   NOVO: MULTI-DATE FUNKCIJE
        // =============================

        /// <summary>
        /// Vraća sve selektovane datume iz CheckedListBox-a.
        /// </summary>
        private List<DateTime> UzmiSelektovaneDatume()
        {
            var datumi = new List<DateTime>();

            foreach (var item in checkedListBoxDatumi.CheckedItems)
            {
                if (DateTime.TryParse(item.ToString(), out DateTime d))
                    datumi.Add(d.Date);
            }

            return datumi;
        }

        /// <summary>
        /// Prikaz baze za više izabranih datuma.
        /// </summary>
        private void btnPrikazi_Click(object sender, EventArgs e)
        {
            var selektovaniDatumi = UzmiSelektovaneDatume();

            if (!selektovaniDatumi.Any())
            {
                MessageBox.Show("Morate selektovati bar jedan datum!", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var izvestajServis = new IzvestajServis(
                outputFolderPath: textBoxOutput.Text,
                dbService: dbService,
                configData);

            int ukupnoDokumenata = 0;

            foreach (var datum in selektovaniDatumi)
            {
                var pdfoviZaDatum = dbService.UzmiZaDatum(datum);

                if (pdfoviZaDatum.Any())
                {
                    izvestajServis.GenerisiIzvestajExcel(datum, operatorName: "");
                    ukupnoDokumenata += pdfoviZaDatum.Count;
                }
            }

            if (ukupnoDokumenata == 0)
            {
                MessageBox.Show("Nema podataka za izabrane datume.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show("Izveštaji su uspešno generisani!", "OK",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}