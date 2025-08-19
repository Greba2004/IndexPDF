using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PdfiumViewer;
using ClosedXML.Excel;
using IndexPDF2.Modeli;

namespace IndexPDF2
{
    public partial class FormMenica : Form
    {
        // GLOBALNE VARIJABLE
        List<InputPdfFile> pdfFajlovi = new();
        int trenutniIndex = 0;
        ConfigData configData = new();

        string inputFolderPath = "";
        string outputFolderPath = "";
        string operatorName = "";
        string configExcelPath;

        PdfViewer pdfViewer = new PdfViewer(); // PDF pregled

        // KONSTRUKTOR
        public FormMenica(string inputFolder, string outputFolder, string operatorName)
        {
            InitializeComponent();

            this.inputFolderPath = inputFolder;
            this.outputFolderPath = outputFolder;
            this.operatorName = operatorName;

            this.KeyPreview = true;
            this.KeyDown += FormMenica_KeyDown;

            configExcelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xlsx");
            MessageBox.Show("Config fajl se traži na putanji:\n" + configExcelPath);

            try
            {
                configData = ExcelConfigLoader.UcitajKonfiguracijuIzExcel(configExcelPath);
                PostaviNazivePoljaUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri učitavanju Excel fajla: " + ex.Message);
            }

            UcitajPdfFajlove();
            if (pdfFajlovi.Count > 0)
            {
                trenutniIndex = 0;
                PrikaziTrenutniFajl();
            }
        }

        // --- VALIDACIJA ---
        private bool ValidirajObaveznaPolja()
        {
            ComboBox[] comboBoxes = { comboBox1, comboBox2, comboBox3, comboBox4, comboBox5, comboBox6, comboBox7, comboBox8 };

            for (int i = 0; i < comboBoxes.Length; i++)
            {
                if (configData.PoljaObavezna[i] && string.IsNullOrWhiteSpace(comboBoxes[i].Text))
                {
                    MessageBox.Show($"Polje '{configData.PoljaNazivi[i]}' je obavezno i ne može biti prazno!");
                    comboBoxes[i].Focus();
                    return false;
                }
            }
            return true;
        }

        private bool ValidirajDatume()
        {
            string datumOdText = textBoxDatumOd.Text.Trim();
            string datumDoText = textBoxDatumDo.Text.Trim();

            if (!string.IsNullOrWhiteSpace(datumOdText) &&
                !DateTime.TryParseExact(datumOdText, "dd.MM.yyyy.", null, System.Globalization.DateTimeStyles.None, out _))
            {
                MessageBox.Show("Datum OD nije validan. Koristite format: dd.MM.yyyy.");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(datumDoText) &&
                !DateTime.TryParseExact(datumDoText, "dd.MM.yyyy.", null, System.Globalization.DateTimeStyles.None, out _))
            {
                MessageBox.Show("Datum DO nije validan. Koristite format: dd.MM.yyyy.");
                return false;
            }

            return true;
        }

        // --- UI POLJA ---
        private void PostaviNazivePoljaUI()
        {
            for (int i = 0; i < 8; i++)
            {
                string labelName = "label" + (i + 4);
                var label = this.Controls.Find(labelName, true).FirstOrDefault() as Label;
                if (label != null)
                {
                    label.Text = configData.PoljaNazivi[i] + (configData.PoljaObavezna[i] ? " *" : "");
                    if (configData.PoljaObavezna[i])
                        label.ForeColor = Color.Red;
                }

                string comboBoxName = "comboBox" + (i + 1);
                var comboBox = this.Controls.Find(comboBoxName, true).FirstOrDefault() as ComboBox;
                if (comboBox != null && configData.PoljaListe[i] != null)
                {
                    comboBox.AutoCompleteCustomSource.Clear();
                    comboBox.AutoCompleteCustomSource.AddRange(configData.PoljaListe[i].ToArray());
                    comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                }
            }
        }

        // --- PDF FAJLOVI ---
        private void UcitajPdfFajlove()
        {
            if (string.IsNullOrEmpty(inputFolderPath) || !Directory.Exists(inputFolderPath))
            {
                MessageBox.Show("Ulazni folder nije validan!");
                return;
            }

            var fajlovi = Directory.GetFiles(inputFolderPath, "*.pdf");
            pdfFajlovi = fajlovi.Select(f => new InputPdfFile(f)).ToList();

            if (pdfFajlovi.Count == 0)
                MessageBox.Show("Nema PDF fajlova u izabranom folderu.");
        }

