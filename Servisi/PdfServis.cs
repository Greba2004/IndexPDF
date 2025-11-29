using IndexPDF2.Modeli;
using Syncfusion.Windows.Forms.PdfViewer;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace IndexPDF2.Servisi
{
    public class PdfServis
    {
        private PdfViewerControl pdfViewer;
        private DatabaseService dbService;
        private string operatorName;
        private InputPdfFile trenutniPdf;
        private string inputFolderPath;

        public string ImeOperatera => operatorName;
        public InputPdfFile TrenutniPdf => trenutniPdf;
        public PdfViewerControl PdfViewerInstance => pdfViewer;

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
        // 2) UCITAVANJE PDF-A U VIEWER (SYNCFUSION)
        // ---------------------------------------------------------
        public void PrikaziTrenutniFajl(Panel panel)
        {
            OslobodiPdfViewer();

            if (trenutniPdf == null) return;

            if (File.Exists(trenutniPdf.OriginalPath))
            {
                pdfViewer = new PdfViewerControl
                {
                    Dock = DockStyle.Fill,
                    EnableContextMenu = true,
                    IsTextSelectionEnabled = true,   // OCR sloj će biti automatski prepoznat
                    ShowToolBar = true
                };

                pdfViewer.Load(trenutniPdf.OriginalPath);

                panel.Controls.Clear();
                panel.Controls.Add(pdfViewer);
            }
        }

        private void OslobodiPdfViewer()
        {
            if (pdfViewer != null)
            {
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

            trenutniPdf = dbService.UzmiPrviSlobodanPdf();

            if (trenutniPdf != null)
            {
                bool uspesnoZakljucao = dbService.ZakljucajPdf(trenutniPdf.Id, operatorName);
                if (!uspesnoZakljucao)
                {
                    return UzmiSledeciPdf();
                }

                trenutniPdf.IsLocked = true;
                trenutniPdf.LockedBy = operatorName;
                trenutniPdf.LockedAt = DateTime.Now;
                return true;
            }

            return false;
        }

        // ---------------------------------------------------------
        // 4) NAKON OBRADE — UPDATE + PREMEŠTANJE
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