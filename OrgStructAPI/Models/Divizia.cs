namespace OrgStructAPI.Models
{
    public class Divizia
    {

        public Divizia(int id_divizie, string nazov_divizie, int? id_veduceho_firmy, int id_firmy)
        {
            this.id_divizie = id_divizie;
            this.nazov_divizie = nazov_divizie;
            this.id_veduceho_firmy = id_veduceho_firmy;
            this.id_firmy = id_firmy;
        }

        public int id_divizie { get; set; }
        public string nazov_divizie { get; set; }
        public int? id_veduceho_firmy { get; set; }
        public int id_firmy { get; set; }
    }
}
