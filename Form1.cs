namespace IndexPDF2
{
    using PdfiumViewer;
    using IndexPDF2.Modeli;
    public partial class Form1 : Form
    {
        //GLOBALNE VARIJABLE
        List<InputPdfFile> pdfFajlovi = new();
        int trenutniIndex = 0;
        ConfigData configData = new();

        string inputFolderPath = "";
        string outputFolderPath = "";

        string configExcelPath = "putanja/do/config.xlsx";
        // IZBOR INPUT I OUTPUT FOLDERA
        private void Form1_Load(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                MessageBox.Show("Izaberi INPUT folder");
                if (folderDialog.ShowDialog() == DialogResult.OK)
                    inputFolderPath = folderDialog.SelectedPath;
                else Close(); // prekini ako ne izabere
            }

            using (var folderDialog = new FolderBrowserDialog())
            {
                MessageBox.Show("Izaberi OUTPUT folder");
                if (folderDialog.ShowDialog() == DialogResult.OK)
                    outputFolderPath = folderDialog.SelectedPath;
                else Close();
            }

            UcitajPdfFajlove();
            PrikaziTrenutniFajl();
        }
        //UCITAVANJE PDF FAJLOVA
        private void UcitajPdfFajlove()
        {
            var fajlovi = Directory.GetFiles(inputFolderPath, "*.pdf");
            pdfFajlovi = fajlovi.Select(f => new InputPdfFile(f)).ToList();
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
