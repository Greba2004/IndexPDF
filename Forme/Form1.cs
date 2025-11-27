using IndexPDF2.Modeli;
using IndexPDF2.Servisi;
using PdfiumViewer;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IndexPDF2
{
    public partial class Form1 : Form
    {
        private ConfigServis configServis;
        private CsvServis csvServis;
        private DataValidationService validationService = new DataValidationService();
        private IzvestajServis izvestajServis;

        private ConfigData configData;
        private string inputFolderPath;
        private string outputFolderPath;
        private string operatorName;
        private string configExcelPath;
        private string csvTempPath;

        private DatabaseService dbService;
        private PdfServis pdfServis;

        public Form1(string inputFolder, string outputFolder, string operatorName)
        {
            InitializeComponent();

            this.inputFolderPath = inputFolder;
            this.outputFolderPath = outputFolder;
            this.operatorName = operatorName;
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
            this.FormClosing += Form1_FormClosing;

            // Folder u ProgramData  
            string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "IndexPDF");
            if (!Directory.Exists(appDataFolder)) Directory.CreateDirectory(appDataFolder);

            configExcelPath = Path.Combine(appDataFolder, "config.xlsx");
            csvTempPath = Path.Combine(appDataFolder, "podaci_temp.csv");

            // Inicijalizacija servisa  
            configServis = new ConfigServis(configExcelPath);
            csvServis = new CsvServis(csvTempPath, outputFolderPath);

            try
            {
                configData = configServis.UcitajKonfiguraciju();
                PostaviNazivePoljaUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri učitavanju Excel fajla: " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Učitaj CSV prethodnih podataka  
            var prethodniPodaci = csvServis.UcitajPodatkeIzCsv();

            // Inicijalizacija baze i PdfServis
            dbService = new DatabaseService(inputFolderPath);
            pdfServis = new PdfServis(dbService, operatorName, inputFolderPath, configData);

            // Uzmi prvi PDF za obradu
            if (pdfServis.UzmiSledeciPdf())
            {
                PrikaziTrenutniFajl();
            }

            // **UBACIVANJE SVIH PDF-OVA IZ INPUT FOLDER-A U BAZU AKO NISU VEĆ UBACENI**
            var sviPdfFajlovi = Directory.GetFiles(inputFolderPath, "*.pdf");
            foreach (var pdfPath in sviPdfFajlovi)
            {
                // Prvo proveri da li fajl postoji na disku
                if (!File.Exists(pdfPath))
                {
                    MessageBox.Show($"Fajl ne postoji: {pdfPath}");
                    continue;
                }

                // Provera da li je već u bazi
                var postojiList = dbService.UzmiSvePdfZaPutanju(pdfPath);
                if (postojiList == null)
                {
                    MessageBox.Show($"Greška: UzmiSvePdfZaPutanju vratio null za {pdfPath}");
                    continue;
                }

                if (postojiList.Any())
                {
                    // Fajl je već u bazi, preskoči ga
                    continue;
                }

                // Pokušaj dodavanja u bazu (OBRADJEN = 0)
                try
                {
                    var pdf = new InputPdfFile(pdfPath);
                    dbService.DodajPdf(pdf, operatorName); // DodajPdf sada ubacuje Obradjen = 0
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Greška pri dodavanju u bazu za {pdfPath}: {ex.Message}");
                }
            }

            // Uzmi prvi PDF za obradu
            if (pdfServis.UzmiSledeciPdf())
            {
                PrikaziTrenutniFajl();
            }

            // Inicijalizuj servis za izveštaj  
            izvestajServis = new IzvestajServis(outputFolderPath, dbService, configData);

            // Poveži dugmad sa event handler-ima  
            button1.Click += btnPrethodni_Click; // Može da se ukloni jer više nema prethodni/sledeći
            button2.Click += btnSledeci_Click;   // Može da se ukloni ili prepraviti
            btn_Izvestaj.Click += btnIzvestaj_Click;
            btnSacuvaj.Click += btnSacuvaj_Click;
            chkMenjasNaziv.CheckedChanged += chkMenjasNaziv_CheckedChanged;
            btnZameniFormu.Click += BtnZameniFormu_Click;
            btnNadjiPdf.Click += BtnNadjiPdf_Click;

        }
        private void PostaviNazivePoljaUI()
        {
            for (int i = 0; i < 8; i++)
            {
                // Postavi naziv iz config-a
                PoljaLabel[i].Text = configData.PoljaNazivi[i] + (configData.PoljaObavezna[i] ? " *" : "");
                PoljaLabel[i].ForeColor = configData.PoljaObavezna[i] ? Color.Red : Color.MidnightBlue;

                // Postavi AutoComplete listu iz config-a
                if (configData.PoljaListe[i] != null)
                {
                    PoljaComboBox[i].AutoCompleteCustomSource.Clear();
                    PoljaComboBox[i].AutoCompleteCustomSource.AddRange(configData.PoljaListe[i].ToArray());
                    PoljaComboBox[i].AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    PoljaComboBox[i].AutoCompleteSource = AutoCompleteSource.CustomSource;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !(ActiveControl is Button))
            {
                btnSacuvaj.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void PrikaziTrenutniFajl()
        {
            pdfServis.PrikaziTrenutniFajl(splitContainer1.Panel2);

            var pdf = pdfServis.TrenutniPdf;
            if (pdf == null) return;

            lblNazivPdfFajla.Text = pdf.FileName;
            chkMenjasNaziv.Checked = false;
            textBox2.Enabled = false;
            textBox2.Text = pdf.FileName;

            OcistiPolja();
        }

        private void OcistiPolja()
        {
            comboBox1.Text = ""; comboBox2.Text = ""; comboBox3.Text = ""; comboBox4.Text = "";
            comboBox5.Text = ""; comboBox6.Text = ""; comboBox7.Text = ""; comboBox8.Text = "";
            textBoxDatumOd.Text = ""; textBoxDatumDo.Text = "";
        }

        private void btnSledeci_Click(object sender, EventArgs e)
        {
            if (pdfServis.UzmiSledeciPdf())
            {
                PrikaziTrenutniFajl();
            }
            else
            {
                MessageBox.Show("Nema više PDF-ova za obradu.");
            }
        }

        private void btnPrethodni_Click(object sender, EventArgs e)
        {
            // više ne koristi prethodni, može da se ukloni
        }

        private void btnSacuvaj_Click(object sender, EventArgs e)
        {
            var pdf = pdfServis.TrenutniPdf;
            if (pdf == null) return;

            // Uzmi podatke iz comboBox-eva
            var cbovi = new ComboBox[] { comboBox1, comboBox2, comboBox3, comboBox4, comboBox5, comboBox6, comboBox7, comboBox8 };
            string[] polja = new string[10];
            for (int i = 0; i < 8; i++)
                polja[i] = cbovi[i].Text?.Trim() ?? "";

            polja[8] = textBoxDatumOd.Text?.Trim() ?? "";
            polja[9] = textBoxDatumDo.Text?.Trim() ?? "";

            // Validacija
            if (!validationService.ValidirajObaveznaPolja(polja, configData.PoljaNazivi, configData.PoljaObavezna)) return;
            if (!validationService.ValidirajDatume(polja[8], polja[9])) return;

            // Snimi u PDF objekt
            pdf.Polja = polja;
            if (chkMenjasNaziv.Checked && !string.IsNullOrWhiteSpace(textBox2.Text))
                pdf.NewFileName = textBox2.Text.Trim();

            // Premesti fajl u output folder i obeleži u bazi
            pdfServis.PremestiTrenutniPdfUFolder(outputFolderPath);

            // Uzmi sledeći PDF
            if (pdfServis.UzmiSledeciPdf())
            {
                PrikaziTrenutniFajl();
            }
            else
            {
                MessageBox.Show("Obradili ste sve PDF-ove.");

                string izvestajPutanja = Path.Combine(outputFolderPath, "Izvestaj.xlsx");

                // Provera da li je prethodni izveštaj još otvoren
                if (File.Exists(izvestajPutanja))
                {
                    try
                    {
                        // pokušaj otvoriti fajl ekskluzivno; ako je otvoren, izbaci upozorenje
                        using (var stream = new FileStream(izvestajPutanja, FileMode.Open, FileAccess.ReadWrite))
                        {
                            // fajl nije otvoren, može da se generiše novi izveštaj
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Zatvorite prethodni izveštaj pre kreiranja novog!");
                        return;
                    }
                }

                // Generiši izveštaj
                try
                {
                    var izv = new IndexPDF2.Servisi.IzvestajServis(outputFolderPath, dbService, configData);
                    izv.GenerisiIzvestajExcel(DateTime.Today, pdfServis.ImeOperatera);
                    MessageBox.Show("Izveštaj je uspešno generisan i otvoren!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Greška prilikom generisanja izveštaja: " + ex.Message);
                }
            }

        }

        private void btnIzvestaj_Click(object sender, EventArgs e)
        {
            izvestajServis.GenerisiIzvestajExcel(DateTime.Today, pdfServis.ImeOperatera);
        }

        private void chkMenjasNaziv_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = chkMenjasNaziv.Checked;
            if (chkMenjasNaziv.Checked) textBox2.Text = "";
            else textBox2.Text = pdfServis.TrenutniPdf?.FileName ?? "";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Da li želite da izađete iz aplikacije?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) { e.Cancel = true; return; }

           
            pdfServis.OslobodiSvePdfResurse();
        }

        private void BtnZameniFormu_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Ovo dugme može otvoriti drugu formu (menice)");
        }

        private void BtnNadjiPdf_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Pretraga PDF više nije implementirana jer se radi iz baze.");
        }
    }

}