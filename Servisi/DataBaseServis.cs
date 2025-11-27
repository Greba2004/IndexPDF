using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using IndexPDF2.Modeli;

namespace IndexPDF2.Servisi
{
    public class DatabaseService
    {
        private readonly string dbPath;
        private readonly string connectionString;

        public DatabaseService(string sharedFolder)
        {
            dbPath = Path.Combine(sharedFolder, "IndexPDF.db");
            connectionString = $"Data Source={dbPath};Version=3;";
            KreirajBazuAkoNePostoji();
        }

        private void KreirajBazuAkoNePostoji()
        {
            bool dbPostoji = File.Exists(dbPath);
            if (!dbPostoji) SQLiteConnection.CreateFile(dbPath);

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string pdfTable = @"CREATE TABLE IF NOT EXISTS PdfFiles (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            OriginalPath TEXT NOT NULL,
                            NewFileName TEXT,
                            Polje1 TEXT, Polje2 TEXT, Polje3 TEXT, Polje4 TEXT,
                            Polje5 TEXT, Polje6 TEXT, Polje7 TEXT, Polje8 TEXT,
                            DatumOd TEXT, DatumDo TEXT,
                            DatumObrade DATETIME,
                            OperatorName TEXT,
                            Obradjen INTEGER DEFAULT 0,
                            ZakljucanOd TEXT
                        );";
                using (var cmd = new SQLiteCommand(pdfTable, conn)) cmd.ExecuteNonQuery();

