namespace IndexPDF2
{
    using PdfiumViewer;
    using IndexPDF2.Modeli;
    using System.IO;
    using System.Linq;
    using ClosedXML.Excel;

    public partial class Form1 : Form
    {
        //GLOBALNE VARIJABLE
        List<InputPdfFile> pdfFajlovi = new();
        int trenutniIndex = 0;
        ConfigData configData = new();

        string inputFolderPath = "";
        string outputFolderPath = "";
        string operatorName = "";
        string configExcelPath;
        string csvTempPath;
        public List<InputPdfFile> pdfFajloviZajednicki { get; set; } = new List<InputPdfFile>();
        private bool sviFajloviObradjeni = false;
        private string tempPdfPath; 



        public Form1(string inputFolder, string outputFolder, string operatorName)
        {
            InitializeComponent();
            this.inputFolderPath = inputFolder;
            this.outputFolderPath = outputFolder;
            this.operatorName = operatorName;
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
            this.FormClosing += Form1_FormClosing;

            // --- Folder u ProgramData ---
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

            UcitajPdfFajlove();
            if (pdfFajlovi.Count > 0)
            {
                trenutniIndex = 0;
                PrikaziTrenutniFajl();
            }
        }
        //VALIDACIJA OBAVEZNIH POLJA
        private bool ValidirajObaveznaPolja()
        {
            // Polja su u comboBox-evima ili tekstualnim poljima, zavisno od UI, na primer comboBox1..comboBox8
            ComboBox[] comboBoxes = { comboBox1, comboBox2, comboBox3, comboBox4, comboBox5, comboBox6, comboBox7, comboBox8 };

            for (int i = 0; i < comboBoxes.Length; i++)
            {
                if (configData.PoljaObavezna[i])
                {
                    if (string.IsNullOrWhiteSpace(comboBoxes[i].Text))
                    {
                        MessageBox.Show($"Polje '{configData.PoljaNazivi[i]}' je obavezno i ne može biti prazno!");
                        comboBoxes[i].Focus();
                        return false;
                    }
                }
            }
            return true;
        }
        // Enter simulira button sacuvaj
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
        //Validacija datuma
        private bool ValidirajDatume()
        {
            string datumOdText = textBoxDatumOd.Text.Trim();
            string datumDoText = textBoxDatumDo.Text.Trim();

            if (!string.IsNullOrWhiteSpace(datumOdText))
            {
                if (!DateTime.TryParseExact(datumOdText, "dd.MM.yyyy.", null, System.Globalization.DateTimeStyles.None, out _))
                {
                    MessageBox.Show("Datum OD nije validan. Koristite format: dd.MM.yyyy.");
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(datumDoText))
            {
                if (!DateTime.TryParseExact(datumDoText, "dd.MM.yyyy.", null, System.Globalization.DateTimeStyles.None, out _))
                {
                    MessageBox.Show("Datum DO nije validan. Koristite format: dd.MM.yyyy.");
                    return false;
                }
            }

            return true;
        }
        //POSTAVLJANJE POLJA
        private void PostaviNazivePoljaUI()
        {
            for (int i = 0; i < 8; i++)
            {
                // Postavi labelu (label4 do label11)
                string labelName = "label" + (i + 4);
                var label = this.Controls.Find(labelName, true).FirstOrDefault() as Label;
                if (label != null)
                {
                    label.Text = configData.PoljaNazivi[i] + (configData.PoljaObavezna[i] ? " *" : "");

                    // Ako je obavezno, promeni boju
                    if (configData.PoljaObavezna[i])
                        label.ForeColor = Color.Red;
                }

                // Postavi AutoComplete za odgovarajući ComboBox (comboBox1 do comboBox8)
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
        //UCITAVANJE PDF FAJLOVA
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
            {
                MessageBox.Show("Nema PDF fajlova u izabranom folderu.");
            }
        }
        //Metoda za cuvanje podataka
        private void SacuvajPodatke()
        {

            MessageBox.Show("Izmene su sačuvane.");
        }
        //Metoda za prelazak na sledeci fajl
        private void PredjiNaSledeciFajl()
        {
            trenutniIndex++;

            if (trenutniIndex >= pdfFajlovi.Count)
            {
                // Nema više fajlova za prikaz
                lblNazivPdfFajla.Text = "Nema više fajlova za prikaz";
                OcistiPolja();
                // Po potrebi onemogući kontrole da korisnik ne može menjati ništa više
                // Na primer:
                chkMenjasNaziv.Enabled = false;
                textBox2.Enabled = false;
                // itd.
            }
            else
            {
                PrikaziTrenutniFajl();
            }
        }
        //PRIKAZIVANJE PODATAKA NA FORMU
        private void PrikaziTrenutniFajl()
        {
            OslobodiPdfViewer();
            if (trenutniIndex < 0 || trenutniIndex >= pdfFajlovi.Count)
                return;

            var fajl = pdfFajlovi[trenutniIndex];

            // Gornja labela - naziv učitanog fajla
            lblNazivPdfFajla.Text = fajl.FileName;

            // Checkbox i TextBox
            chkMenjasNaziv.Checked = false;        // Checkbox isključimo na početku
            textBox2.Enabled = false;
            textBox2.Text = fajl.FileName;
            // Učitaj PDF fajl
            UcitajPdfUFajlViewer(fajl.OriginalPath);
        }
        // MENJANJE NAZIVA PDF 
        private void chkMenjasNaziv_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMenjasNaziv.Checked)
            {
                textBox2.Enabled = true;
                textBox2.Text = "";   // kad se čekira, brišemo TextBox da korisnik unese
                textBox2.Focus();
            }
            else
            {
                var fajl = pdfFajlovi[trenutniIndex];  // uzmi trenutno učitan fajl
                textBox2.Enabled = false;
                textBox2.Text = fajl.FileName;
            }
        }
        //CUVANJE PODATAKA U TRENUTNI PDF
        private void SacuvajUnetePodatkeUTrenutniPdf()
        {
            if (trenutniIndex < 0 || trenutniIndex >= pdfFajlovi.Count)
                return;

            var pdf = pdfFajlovi[trenutniIndex];

            // Sačuvaj ostale podatke (comboBox-eve i datume)
            for (int i = 0; i < 8; i++)
            {
                var controlName = "comboBox" + (i + 1);
                var control = this.Controls.Find(controlName, true).FirstOrDefault() as ComboBox;
                pdf.Polja[i] = control?.Text ?? "";
            }

            pdf.Polja[8] = textBoxDatumOd.Text;
            pdf.Polja[9] = textBoxDatumDo.Text;

            // Ako je čekiran checkbox i unet novi naziv - sačuvaj ga kao novi naziv
            if (chkMenjasNaziv.Checked && !string.IsNullOrWhiteSpace(txtNoviNazivFajla.Text))
            {
                pdf.NewFileName = textBox2.Text.Trim();
            }
            else
            {
                // Ako checkbox nije čekiran, koristi originalni naziv fajla
                pdf.NewFileName = pdf.FileName;
            }
        }
        PdfViewer pdfViewer = new PdfViewer(); // globalno
                                               //PDF PREGLED
        private void UcitajPdfUFajlViewer(string path)
        {
            OslobodiPdfViewer();

            // kreiraj temp fajl
            tempPdfPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");
            File.Copy(path, tempPdfPath, true);

            // otvori PDF iz temp fajla
            pdfViewer = new PdfViewer
            {
                Dock = DockStyle.Fill,
                Document = PdfiumViewer.PdfDocument.Load(tempPdfPath)
            };

            splitContainer1.Panel2.Controls.Clear();
            splitContainer1.Panel2.Controls.Add(pdfViewer);
        }

