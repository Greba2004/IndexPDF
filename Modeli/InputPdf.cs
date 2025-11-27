using System;
using System.IO;

namespace IndexPDF2.Modeli
{
    public class InputPdfFile
    {
        public int Id { get; set; } // mapira se na Id u bazi (0 ako nepoznato)
        public string OriginalPath { get; set; }
        public string FileName => Path.GetFileName(OriginalPath);
        public string NewFileName { get; set; }
        public bool IsLocked { get; set; } = false;
        public string LockedBy { get; set; } = null;
        public DateTime? LockedAt { get; set; } = null;

        public string[] Polja { get; set; } = new string[10];
        public DateTime DatumObrade { get; set; } = DateTime.MinValue;

        public string OperatorName { get; set; } = "";

        public InputPdfFile(string path)
        {
            OriginalPath = path;
            NewFileName = FileName;
        }
    }
}