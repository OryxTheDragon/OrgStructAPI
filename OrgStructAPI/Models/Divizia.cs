namespace OrgStructAPI.Models
{
    public class Divizia
    {
        public int id_divizie { get; set; }
        public string nazov_divizie { get; set; }
        public int? id_veduceho_firmy { get; set; }
        public int? id_firmy { get; set; }
    }
}
