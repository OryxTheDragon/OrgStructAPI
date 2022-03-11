namespace OrgStructAPI.Models
{
    public class Zamestnanec
    {

        public Zamestnanec(int id_zamestnanca, string meno, string priezvisko, int? id_oddelenia, string? phone_num, string? titul)
        {
            this.id_zamestnanca = id_zamestnanca;
            this.meno = meno;
            this.priezvisko = priezvisko;
            this.id_oddelenia = id_oddelenia;
            this.phone_num = phone_num;
            this.titul = titul;
        }

        public int id_zamestnanca { get; set; }
        public string meno { get; set; }
        public string priezvisko { get; set; }
        public int? id_oddelenia { get; set; }
        public string? phone_num { get; set; }
        public string? titul { get; set; }
    }
}
