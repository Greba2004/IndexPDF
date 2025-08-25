using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        string csvTempPath;

        PdfViewer pdfViewer = new PdfViewer(); // PDF pregled
        List<InputPdfFile> pdfFajloviZajednicki;
        private Form1 mainForm;
        private bool sviFajloviObradjeni = false;
        private string tempPdfPath;

        // KONSTRUKTOR
        public FormMenica(string inputFolder, string outputFolder, string operatorName, List<InputPdfFile> pdfZajednicki)
        {
            InitializeComponent();

            this.inputFolderPath = inputFolder;
            this.outputFolderPath = outputFolder;
            this.operatorName = operatorName;
            this.pdfFajloviZajednicki = pdfZajednicki;
            this.pdfFajlovi = new List<InputPdfFile>(pdfZajednicki);
            this.FormClosing += FormMenica_FormClosing;
           
            if (pdfFajlovi.Count > 0)
            {
                trenutniIndex = 0;
                PrikaziTrenutniFajl();
            }
            this.KeyPreview = true;
            this.KeyDown += FormMenica_KeyDown;

            // --- Kreiraj folder u ProgramData ---
            string appDataFolder = Path.Combine(
               Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
               "IndexPDF"
           );

            if (!Directory.Exists(appDataFolder))
                Directory.CreateDirectory(appDataFolder);

            configExcelPath = Path.Combine(appDataFolder, "config.xlsx");
            csvTempPath = Path.Combine(appDataFolder, "podaci_temp.csv");

            try
            {
                configData = ExcelConfigLoader.UcitajKonfiguracijuIzExcel(configExcelPath);
                PostaviNazivePoljaUI();
                UcitajPodatkeIzCsv();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri učitavanju Excel fajla: " + ex.Message +
                                "\nMolimo postavite 'config.xlsx' u folder:\n" + configExcelPath,
                                "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // --- Učitaj PDF fajlove ---
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
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                if (!(ActiveControl is Button))
                {
                    btnSacuvaj.PerformClick();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
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

            // Učitaj PDF u memorijski stream
            byte[] pdfBytes = File.ReadAllBytes(path);
            var memoryStream = new MemoryStream(pdfBytes);
            var pdfDoc = PdfiumViewer.PdfDocument.Load(memoryStream);

            // Kreiraj novi PDF Viewer
            pdfViewer = new PdfiumViewer.PdfViewer
            {
                Dock = DockStyle.Fill,
                Document = pdfDoc
            };

            // Ukloni prethodni kontrol iz TableLayoutPanel
            var controlInCell = tableLayoutPanel1.GetControlFromPosition(0, 0);
            if (controlInCell != null)
                tableLayoutPanel1.Controls.Remove(controlInCell);

            // Dodaj PDF Viewer u TableLayoutPanel
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

            // obrisi temp fajl ako postoji
            if (!string.IsNullOrEmpty(tempPdfPath) && File.Exists(tempPdfPath))
            {
                try { File.Delete(tempPdfPath); } catch { /* ignorisi grešku */ }
                tempPdfPath = null;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
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
            OslobodiPdfViewer();
            if (trenutniIndex < 0 || trenutniIndex >= pdfFajlovi.Count)
                return;

            var trenutniPdf = pdfFajlovi[trenutniIndex];

            string nazivFajla = string.IsNullOrWhiteSpace(trenutniPdf.NewFileName)
                ? Path.GetFileName(trenutniPdf.OriginalPath)
                : trenutniPdf.NewFileName;

            if (!nazivFajla.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                nazivFajla += ".pdf";

            string destinationPath = Path.Combine(outputFolderPath, nazivFajla);

            if (File.Exists(trenutniPdf.OriginalPath))
            {
                // prvo oslobodi viewer
                OslobodiPdfViewer();

                // sada uradi Copy + Delete umesto Move
                bool copySucceeded = false;
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        File.Copy(trenutniPdf.OriginalPath, destinationPath, overwrite: true);
                        copySucceeded = true;
                        break;
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(200); // malo sačekaj pa probaj ponovo
                    }
                }

                if (copySucceeded)
                {
                    try
                    {
                        File.Delete(trenutniPdf.OriginalPath);
                    }
                    catch (IOException)
                    {
                        // ako je i dalje zaključan, ostavi ga — već imamo kopiju
                    }

                    // ažuriraj putanju da pokazuje na novi fajl
                    trenutniPdf.OriginalPath = destinationPath;
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

                foreach (var pdf in pdfFajloviZajednicki)
                {
                    bool sviPopunjeni = true;
                    for (int i = 0; i < 8; i++)
                    {
                        if (configData.PoljaObavezna[i] && string.IsNullOrWhiteSpace(pdf.Polja[i]))
                        {
                            sviPopunjeni = false;
                            break;
                        }
                    }

                    // Ako nisu popunjena obavezna polja, preskoči ovaj red
                    if (!sviPopunjeni) continue;

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
        private void FormMenica_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sviFajloviObradjeni)
            {
                // Preskoči dijalog i samo sacuvaj CSV
                SacuvajPodatkeUCsv();
                return;
            }
            // Opciono: pitaj korisnika da potvrdi izlazak
            MessageBox.Show("FormClosing event aktiviran!");
            var result = MessageBox.Show("Da li želite da izađete iz aplikacije?",
                                         "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true; // poništi zatvaranje
                return;
            }

            // Sačuvaj sve trenutne podatke u CSV pre zatvaranja
            SacuvajPodatkeUCsv();

           
        }
        private void SacuvajPodatkeUCsv()
        {
            try
            {
                // Ukloni sve fajlove koji nisu u output folderu
                var pdfZaCuvanje = pdfFajloviZajednicki
                    .Where(p => File.Exists(p.OriginalPath) && p.OriginalPath.StartsWith(outputFolderPath))
                    .ToList();

                using (var writer = new StreamWriter(csvTempPath, false)) // overwrite
                {
                    foreach (var pdf in pdfZaCuvanje)
                    {
                        string linija = string.Join(";", new string[]
                        {
                    pdf.FileName,
                    pdf.NewFileName ?? "",
                    string.Join("|", pdf.Polja ?? new string[10]),
                    pdf.DatumObrade == DateTime.MinValue ? "" : pdf.DatumObrade.ToString("o"),
                    operatorName
                        });
                        writer.WriteLine(linija);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri čuvanju CSV fajla: " + ex.Message);
            }
        }
        private void UcitajPodatkeIzCsv()
        {
            try
            {
                if (!File.Exists(csvTempPath)) return;

                var lines = File.ReadAllLines(csvTempPath);
                foreach (var line in lines)
                {
                    var delovi = line.Split(';');
                    if (delovi.Length >= 5)
                    {
                        var pdf = new InputPdfFile(delovi[0]);
                        pdf.NewFileName = delovi[1];
                        pdf.Polja = delovi[2].Split('|');
                        if (DateTime.TryParse(delovi[3], out var dt))
                            pdf.DatumObrade = dt;

                        // Dodaj samo ako fajl postoji u output folderu
                        if (File.Exists(pdf.OriginalPath) && pdf.OriginalPath.StartsWith(outputFolderPath))
                            pdfFajloviZajednicki.Add(pdf);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri učitavanju CSV fajla: " + ex.Message);
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
            if (!pdfFajloviZajednicki.Contains(pdfFajlovi[trenutniIndex]))
            {
                pdfFajloviZajednicki.Add(pdfFajlovi[trenutniIndex]);
            }
            var trenutniPdf = pdfFajlovi[trenutniIndex];
            if (!pdfFajloviZajednicki.Any(p => p.OriginalPath == trenutniPdf.OriginalPath))
            {
                pdfFajloviZajednicki.Add(trenutniPdf);
            }

            PremestiTrenutniPdfUFolder();

            PredjiNaSledeciFajl();

            OcistiPolja();

            if (trenutniIndex >= pdfFajlovi.Count)
            {
                MessageBox.Show("Svi fajlovi su obrađeni!", "Gotovo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                sviFajloviObradjeni = true;

                try
                {
                    // ✅ Generiši izveštaj
                    GenerisiIzvestajExcel();

                    // ✅ Oslobodi sve resurse unutar aplikacije
                    OslobodiSvePdfResurse();

                    // ✅ Ubij spoljne procese koji drže PDF (ako ih ima)
                    string[] pdfProcesi = { "Acrobat", "AcroRd32", "FoxitReader", "PdfiumViewerApp" };
                    foreach (var procName in pdfProcesi)
                    {
                        foreach (var proc in System.Diagnostics.Process.GetProcessesByName(procName))
                        {
                            try
                            {
                                proc.Kill();
                                proc.WaitForExit();
                            }
                            catch { /* ignorisi greške */ }
                        }
                    }

                    // ✅ Poruka i gašenje aplikacije
                    MessageBox.Show("Svi PDF resursi su oslobođeni i aplikacija će se zatvoriti.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Greška pri završavanju procesa: " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void OslobodiSvePdfResurse()
        {
            try
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

                if (!string.IsNullOrEmpty(tempPdfPath) && File.Exists(tempPdfPath))
                {
                    File.Delete(tempPdfPath);
                    tempPdfPath = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch { /* ignorisi grešku */ }
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



      

        private void FormMenica_Load(object sender, EventArgs e)
        {

        }
    }
}