        private void PrikaziTrenutniFajl()
        {
            if (trenutniIndex < 0 || trenutniIndex >= pdfFajlovi.Count) return;

            var fajl = pdfFajlovi[trenutniIndex];
            lblNazivPdfFajla.Text = fajl.FileName;

            chkMenjasNaziv.Checked = false;
            textBox2.Enabled = false;
            textBox2.Text = fajl.FileName;

            UcitajPdfUFajlViewer(fajl.OriginalPath);
        }

        private void UcitajPdfUFajlViewer(string path)
        {
            // Oslobodi prethodni PDF Viewer i dokument
            if (pdfViewer != null)
            {
                if (pdfViewer.Document != null)
                {
                    pdfViewer.Document.Dispose();
                    pdfViewer.Document = null;
                }
                pdfViewer.Dispose();
                pdfViewer = null;
            }

            // Kreiraj novi PDF Viewer i otvori dokument
            var pdfDoc = PdfiumViewer.PdfDocument.Load(path); // 📌 dokument ostaje "živ"
            pdfViewer = new PdfiumViewer.PdfViewer
            {
                Dock = DockStyle.Fill,
                Document = pdfDoc
            };

            var controlInCell = tableLayoutPanel1.GetControlFromPosition(0, 0);
            if (controlInCell != null)
                tableLayoutPanel1.Controls.Remove(controlInCell);

            tableLayoutPanel1.Controls.Add(pdfViewer, 0, 0);
        }

        private void OslobodiPdfViewer()
        {
            if (pdfViewer != null)
            {
                if (pdfViewer.Document != null)
                {
                    pdfViewer.Document.Dispose();
                    pdfViewer.Document = null;
                }
                pdfViewer.Dispose();
                pdfViewer = null;
            }
        }

        // --- ČUVANJE I PREMESTANJE ---
        private void SacuvajUnetePodatkeUTrenutniPdf()
        {
            if (trenutniIndex < 0 || trenutniIndex >= pdfFajlovi.Count) return;

            var pdf = pdfFajlovi[trenutniIndex];

            for (int i = 0; i < 8; i++)
            {
                var control = this.Controls.Find("comboBox" + (i + 1), true).FirstOrDefault() as ComboBox;
                pdf.Polja[i] = control?.Text ?? "";
            }

            pdf.Polja[8] = textBoxDatumOd.Text;
            pdf.Polja[9] = textBoxDatumDo.Text;

            if (chkMenjasNaziv.Checked && !string.IsNullOrWhiteSpace(textBox2.Text))
                pdf.NewFileName = textBox2.Text.Trim();
            else
                pdf.NewFileName = pdf.FileName;
        }

