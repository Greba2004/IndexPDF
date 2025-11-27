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
                    var pdf = new InputPdfFile(pdfPath);
                    dbService.DodajPdf(pdf, operatorName);
                }
            }
        }

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

        public bool UzmiSledeciPdf()
        {
            DodajNovePdfoveUBazu();

            // prvo uzmi neobrađene
            trenutniPdf = dbService.UzmiSledeciPdfZaObradu(operatorName);

            // ako nema neobrađenih, uzmi prvi PDF iz input foldera
            if (trenutniPdf == null && Directory.Exists(inputFolderPath))
            {
                var sviPdfovi = Directory.GetFiles(inputFolderPath, "*.pdf");
                foreach (var pdfPath in sviPdfovi)
                {
                    var postoje = dbService.UzmiSvePdfZaPutanju(pdfPath);
                    if (postoje != null && postoje.Count > 0)
                    {
                        trenutniPdf = postoje[0];
                        break;
                    }
                }
            }

            return trenutniPdf != null;
        }

        public void PremestiTrenutniPdfUFolder(string outputFolderPath)
        {
            if (trenutniPdf == null) return;

            var oldPath = trenutniPdf.OriginalPath;
            trenutniPdf.OperatorName = operatorName;
            trenutniPdf.DatumObrade = DateTime.Now;

            // Prepiši postojeću obradu
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
   
    

