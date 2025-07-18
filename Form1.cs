namespace IndexPDF2
{
    using PdfiumViewer;
    using IndexPDF2.Modeli;
    using System.IO;
    using System.Linq;
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
        //POSTAVLJANJE POLJA
        private void PostaviNazivePoljaUI()
        {
            for (int i = 0; i < 8; i++)
            {
                string labelName = "label" + (i + 4); // od label4 do label11
                var label = this.Controls.Find(labelName, true).FirstOrDefault() as Label;
                if (label != null)
                {
                    label.Text = configData.PoljaNazivi[i];
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
        //Dugme za sledeci
        private void btnSledeci_Click(object sender, EventArgs e)
        {
            trenutniIndex++;
            if (trenutniIndex >= pdfFajlovi.Count)
                trenutniIndex = pdfFajlovi.Count - 1;

            PrikaziTrenutniFajl();
        }
        //Dugme za predhodni
        private void btnPrethodni_Click(object sender, EventArgs e)
        {
            trenutniIndex--;
            if (trenutniIndex < 0)
                trenutniIndex = 0;

            PrikaziTrenutniFajl();
        }
        //Dugme za cuvanje izmena
        private void btnSacuvaj_Click(object sender, EventArgs e)
        {
            // Proveri da li su obavezna polja popunjena
            if (!ValidirajObaveznaPolja())
                return;

            PremestiTrenutniPdfUFolder();
            PredjiNaSledeciFajl();
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
    }
}
