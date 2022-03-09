namespace OrgStructAPI.Models
{
    public class Oddelenia
    {
        public int id_oddelenia { get; set; }
        public string nazov_oddelenia { get; set; }
        public int? id_veduceho_oddelenia { get; set; }
        public int id_divizie { get; set; }
    }
}
