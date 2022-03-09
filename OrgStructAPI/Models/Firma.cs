namespace OrgStructAPI.Models
{
    public class Firma
    {
        public Firma(int? v1, string? v2, int? v3)
        {
            this.id_firmy = v1;
            this.nazov_firmy = v2;
            this.id_riaditel_firmy = v3;
        }

        public int? id_firmy { get; set; }
        public string? nazov_firmy { get; set; }
        public int? id_riaditel_firmy { get; set; }
    }
}