                string izvestajiTable = @"CREATE TABLE IF NOT EXISTS Izvestaji (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                NazivIzvestaja TEXT,
                                DatumKreiranja DATETIME,
                                OperatorName TEXT
                             );";
                using (var cmd = new SQLiteCommand(izvestajiTable, conn)) cmd.ExecuteNonQuery();
            }
        }

        // Dodaje novi zapis (Obradjen = 0)
        public void DodajPdf(InputPdfFile pdf, string operatorName)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO PdfFiles 
                     (OriginalPath, NewFileName, Polje1, Polje2, Polje3, Polje4, Polje5, Polje6, Polje7, Polje8, DatumOd, DatumDo, DatumObrade, OperatorName, Obradjen)
                     VALUES (@OriginalPath, @NewFileName, @Polje1, @Polje2, @Polje3, @Polje4, @Polje5, @Polje6, @Polje7, @Polje8, @DatumOd, @DatumDo, @DatumObrade, @OperatorName, 0);";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@OriginalPath", pdf.OriginalPath);
                    cmd.Parameters.AddWithValue("@NewFileName", pdf.NewFileName ?? "");
                    for (int i = 0; i < 8; i++)
                        cmd.Parameters.AddWithValue($"@Polje{i + 1}", pdf.Polja != null && i < pdf.Polja.Length ? pdf.Polja[i] ?? "" : "");
                    cmd.Parameters.AddWithValue("@DatumOd", pdf.Polja != null && pdf.Polja.Length > 8 ? pdf.Polja[8] ?? "" : "");
                    cmd.Parameters.AddWithValue("@DatumDo", pdf.Polja != null && pdf.Polja.Length > 9 ? pdf.Polja[9] ?? "" : "");
                    cmd.Parameters.AddWithValue("@DatumObrade", pdf.DatumObrade == DateTime.MinValue ? (object)DBNull.Value : pdf.DatumObrade);
                    cmd.Parameters.AddWithValue("@OperatorName", operatorName ?? "");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Vraća sve zapise koji imaju OriginalPath = putanja
        public List<InputPdfFile> UzmiSvePdfZaPutanju(string putanja)
        {
            var lista = new List<InputPdfFile>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT * FROM PdfFiles WHERE OriginalPath = @OriginalPath";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@OriginalPath", putanja);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var pdf = MapReaderToInputPdfFile(reader);
                            lista.Add(pdf);
                        }
                    }
                }
            }
            return lista;
        }

        // Uzmi prvi neobrađeni i ne zaključani fajl; postavi ZakljucanOd = operator
        public InputPdfFile UzmiSledeciPdfZaObradu(string operatorName)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    string lockSql = @"
                UPDATE PdfFiles
                SET ZakljucanOd = @operator
                WHERE Id IN (
                    SELECT Id FROM PdfFiles
                    WHERE Obradjen = 0 AND (ZakljucanOd IS NULL OR ZakljucanOd = '')
                    ORDER BY Id LIMIT 1
                );
                SELECT changes();";

                    int affected;
                    using (var cmd = new SQLiteCommand(lockSql, conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@operator", operatorName);
                        affected = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // => Niko nije zakljucao (nema fajlova)
                    if (affected == 0)
                    {
                        tran.Commit();
                        return null;
                    }

                    // U ovom trenutku SAMO ova transakcija je uspela da zaključa taj fajl!

                    string getSql = @"
                SELECT * FROM PdfFiles
                WHERE ZakljucanOd = @operator AND Obradjen = 0
                ORDER BY Id LIMIT 1;";

                    using (var cmd = new SQLiteCommand(getSql, conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@operator", operatorName);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var pdf = MapReaderToInputPdfFile(reader);
                                tran.Commit();
                                return pdf;
                            }
                        }
                    }

                    tran.Commit();
                    return null;
                }
            }
        }

        // Oznaci kao obradjen prema OriginalPath (koristi originalnu putanju pre move-a)
        public void OznaciPdfKaoObradjen(InputPdfFile pdf)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                // Ako imamo Id -> update po Id (sigurnije). Inace fallback na OriginalPath.
                string queryById = @"UPDATE PdfFiles
                             SET Obradjen = 1,
                                 OperatorName = @operatorName,
                                 DatumObrade = @datum,
                                 ZakljucanOd = NULL,
                                 Polje1 = @Polje1, Polje2 = @Polje2, Polje3 = @Polje3, Polje4 = @Polje4,
                                 Polje5 = @Polje5, Polje6 = @Polje6, Polje7 = @Polje7, Polje8 = @Polje8,
                                 DatumOd = @DatumOd, DatumDo = @DatumDo,
                                 NewFileName = @NewFileName
                             WHERE Id = @Id;";

                string queryByPath = @"UPDATE PdfFiles
                             SET Obradjen = 1,
                                 OperatorName = @operatorname,
                                 DatumObrade = @datum,
                                 ZakljucanOd = NULL,
                                 Polje1 = @Polje1, Polje2 = @Polje2, Polje3 = @Polje3, Polje4 = @Polje4,
                                 Polje5 = @Polje5, Polje6 = @Polje6, Polje7 = @Polje7, Polje8 = @Polje8,
                                 DatumOd = @DatumOd, DatumDo = @DatumDo,
                                 NewFileName = @NewFileName
                             WHERE OriginalPath = @OriginalPath;";

                string sql = (pdf.Id > 0) ? queryById : queryByPath;

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    if (pdf.Id > 0)
                        cmd.Parameters.AddWithValue("@Id", pdf.Id);
                    else
                        cmd.Parameters.AddWithValue("@OriginalPath", pdf.OriginalPath ?? "");

                    cmd.Parameters.AddWithValue("@operatorName", pdf.OperatorName ?? "");
                    cmd.Parameters.AddWithValue("@datum", pdf.DatumObrade == DateTime.MinValue ? (object)DBNull.Value : pdf.DatumObrade);
                    cmd.Parameters.AddWithValue("@NewFileName", pdf.NewFileName ?? "");

                    for (int i = 0; i < 8; i++)
                        cmd.Parameters.AddWithValue($"@Polje{i + 1}", pdf.Polja != null && i < pdf.Polja.Length ? pdf.Polja[i] ?? "" : "");

                    cmd.Parameters.AddWithValue("@DatumOd", pdf.Polja != null && pdf.Polja.Length > 8 ? pdf.Polja[8] ?? "" : "");
                    cmd.Parameters.AddWithValue("@DatumDo", pdf.Polja != null && pdf.Polja.Length > 9 ? pdf.Polja[9] ?? "" : "");

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Uzmi sve zapise za konkretan datum (po datumu DatumObrade)
        public List<InputPdfFile> UzmiZaDatum(DateTime datum)
        {
            var lista = new List<InputPdfFile>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT * FROM PdfFiles WHERE date(DatumObrade) = @Datum";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Datum", datum.ToString("yyyy-MM-dd"));
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var pdf = MapReaderToInputPdfFile(reader);
                            lista.Add(pdf);
                        }
                    }
                }
            }
            return lista;
        }
        public List<InputPdfFile> UzmiSve()
{
    var lista = new List<InputPdfFile>();
    using (var conn = new SQLiteConnection(connectionString))
    {
        conn.Open();
        string query = "SELECT * FROM PdfFiles ORDER BY DatumObrade DESC;";
        using (var cmd = new SQLiteCommand(query, conn))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var pdf = MapReaderToInputPdfFile(reader);
                lista.Add(pdf);
            }
        }
    }
    return lista;
}


        // Helper: mapiranje reader-a u model
        private InputPdfFile MapReaderToInputPdfFile(SQLiteDataReader reader)
        {
            var orig = reader["OriginalPath"]?.ToString() ?? "";
            var pdf = new InputPdfFile(orig);
            // Id ako postoji
            if (reader["Id"] != DBNull.Value) pdf.Id = Convert.ToInt32(reader["Id"]);
            pdf.NewFileName = reader["NewFileName"]?.ToString() ?? pdf.FileName;
            pdf.Polja = new string[10];
            for (int i = 0; i < 8; i++)
                pdf.Polja[i] = reader[$"Polje{i + 1}"]?.ToString() ?? "";
            pdf.Polja[8] = reader["DatumOd"]?.ToString() ?? "";
            pdf.Polja[9] = reader["DatumDo"]?.ToString() ?? "";
            pdf.OperatorName = reader["OperatorName"]?.ToString() ?? "";
            if (reader["DatumObrade"] != DBNull.Value && !string.IsNullOrWhiteSpace(reader["DatumObrade"].ToString()))
            {
                if (DateTime.TryParse(reader["DatumObrade"].ToString(), out DateTime dt))
                    pdf.DatumObrade = dt;
            }
            return pdf;
        }
        public void AzurirajPdf(InputPdfFile pdf, string operatorName)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string queryById = @"
            UPDATE PdfFiles
            SET Polje1 = @Polje1,
                Polje2 = @Polje2,
                Polje3 = @Polje3,
                Polje4 = @Polje4,
                Polje5 = @Polje5,
                Polje6 = @Polje6,
                Polje7 = @Polje7,
                Polje8 = @Polje8,
                DatumOd = @DatumOd,
                DatumDo = @DatumDo,
                NewFileName = @NewFileName,
                DatumObrade = @DatumObrade,
                OperatorName = @OperatorName,
                Obradjen = 1,
                ZakljucanOd = NULL
            WHERE Id = @Id;";

                string queryByPath = @"
            UPDATE PdfFiles
            SET Polje1 = @Polje1,
                Polje2 = @Polje2,
                Polje3 = @Polje3,
                Polje4 = @Polje4,
                Polje5 = @Polje5,
                Polje6 = @Polje6,
                Polje7 = @Polje7,
                Polje8 = @Polje8,
                DatumOd = @DatumOd,
                DatumDo = @DatumDo,
                NewFileName = @NewFileName,
                DatumObrade = @DatumObrade,
                OperatorName = @OperatorName,
                Obradjen = 1,
                ZakljucanOd = NULL
            WHERE OriginalPath = @OriginalPath;";

                string sql = (pdf.Id > 0) ? queryById : queryByPath;

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    if (pdf.Id > 0)
                        cmd.Parameters.AddWithValue("@Id", pdf.Id);
                    else
                        cmd.Parameters.AddWithValue("@OriginalPath", pdf.OriginalPath ?? "");

                    cmd.Parameters.AddWithValue("@NewFileName", pdf.NewFileName ?? "");
                    for (int i = 0; i < 8; i++)
                        cmd.Parameters.AddWithValue($"@Polje{i + 1}", pdf.Polja != null && i < pdf.Polja.Length ? pdf.Polja[i] ?? "" : "");
                    cmd.Parameters.AddWithValue("@DatumOd", pdf.Polja != null && pdf.Polja.Length > 8 ? pdf.Polja[8] ?? "" : "");
                    cmd.Parameters.AddWithValue("@DatumDo", pdf.Polja != null && pdf.Polja.Length > 9 ? pdf.Polja[9] ?? "" : "");
                    cmd.Parameters.AddWithValue("@DatumObrade", pdf.DatumObrade == DateTime.MinValue ? (object)DBNull.Value : pdf.DatumObrade);
                    cmd.Parameters.AddWithValue("@OperatorName", operatorName ?? "");

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}