        private void PremestiTrenutniPdfUFolder()
        {
            if (trenutniIndex < 0 || trenutniIndex >= pdfFajlovi.Count) return;

            // Oslobodi PDF Viewer i dokument
            OslobodiPdfViewer();

            var trenutniPdf = pdfFajlovi[trenutniIndex];
            string nazivFajla = string.IsNullOrWhiteSpace(trenutniPdf.NewFileName) ? Path.GetFileName(trenutniPdf.OriginalPath) : trenutniPdf.NewFileName;
            if (!nazivFajla.EndsWith(".pdf")) nazivFajla += ".pdf";

            string novaPutanja = Path.Combine(outputFolderPath, nazivFajla);

            if (File.Exists(trenutniPdf.OriginalPath))
            {
                // Dodaj mali retry u slučaju da je fajl još zaključen
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        File.Move(trenutniPdf.OriginalPath, novaPutanja);
                        trenutniPdf.OriginalPath = novaPutanja;
                        break;
                    }
                    catch (IOException)
                    {
                        System.Threading.Thread.Sleep(50);
                    }
                }
            }
        }

        // --- IZVEŠTAJ ---
        private void GenerisiIzvestajExcel()
        {
            try
            {
                var workbookPath = Path.Combine(outputFolderPath, "izvestaj.xlsx");

                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string archiveFolder = Path.Combine(appDirectory, "SviIzvestaji");
                if (!Directory.Exists(archiveFolder))
                    Directory.CreateDirectory(archiveFolder);

                var workbook = new XLWorkbook();
                var worksheet = workbook.AddWorksheet("Izvestaj");

                int red = 1;
                int kolona = 1;

                worksheet.Cell(red, kolona++).Value = "Stari naziv fajla";
                worksheet.Cell(red, kolona++).Value = "Novi naziv fajla";
                foreach (var naziv in configData.PoljaNazivi)
                    worksheet.Cell(red, kolona++).Value = naziv;
                worksheet.Cell(red, kolona++).Value = "Datum Od";
                worksheet.Cell(red, kolona++).Value = "Datum Do";
                worksheet.Cell(red, kolona++).Value = "Datum obrade";
                worksheet.Cell(red, kolona).Value = "Ime operatera";

                red++;

                foreach (var pdf in pdfFajlovi)
                {
                    string noviNaziv = string.IsNullOrWhiteSpace(pdf.NewFileName) ? pdf.FileName : pdf.NewFileName;
                    int kol = 1;

                    worksheet.Cell(red, kol++).Value = pdf.FileName;
                    worksheet.Cell(red, kol++).Value = noviNaziv;

                    for (int i = 0; i < 8; i++)
                        worksheet.Cell(red, kol++).Value = pdf.Polja[i] ?? "";

                    worksheet.Cell(red, kol++).Value = pdf.Polja[8] ?? "";
                    worksheet.Cell(red, kol++).Value = pdf.Polja[9] ?? "";
                    worksheet.Cell(red, kol++).Value = pdf.DatumObrade.ToString("dd.MM.yyyy. HH:mm:ss");
                    worksheet.Cell(red, kol).Value = operatorName;

                    red++;
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(workbookPath);

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string archiveFileName = $"Izvestaj_{timestamp}.xlsx";
                string archiveFilePath = Path.Combine(archiveFolder, archiveFileName);
                workbook.SaveAs(archiveFilePath);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = workbookPath,
                    UseShellExecute = true
                });

                MessageBox.Show($"Izveštaj je uspešno generisan i otvoren.\nKopija je sačuvana u folderu 'SviIzvestaji'.",
                                "Uspeh", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri generisanju izveštaja: " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- NAVIGACIJA ---
        private void PredjiNaSledeciFajl()
        {
            trenutniIndex++;
            if (trenutniIndex >= pdfFajlovi.Count)
            {
                lblNazivPdfFajla.Text = "Nema više fajlova za prikaz";
                OcistiPolja();
                chkMenjasNaziv.Enabled = false;
                textBox2.Enabled = false;
            }
            else
            {
                PrikaziTrenutniFajl();
            }
        }

        private void OcistiPolja()
        {
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox4.Text = "";
            comboBox5.Text = "";
            comboBox6.Text = "";
            comboBox7.Text = "";
            comboBox8.Text = "";

            textBoxDatumOd.Text = "";
            textBoxDatumDo.Text = "";
        }

        // --- EVENTI ---
        private void FormMenica_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !(ActiveControl is Button))
            {
                btnSacuvaj.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void chkMenjasNaziv_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMenjasNaziv.Checked)
            {
                textBox2.Enabled = true;
                textBox2.Text = "";
                textBox2.Focus();
            }
            else
            {
                var fajl = pdfFajlovi[trenutniIndex];
                textBox2.Enabled = false;
                textBox2.Text = fajl.FileName;
            }
        }

        private void btnSacuvaj_Click(object sender, EventArgs e)
        {
            if (!ValidirajObaveznaPolja())
                return;
            if (!ValidirajDatume())
                return;


            SacuvajUnetePodatkeUTrenutniPdf();

            if (trenutniIndex >= 0 && trenutniIndex < pdfFajlovi.Count)
            {
                pdfFajlovi[trenutniIndex].DatumObrade = DateTime.Now;
            }


            PremestiTrenutniPdfUFolder();

            PredjiNaSledeciFajl();

            OcistiPolja();

            if (trenutniIndex >= pdfFajlovi.Count)
            {
                MessageBox.Show("Svi fajlovi su obrađeni!", "Gotovo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                try
                {
                    // ✅ Generiši izveštaj
                    GenerisiIzvestajExcel();

                    // ✅ Ako je input folder prazan, ugasi aplikaciju
                    if (Directory.GetFiles(inputFolderPath, "*.pdf").Length == 0)
                    {
                        MessageBox.Show("Ulazni folder je prazan. Aplikacija će se zatvoriti.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Application.Exit();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Greška pri generisanju ili otvaranju izveštaja: " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSledeci_Click(object sender, EventArgs e)
        {
            trenutniIndex++;
            if (trenutniIndex >= pdfFajlovi.Count) trenutniIndex = pdfFajlovi.Count - 1;

            PrikaziTrenutniFajl();
            OcistiPolja();
        }

        private void btnPrethodni_Click(object sender, EventArgs e)
        {
            trenutniIndex--;
            if (trenutniIndex < 0) trenutniIndex = 0;

            PrikaziTrenutniFajl();
            OcistiPolja();
        }

        private void btnIzvestaj_Click(object sender, EventArgs e)
        {
            GenerisiIzvestajExcel();
        }
        
    }
}
