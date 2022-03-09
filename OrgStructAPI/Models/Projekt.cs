namespace OrgStructAPI.Models
{
    public class Projekt
    {
        public int id_projektu { get; set; }
        public string nazov_projektu { get; set; }
        public string? popis_projektu { get; set; }
        public int? id_veduci_projektu { get; set; }
    }
}
