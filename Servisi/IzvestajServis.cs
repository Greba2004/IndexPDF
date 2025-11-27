using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;
using IndexPDF2.Modeli;
using System.Collections.Generic;

namespace IndexPDF2.Servisi
{
    public class IzvestajServis
    {
        private readonly string outputFolderPath;
        private readonly DatabaseService dbService;
        private readonly ConfigData configData;

        public IzvestajServis(string outputFolderPath, DatabaseService dbService, ConfigData configData)
        {
            this.outputFolderPath = outputFolderPath;
            this.dbService = dbService;
            this.configData = configData;
        }

        public void GenerisiIzvestajExcel(DateTime datum, string operatorName)
        {
            try
            {
                var pdfovi = dbService.UzmiZaDatum(datum)
                      .Where(p => string.IsNullOrEmpty(operatorName) || p.OperatorName == operatorName)
                      .GroupBy(p => p.OriginalPath) // grupiši po putanji fajla
                      .Select(g => g.OrderByDescending(x => x.DatumObrade).First()) // uzmi poslednju obradu
                      .OrderBy(p => p.DatumObrade)
                      .ToList();

                if (!pdfovi.Any())
                {
                    MessageBox.Show("Nema PDF fajlova za izabrani datum/operatera.", "Obaveštenje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string archiveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SviIzvestaji");
                if (!Directory.Exists(archiveFolder)) Directory.CreateDirectory(archiveFolder);

                string workbookPath = Path.Combine(outputFolderPath, "izvestaj.xlsx");

                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.AddWorksheet("Izvestaj");

                    int red = 1;
                    int kolona = 1;

                    ws.Cell(red, kolona++).Value = "Stari naziv fajla";
                    ws.Cell(red, kolona++).Value = "Novi naziv fajla";

                    for (int i = 0; i < configData.PoljaNazivi.Length; i++)
                        ws.Cell(red, kolona++).Value = configData.PoljaNazivi[i];

                    ws.Cell(red, kolona++).Value = "Datum Od";
                    ws.Cell(red, kolona++).Value = "Datum Do";
                    ws.Cell(red, kolona++).Value = "Datum obrade";
                    ws.Cell(red, kolona).Value = "Ime operatera";

                    red++;

                    foreach (var pdf in pdfovi)
                    {
                        int k = 1;
                        ws.Cell(red, k++).Value = pdf.FileName;
                        ws.Cell(red, k++).Value = string.IsNullOrWhiteSpace(pdf.NewFileName) ? pdf.FileName : pdf.NewFileName;

                        for (int i = 0; i < 8; i++)
                            ws.Cell(red, k++).Value = pdf.Polja != null && i < pdf.Polja.Length ? pdf.Polja[i] ?? "" : "";

                        ws.Cell(red, k++).Value = pdf.Polja != null && pdf.Polja.Length > 8 ? pdf.Polja[8] ?? "" : "";
                        ws.Cell(red, k++).Value = pdf.Polja != null && pdf.Polja.Length > 9 ? pdf.Polja[9] ?? "" : "";
                        ws.Cell(red, k++).Value = pdf.DatumObrade != DateTime.MinValue ? pdf.DatumObrade.ToString("dd.MM.yyyy. HH:mm:ss") : "";
                        ws.Cell(red, k).Value = pdf.OperatorName ?? "";

                        red++;
                    }

                    ws.Columns().AdjustToContents();
                    workbook.SaveAs(workbookPath);

                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                    string archivePath = Path.Combine(archiveFolder, $"Izvestaj_{timestamp}.xlsx");
                    workbook.SaveAs(archivePath);
                }

                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = workbookPath,
                        UseShellExecute = true
                    });
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri generisanju izveštaja: " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

}