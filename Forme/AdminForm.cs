using IndexPDF2.Servisi;
using IndexPDF2.Modeli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IndexPDF2.Forme
{
    public partial class AdminForm : Form
    {
        private readonly DatabaseService db;
        private List<InputPdfFile> sviZapisi = new List<InputPdfFile>();

        public AdminForm(DatabaseService dbService)
        {
            InitializeComponent();
            db = dbService;

            btnPrimeni.Click += BtnPrimeni_Click;
            btnReset.Click += BtnReset_Click;
            btnExport.Click += BtnExport_Click;  // ← Poveži dugme iz designer-a

            txtPretraga.TextChanged += TxtPretraga_TextChanged;

            // Inicijalno učitaj sve zapise
            UcitajSveZapise();
            UcitajOperatere();
        }

        private void UcitajOperatere()
        {
            var operateri = sviZapisi.Select(x => x.OperatorName)
                                     .Where(x => !string.IsNullOrWhiteSpace(x))
                                     .Distinct()
                                     .OrderBy(x => x)
                                     .ToList();

            comboOperater.Items.Clear();
            comboOperater.Items.Add("(svi)");
            comboOperater.Items.AddRange(operateri.ToArray());
            comboOperater.SelectedIndex = 0;
        }

        private void UcitajSveZapise()
        {
            sviZapisi = db.UzmiSve(); // <-- ovo popunjava listu koja se koristi za filter i export

            // Za DataGridView
            var dgvLista = sviZapisi.Select(p => new
            {
                StariNaziv = p.FileName,
                NoviNaziv = string.IsNullOrWhiteSpace(p.NewFileName) ? p.FileName : p.NewFileName,
                Polje1 = p.Polja.Length > 0 ? p.Polja[0] : "",
                Polje2 = p.Polja.Length > 1 ? p.Polja[1] : "",
                Polje3 = p.Polja.Length > 2 ? p.Polja[2] : "",
                Polje4 = p.Polja.Length > 3 ? p.Polja[3] : "",
                Polje5 = p.Polja.Length > 4 ? p.Polja[4] : "",
                Polje6 = p.Polja.Length > 5 ? p.Polja[5] : "",
                Polje7 = p.Polja.Length > 6 ? p.Polja[6] : "",
                Polje8 = p.Polja.Length > 7 ? p.Polja[7] : "",
                DatumOd = p.Polja.Length > 8 ? p.Polja[8] : "",
                DatumDo = p.Polja.Length > 9 ? p.Polja[9] : "",
                DatumObrade = p.DatumObrade != DateTime.MinValue ? p.DatumObrade.ToString("dd.MM.yyyy. HH:mm:ss") : "",
                Operater = p.OperatorName
            }).ToList();

            dgvPodaci.DataSource = dgvLista;
        }

        private void BtnPrimeni_Click(object sender, EventArgs e)
        {
            PrimeniFilter();
        }


        private void BtnReset_Click(object sender, EventArgs e)
        {
            comboOperater.SelectedIndex = 0;
            dateOd.Value = DateTime.Now.AddMonths(-1);
            dateDo.Value = DateTime.Now;
            (Controls.Find("txtPretraga", true).FirstOrDefault() as TextBox).Text = "";
            dgvPodaci.DataSource = sviZapisi.OrderByDescending(x => x.DatumObrade).ToList();
        }

        private void TxtPretraga_TextChanged(object sender, EventArgs e)
        {
            PrimeniFilter();
        }

        private void PrimeniFilter()
        {
            var lista = sviZapisi.AsEnumerable();

            // Filter po operatoru
            if (comboOperater.SelectedIndex > 0)
            {
                string op = comboOperater.SelectedItem.ToString();
                lista = lista.Where(x => x.OperatorName == op);
            }

            // Filter po datumu
            lista = lista.Where(x =>
                x.DatumObrade >= dateOd.Value.Date &&
                x.DatumObrade <= dateDo.Value.Date.AddDays(1)
            );

            // Filter po nazivu fajla
            var txt = (Controls.Find("txtPretraga", true).FirstOrDefault() as TextBox)?.Text;
            if (!string.IsNullOrWhiteSpace(txt))
            {
                lista = lista.Where(x =>
                    x.FileName.Contains(txt, StringComparison.OrdinalIgnoreCase) ||
                    (x.NewFileName?.Contains(txt, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            dgvPodaci.DataSource = lista.OrderByDescending(x => x.DatumObrade).ToList();
        }
        private List<InputPdfFile> FiltrirajZaExport()
        {
            var lista = sviZapisi.AsEnumerable();

            // Filter po operatoru
            if (comboOperater.SelectedIndex > 0)
            {
                string op = comboOperater.SelectedItem.ToString();
                lista = lista.Where(x => x.OperatorName == op);
            }

            // Filter po datumu
            lista = lista.Where(x =>
                x.DatumObrade >= dateOd.Value.Date &&
                x.DatumObrade <= dateDo.Value.Date.AddDays(1)
            );

            // Filter po nazivu fajla (pretraga)
            var txt = (Controls.Find("txtPretraga", true).FirstOrDefault() as TextBox)?.Text;
            if (!string.IsNullOrWhiteSpace(txt))
            {
                lista = lista.Where(x =>
                    x.FileName.Contains(txt, StringComparison.OrdinalIgnoreCase) ||
                    (x.NewFileName?.Contains(txt, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            return lista.OrderByDescending(x => x.DatumObrade).ToList();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                var listaZaExport = FiltrirajZaExport(); // vraća List<InputPdfFile>

                if (!listaZaExport.Any())
                {
                    MessageBox.Show("Nema podataka za export.", "Obaveštenje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string privremeniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tmp_izvestaj.xlsx");
                using (var workbook = new ClosedXML.Excel.XLWorkbook())
                {
                    var ws = workbook.AddWorksheet("Izvestaj");
                    int red = 1;
                    int kolona = 1;

                    ws.Cell(red, kolona++).Value = "Stari naziv fajla";
                    ws.Cell(red, kolona++).Value = "Novi naziv fajla";
                    for (int i = 0; i < 8; i++)
                        ws.Cell(red, kolona++).Value = $"Polje{i + 1}";
                    ws.Cell(red, kolona++).Value = "Datum Od";
                    ws.Cell(red, kolona++).Value = "Datum Do";
                    ws.Cell(red, kolona++).Value = "Datum obrade";
                    ws.Cell(red, kolona).Value = "Ime operatera";
                    red++;

                    foreach (var pdf in listaZaExport)
                    {
                        kolona = 1;
                        ws.Cell(red, kolona++).Value = pdf.FileName;
                        ws.Cell(red, kolona++).Value = string.IsNullOrWhiteSpace(pdf.NewFileName) ? pdf.FileName : pdf.NewFileName;
                        for (int i = 0; i < 8; i++)
                            ws.Cell(red, kolona++).Value = pdf.Polja != null && i < pdf.Polja.Length ? pdf.Polja[i] ?? "" : "";
                        ws.Cell(red, kolona++).Value = pdf.Polja != null && pdf.Polja.Length > 8 ? pdf.Polja[8] ?? "" : "";
                        ws.Cell(red, kolona++).Value = pdf.Polja != null && pdf.Polja.Length > 9 ? pdf.Polja[9] ?? "" : "";
                        ws.Cell(red, kolona++).Value = pdf.DatumObrade != DateTime.MinValue ? pdf.DatumObrade.ToString("dd.MM.yyyy. HH:mm:ss") : "";
                        ws.Cell(red, kolona).Value = pdf.OperatorName ?? "";
                        red++;
                    }

                    ws.Columns().AdjustToContents();
                    workbook.SaveAs(privremeniPath);
                }

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = privremeniPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri exportu: " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}