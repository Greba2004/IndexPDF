using IndexPDF2.Modeli;
using PdfiumViewer;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace IndexPDF2.Servisi
{
    public class PdfServis
    {
        private PdfViewer pdfViewer;
        private DatabaseService dbService;
        private string operatorName;
        private InputPdfFile trenutniPdf;
        private string inputFolderPath;

        public string ImeOperatera => operatorName;
        public InputPdfFile TrenutniPdf => trenutniPdf;
        public PdfViewer PdfViewerInstance => pdfViewer;

        public PdfServis(DatabaseService dbService, string operatorName, string inputFolder, ConfigData configData)
        {
            this.dbService = dbService;
            this.operatorName = operatorName;
            this.inputFolderPath = inputFolder;

            DodajNovePdfoveUBazu();
        }

        // ---------------------------------------------------------
        // 1) DODAVANJE NOVIH PDF FAJLOVA U BAZU
        // ---------------------------------------------------------
        private void DodajNovePdfoveUBazu()
        {
            if (!Directory.Exists(inputFolderPath)) return;

            var sviPdfFajlovi = Directory.GetFiles(inputFolderPath, "*.pdf");
            foreach (var pdfPath in sviPdfFajlovi)
            {
                if (!File.Exists(pdfPath)) continue;

                var postoje = dbService.UzmiSvePdfZaPutanju(pdfPath);
                if (postoje == null || postoje.Count == 0)
                {
                    var pdf = new InputPdfFile(pdfPath)
                    {
                        IsLocked = false,
                        LockedBy = null,
                        LockedAt = null
                    };

                    dbService.DodajPdf(pdf, operatorName);
                }
            }
        }

        // ---------------------------------------------------------
        // 2) UCITAVANJE PDF-A U VIEWER
        // ---------------------------------------------------------
        public void PrikaziTrenutniFajl(Panel panel)
        {
            OslobodiPdfViewer();
            if (trenutniPdf == null) return;

            if (File.Exists(trenutniPdf.OriginalPath))
            {
                var dokument = PdfDocument.Load(trenutniPdf.OriginalPath);
                pdfViewer = new PdfViewer
                {
                    Dock = DockStyle.Fill,
                    Document = dokument
                };

                panel.Controls.Clear();
                panel.Controls.Add(pdfViewer);
            }
        }

        private void OslobodiPdfViewer()
        {
            if (pdfViewer != null)
            {
                pdfViewer.Document?.Dispose();
                pdfViewer.Dispose();
                pdfViewer = null;
            }
        }

        public void OslobodiSvePdfResurse()
        {
            OslobodiPdfViewer();
            trenutniPdf = null;
        }

        // ---------------------------------------------------------
        // 3) UZMI SLEDECI PDF (sa zakljucavanjem)
        // ---------------------------------------------------------
        public bool UzmiSledeciPdf()
        {
            DodajNovePdfoveUBazu();

            // 1) Pokušaj da uzmeš neobrađen i nezaključan PDF
            trenutniPdf = dbService.UzmiPrviSlobodanPdf();

            if (trenutniPdf != null)
            {
                bool uspesnoZakljucao = dbService.ZakljucajPdf(trenutniPdf.Id, operatorName);
                if (!uspesnoZakljucao)
                {
                    // slučaj: između čitanja i zaključavanja ga je uzeo neko drugi
                    return UzmiSledeciPdf();
                }

                trenutniPdf.IsLocked = true;
                trenutniPdf.LockedBy = operatorName;
                trenutniPdf.LockedAt = DateTime.Now;
                return true;
            }

            // 2) Ako nema nezaključanih PDF-ova
            return false;
        }

        // ---------------------------------------------------------
        // 4) NAKON OBRADE – VRŠIMO UPDATE I PREBACIVANJE
        // ---------------------------------------------------------
        public void PremestiTrenutniPdfUFolder(string outputFolderPath)
        {
            if (trenutniPdf == null) return;

            var oldPath = trenutniPdf.OriginalPath;
            trenutniPdf.OperatorName = operatorName;
            trenutniPdf.DatumObrade = DateTime.Now;
            trenutniPdf.IsLocked = false;
            trenutniPdf.LockedBy = null;
            trenutniPdf.LockedAt = null;

            dbService.AzurirajPdf(trenutniPdf, operatorName);

            string nazivFajla = string.IsNullOrWhiteSpace(trenutniPdf.NewFileName)
                ? Path.GetFileName(trenutniPdf.OriginalPath)
                : trenutniPdf.NewFileName;

            if (!nazivFajla.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                nazivFajla += ".pdf";

            string novaPutanja = Path.Combine(outputFolderPath, nazivFajla);

            if (File.Exists(oldPath))
            {
                OslobodiPdfViewer();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                File.Move(oldPath, novaPutanja);
                trenutniPdf.OriginalPath = novaPutanja;
            }
        }
    }
}