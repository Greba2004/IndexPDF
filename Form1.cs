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
        string configExcelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xlsx");

        public Form1(string inputFolder, string outputFolder)
        {
            InitializeComponent();
            this.inputFolderPath = inputFolder;
            this.outputFolderPath = outputFolder;
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
        //Validacija datuma
        private bool ValidirajDatume()
        {
            string datumOdText = textBoxDatumOd.Text.Trim();
            string datumDoText = textBoxDatumDo.Text.Trim();

            if (!DateTime.TryParseExact(datumOdText, "dd.MM.yyyy.", null, System.Globalization.DateTimeStyles.None, out DateTime datumOd))
            {
                MessageBox.Show("Datum OD nije validan. Koristite format: dd.MM.yyyy.");
                return false;
            }

            if (!DateTime.TryParseExact(datumDoText, "dd.MM.yyyy.", null, System.Globalization.DateTimeStyles.None, out DateTime datumDo))
            {
                MessageBox.Show("Datum DO nije validan. Koristite format: dd.MM.yyyy.");
                return false;
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
                    label.Text = configData.PoljaNazivi[i];
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
            if (trenutniIndex < pdfFajlovi.Count - 1)
            {
                trenutniIndex++;
                PrikaziTrenutniFajl();
            }
            else
            {
                MessageBox.Show("Nema više fajlova.");
            }
        }
        //PRIKAZIVANJE PODATAKA NA FORMU
        private void PrikaziTrenutniFajl()
        {
            if (trenutniIndex < 0 || trenutniIndex >= pdfFajlovi.Count)
                return;

            var fajl = pdfFajlovi[trenutniIndex];

            // naziv fajla
            lblNazivPdfFajla.Text = fajl.FileName;

            if (!chkMenjasNaziv.Checked)
                txtNoviNazivFajla.Text = fajl.NewFileName;
            else
                txtNoviNazivFajla.Text = fajl.FileName;

            // učitaj PDF u desni panel
            UcitajPdfUFajlViewer(fajl.OriginalPath);

            // Očisti i postavi ComboBox vrednosti (dodaćemo kasnije)
        }
        PdfViewer pdfViewer = new PdfViewer(); // globalno
                                               //PDF PREGLED
        private void UcitajPdfUFajlViewer(string path)
        {
            if (pdfViewer != null)
            {
                pdfViewer.Dispose();
                pdfViewer = null;
            }

            pdfViewer = new PdfViewer();
            pdfViewer.Dock = DockStyle.Fill;
            pdfViewer.Document = PdfiumViewer.PdfDocument.Load(path);

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
        }
        //CUVANJE PODATAKA U TRENUTNI PDF
        private void SacuvajUnetePodatkeUTrenutniPdf()
        {
            if (trenutniIndex < 0 || trenutniIndex >= pdfFajlovi.Count)
                return;

            var pdf = pdfFajlovi[trenutniIndex];

            // Pod pretpostavkom da su ComboBox-evi i TextBox-evi numerisani kao: comboBox1, ..., comboBox8 i textBoxDatumOd, textBoxDatumDo
            for (int i = 0; i < 8; i++)
            {
                var controlName = "comboBox" + (i + 1);
                var control = this.Controls.Find(controlName, true).FirstOrDefault() as ComboBox;
                pdf.Polja[i] = control?.Text ?? "";
            }

            pdf.Polja[8] = textBoxDatumOd.Text;
            pdf.Polja[9] = textBoxDatumDo.Text;
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
            textBox1.Text = "";
            textBox2.Text = "";
            textBoxDatumOd.Text = "";
            textBoxDatumDo.Text = "";
        }
        //PREMESTANJE FAJLA KOJI JE OBRADJEN U FOLDER
        private void PremestiTrenutniPdfUFolder()
        {
            if (trenutniIndex < 0 || trenutniIndex >= pdfFajlovi.Count)
                return;

            OslobodiPdfViewer();

            var trenutniPdf = pdfFajlovi[trenutniIndex];
            string nazivFajla = Path.GetFileName(trenutniPdf.OriginalPath);
            string novaPutanja = Path.Combine(outputFolderPath, nazivFajla);

            // Premesti fajl
            File.Move(trenutniPdf.OriginalPath, novaPutanja);
        }
        //DODAVANJE Reda u izvestaj
        private void GenerisiIzvestajExcel()
        {
            var workbookPath = Path.Combine(outputFolderPath, "izvestaj.xlsx");
            var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet("Izvestaj");

            int red = 1;
            int kolona = 1;

            // ✅ Naslovi kolona
            worksheet.Cell(red, kolona++).Value = "Stari naziv fajla";
            worksheet.Cell(red, kolona++).Value = "Novi naziv fajla";

            foreach (var naziv in configData.PoljaNazivi)
                worksheet.Cell(red, kolona++).Value = naziv;

            worksheet.Cell(red, kolona++).Value = "Datum Od";
            worksheet.Cell(red, kolona).Value = "Datum Do";

            red++;

            // 🔁 Dodaj podatke za sve premeštene fajlove
            foreach (var pdf in pdfFajlovi)
            {
                string fajlPutanja = Path.Combine(outputFolderPath, pdf.NewFileName);
                if (!File.Exists(fajlPutanja))
                    continue;

                int kol = 1;
                worksheet.Cell(red, kol++).Value = pdf.FileName;
                worksheet.Cell(red, kol++).Value = pdf.NewFileName;

                for (int i = 0; i < 8; i++)
                    worksheet.Cell(red, kol++).Value = pdf.Polja[i] ?? "";

                worksheet.Cell(red, kol++).Value = pdf.Polja[8] ?? "";
                worksheet.Cell(red, kol).Value = pdf.Polja[9] ?? "";

                red++;
            }

            workbook.SaveAs(workbookPath);

            // ✅ Automatski otvori Excel
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = workbookPath,
                UseShellExecute = true
            });

            MessageBox.Show("Izveštaj je uspešno generisan i otvoren.", "Uspeh", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            // Proveri da li su obavezna polja popunjena
            if (!ValidirajObaveznaPolja())
                return;
            if (!ValidirajDatume())
                return;
            SacuvajUnetePodatkeUTrenutniPdf();
            PremestiTrenutniPdfUFolder();
            PredjiNaSledeciFajl();
            OcistiPolja();
        }
        //Dugme za izvestaj
        private void btnIzvestaj_Click(object sender, EventArgs e)
        {
            GenerisiIzvestajExcel();
            MessageBox.Show("Izveštaj je uspešno generisan!");
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
    }
}
