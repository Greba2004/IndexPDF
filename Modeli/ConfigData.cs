namespace IndexPDF2.Modeli
{
    public class ConfigData
    {
        public string[] PoljaNazivi { get; set; } = new string[8]; // Prvih 8 polja (ComboBox)
        public bool[] PoljaObavezna { get; set; } = new bool[8];
        public Dictionary<string, List<string>> VrednostiPoPoljima { get; set; } = new();
    }
}
