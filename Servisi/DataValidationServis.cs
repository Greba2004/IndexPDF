using System;
using System.Globalization;
using System.Windows.Forms;

namespace IndexPDF2.Servisi
{
    public class DataValidationService
    {
        /// <summary>
        /// Validira obavezna polja (8 polja - koristi configData.PoljaObavezna)
        /// </summary>
        public bool ValidirajObaveznaPolja(string[] polja, string[] naziviPolja, bool[] poljaObavezna)
        {
            if (polja == null || naziviPolja == null || poljaObavezna == null)
                return true;

            int max = Math.Min(polja.Length, poljaObavezna.Length);
            for (int i = 0; i < Math.Min(8, max); i++)
            {
                if (poljaObavezna[i] && string.IsNullOrWhiteSpace(polja[i]))
                {
                    MessageBox.Show($"Polje '{naziviPolja[i]}' je obavezno i ne može biti prazno!");
                    return false;
                }
            }

            return true;
        }


        public bool ValidirajDatume(string datumOdText, string datumDoText)
        {
            if (!string.IsNullOrWhiteSpace(datumOdText))
            {
                if (!DateTime.TryParseExact(datumOdText, "dd.MM.yyyy.", null, DateTimeStyles.None, out _))
                {
                    MessageBox.Show("Datum OD nije validan. Koristite format: dd.MM.yyyy.");
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(datumDoText))
            {
                if (!DateTime.TryParseExact(datumDoText, "dd.MM.yyyy.", null, DateTimeStyles.None, out _))
                {
                    MessageBox.Show("Datum DO nije validan. Koristite format: dd.MM.yyyy.");
                    return false;
                }
            }

            return true;
        }
    }
}