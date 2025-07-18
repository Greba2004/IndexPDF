using System.IO;  // jer koristiš Path i File

namespace IndexPDF2.Modeli
{
    public class InputPdfFile
    {
        public string OriginalPath { get; set; }
        public string FileName => Path.GetFileName(OriginalPath);
        public string NewFileName { get; set; }

        public string[] Polja { get; set; } = new string[10];

        public InputPdfFile(string path)
        {
            OriginalPath = path;
            NewFileName = FileName;
        }
    }
}

