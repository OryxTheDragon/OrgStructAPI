namespace OrgStructAPI.Models
{
    public class Oddelenie
    {
        public Oddelenie(int id_oddelenia,string nazov_oddelenia, int? id_veduceho_oddelenia, int id_projektu_oddelenia)
        {
            this.id_oddelenia = id_oddelenia;
            this.nazov_oddelenia = nazov_oddelenia;
            this.id_veduceho_oddelenia = id_veduceho_oddelenia;
            this.id_projektu_oddelenia = id_projektu_oddelenia;
        }

        public int id_oddelenia { get; set; }
        public string nazov_oddelenia { get; set; }
        public int? id_veduceho_oddelenia { get; set; }
        public int id_projektu_oddelenia { get; set; }
    }
}
