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
            var worksheet = workbook.Worksheet(1);

            // Učitaj prvih 8 kolona
            for (int i = 1; i <= 8; i++)
            {
                // Naziv polja (prvi red)
                string naziv = worksheet.Cell(1, i).GetString().Trim();
                config.PoljaNazivi[i - 1] = naziv;

                // Obavezno DA/NE (drugi red)
                string obavezno = worksheet.Cell(2, i).GetString().Trim().ToUpper();
                config.PoljaObavezna[i - 1] = obavezno == "DA";

                // Lista vrednosti od trećeg reda naniže
                List<string> vrednosti = new List<string>();
                int red = 3;

                while (true)
                {
                    string val = worksheet.Cell(red, i).GetString().Trim();
                    if (string.IsNullOrEmpty(val))
                        break;

                    vrednosti.Add(val);
                    red++;
                }

                config.PoljaListe[i - 1] = vrednosti;
            }

            return config;
        }
    }
}