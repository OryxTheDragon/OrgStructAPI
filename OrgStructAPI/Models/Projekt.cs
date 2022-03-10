namespace OrgStructAPI.Models
{
    public class Projekt
    {
        public Projekt(int id_projektu, string nazov_projektu, string? popis_projektu, int? id_veduci_projektu, int id_divizia_projektu)
        {
            this.id_projektu = id_projektu;
            this.nazov_projektu = nazov_projektu;
            this.popis_projektu = popis_projektu;
            this.id_veduci_projektu = id_veduci_projektu;
            this.id_divizia_projektu = id_divizia_projektu;
        }

        public int id_projektu { get; set; }
        public string nazov_projektu { get; set; }
        public string? popis_projektu { get; set; }
        public int? id_veduci_projektu { get; set; }
        public int id_divizia_projektu { get; set; }
    }
}
