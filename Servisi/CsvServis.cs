using IndexPDF2.Modeli;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IndexPDF2.Servisi
{
    public class CsvServis
    {
        private readonly string csvPath;
        private readonly string outputFolderPath;

        public CsvServis(string csvPath, string outputFolderPath)
        {
            this.csvPath = csvPath;
            this.outputFolderPath = outputFolderPath;
        }

        public void SacuvajPodatkeUCsv(List<InputPdfFile> pdfFajlovi, string operatorName)
        {
            try
            {
                var pdfZaCuvanje = pdfFajlovi
                    .Where(p => File.Exists(p.OriginalPath) && p.OriginalPath.StartsWith(outputFolderPath))
                    .ToList();

                using (var writer = new StreamWriter(csvPath, false))
                {
                    foreach (var pdf in pdfZaCuvanje)
                    {
                        string poljaJoined = pdf.Polja != null ? string.Join("|", pdf.Polja) : "";
                        string linija = string.Join(";", new string[]
                        {
                            pdf.FileName,
                            pdf.NewFileName ?? "",
                            poljaJoined,
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

        public List<InputPdfFile> UcitajPodatkeIzCsv()
        {
            var pdfFajlovi = new List<InputPdfFile>();
            try
            {
                if (!File.Exists(csvPath)) return pdfFajlovi;

                var lines = File.ReadAllLines(csvPath);
                foreach (var line in lines)
                {
                    var delovi = line.Split(';');
                    if (delovi.Length >= 5)
                    {
                        var pdf = new InputPdfFile(delovi[0])
                        {
                            NewFileName = delovi[1]
                        };

                        // Polja su spojena sa '|'
                        var poljaStr = delovi[2];
                        if (!string.IsNullOrEmpty(poljaStr))
                        {
                            var polja = poljaStr.Split('|');
                            // Osiguraj dužinu 10 (kako stari kod očekuje)
                            var arr = new string[10];
                            for (int i = 0; i < Math.Min(polja.Length, 10); i++)
                                arr[i] = polja[i];
                            pdf.Polja = arr;
                        }

                        if (DateTime.TryParse(delovi[3], out var dt))
                            pdf.DatumObrade = dt;

                        pdfFajlovi.Add(pdf);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri učitavanju CSV fajla: " + ex.Message);
            }

            return pdfFajlovi;
        }
    }
}