        // POMOCNA METODA ZA PREMESTANJE FAJLA
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

        //METODA ZA CISCENJE POLJA
        private void OcistiPolja()
        {
            // Očisti svih 8 ComboBox polja
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox4.Text = "";
            comboBox5.Text = "";
            comboBox6.Text = "";
            comboBox7.Text = "";
            comboBox8.Text = "";

            // Očisti tekst iz TextBox-ova za datume
            textBoxDatumOd.Text = "";
            textBoxDatumDo.Text = "";
        }
        // PREMESTANJE FAJLA KOJI JE OBRADJEN U FOLDER
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

        //DODAVANJE Reda u izvestaj
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

        //Dugme za sledeci
        private void btnSledeci_Click(object sender, EventArgs e)
        {
            trenutniIndex++;
            if (trenutniIndex >= pdfFajlovi.Count)
                trenutniIndex = pdfFajlovi.Count - 1;

            PrikaziTrenutniFajl();
            OcistiPolja();
        }
        //Dugme za predhodni
        private void btnPrethodni_Click(object sender, EventArgs e)
        {
            trenutniIndex--;
            if (trenutniIndex < 0)
                trenutniIndex = 0;

            PrikaziTrenutniFajl();
            OcistiPolja();
        }
        //Dugme za cuvanje izmena
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
        //Dugme za izvestaj
        private void btnIzvestaj_Click(object sender, EventArgs e)
        {

            MessageBox.Show("Pozivam generisi izvestaj");
            MessageBox.Show($"Broj PDF-ova u izveštaju: {pdfFajloviZajednicki.Count}\nPDF-ovi sa datumom: {pdfFajloviZajednicki.Count(p => p.DatumObrade != DateTime.MinValue)}");
            GenerisiIzvestajExcel();

        }
        private void btnZameniFormu_Click(object sender, EventArgs e)
        {
            OslobodiPdfViewer();
            FormMenica menicaForm = new FormMenica(inputFolderPath, outputFolderPath, operatorName, pdfFajlovi);
            menicaForm.Show();
            this.Hide();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sviFajloviObradjeni)
            {
                // Preskoči dijalog i samo sacuvaj CSV
                SacuvajPodatkeUCsv();
                return;
            }
            pdfViewer.Document?.Dispose();
            pdfViewer.Dispose();

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
        public void OtvoriPdf(string putanjaPdf)
        {
            if (string.IsNullOrEmpty(putanjaPdf) || !File.Exists(putanjaPdf))
                return;

            OslobodiPdfViewer();

            tempPdfPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");
            File.Copy(putanjaPdf, tempPdfPath, true);

            pdfViewer = new PdfViewer
            {
                Dock = DockStyle.Fill,
                Document = PdfiumViewer.PdfDocument.Load(tempPdfPath)
            };

            this.Controls.Add(pdfViewer);
        }
        private void SacuvajPodatkeUCsv()
        {
            try
            {
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

                        pdfFajloviZajednicki.Add(pdf);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri učitavanju CSV fajla: " + ex.Message);
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




        public Form1()
        {
            InitializeComponent();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void btn_Izvestaj_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
