namespace OrgStructAPI.Models
{
    public class Zamestnanec
    {

        public Zamestnanec(int id_zamestnanca, string meno, string priezvisko, int id_firmy_zamestnanca, string? phone_num, string? titul)
        {
            this.id_zamestnanca = id_zamestnanca;
            this.meno = meno;
            this.priezvisko = priezvisko;
            this.phone_num = phone_num;
            this.titul = titul;
            this.id_firmy_zamestnanca = id_firmy_zamestnanca;
        }

        public int id_zamestnanca { get; set; }
        public string meno { get; set; }
        public string priezvisko { get; set; }
        public int id_firmy_zamestnanca { get; set; }
        public string? phone_num { get; set; }
        public string? titul { get; set; }
    }
}
