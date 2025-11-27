using ClosedXML.Excel;
using IndexPDF2.Modeli;
using System;
using System.Collections.Generic;
using System.IO;

namespace IndexPDF2.Servisi
{
    public class ConfigServis
    {
        private readonly string excelPath;

        public ConfigServis(string excelPath)
        {
            this.excelPath = excelPath;
        }

        public ConfigData UcitajKonfiguraciju()
        {
            if (string.IsNullOrEmpty(excelPath) || !File.Exists(excelPath))
                throw new FileNotFoundException("Excel konfiguracioni fajl nije pronađen!", excelPath);

            var config = new ConfigData();

            using var workbook = new XLWorkbook(excelPath);
            var worksheet = workbook.Worksheet(1);

            // Očekujemo istu strukturu kao i tvoja stara ExcelConfigLoader (header u prvom redu, "DA" u drugom)
            for (int i = 1; i <= 8; i++)
            {
                var naziv = worksheet.Cell(1, i).GetString().Trim();
                config.PoljaNazivi[i - 1] = naziv;

                var obavezno = worksheet.Cell(2, i).GetString().Trim().ToUpper();
                config.PoljaObavezna[i - 1] = obavezno == "DA";

                var vrednosti = new List<string>();
                int red = 3;
                while (true)
                {
                    var celija = worksheet.Cell(red, i).GetString()?.Trim();
                    if (string.IsNullOrEmpty(celija)) break;
                    vrednosti.Add(celija);
                    red++;
                }

                config.PoljaListe[i - 1] = vrednosti;
            }

            return config;
        }
    }
}
