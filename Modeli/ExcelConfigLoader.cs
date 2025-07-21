using System;
using System.IO;
using ClosedXML.Excel;

namespace IndexPDF2.Modeli
{
    public static class ExcelConfigLoader
    {
        public static ConfigData UcitajKonfiguracijuIzExcel(string putanjaExcel)
        {
            var config = new ConfigData();

            if (!File.Exists(putanjaExcel))
                throw new FileNotFoundException("Excel konfiguracioni fajl nije pronađen!");

            using var workbook = new XLWorkbook(putanjaExcel);
            var worksheet = workbook.Worksheet(1); // Prvi sheet

            // Učitaj nazive polja iz prvog reda (kolone 1-8)
            for (int i = 1; i <= 8; i++)
            {
                var cellValue = worksheet.Cell(1, i).GetString().Trim();
                config.PoljaNazivi[i - 1] = cellValue;

                var obaveznoCell = worksheet.Cell(2, i).GetString().Trim().ToUpper();
                config.PoljaObavezna[i - 1] = obaveznoCell == "DA";
                List<string> vrednosti = new List<string>();
                int red = 3;

                while (true)
                {
                    var vrednost = worksheet.Cell(red, i).GetString().Trim();

                    if (string.IsNullOrEmpty(vrednost))
                        break;

                    vrednosti.Add(vrednost);
                    red++;
                }

                config.PoljaListe[i - 1] = vrednosti;
            
        }


            return config;
        }
    }
}
