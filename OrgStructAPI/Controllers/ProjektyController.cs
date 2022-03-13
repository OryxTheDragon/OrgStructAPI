using Microsoft.AspNetCore.Mvc;
using OrgStructAPI.Models;
using System.Data;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace OrgStructAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjektyController : ControllerBase
    {
        private SqlConnection _connection = new SqlConnection();
        string _connectionString = Startup.GetConnectionString();

        // GET: api/<ProjektyControllers>
        [HttpGet]
        public IEnumerable<Projekt> Get()
        {
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("SELECT * FROM Projekty", _connection);
            SqlDataReader reader = command.ExecuteReader();
            List<Projekt> _projekty = new List<Projekt>();
            while (reader.Read())
            {
                var newList = ControlaExisten.replaceDBNullsInReaderRow(reader);
                Projekt item = new Projekt(
                    (int)newList[0],
                    (string)newList[1],
                    (string?)newList[2],
                    (int?)newList[3],
                    (int)newList[4]);
                _projekty.Add(item);
            }
            _connection.Close();
            return _projekty;
        }

        // GET api/<ProjektyControllers>/5
        [HttpGet("{id}")]
        public Object? Get(int id)
        {
            if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Projekty, id))
            {
                return BadRequest("Neexistuje projekt s danym ID!");
            }
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("SELECT * FROM Projekty WHERE id_projektu = @id_projektu", _connection);
            command.Parameters.AddWithValue("@id_projektu", SqlDbType.Int).Value = id;
            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return Conflict("Naslo sa ID, ale databaza nenavratila hodnoty, asi bude chyba v SQL query.");
            }
            var newList = ControlaExisten.replaceDBNullsInReaderRow(reader);
            Projekt item = new Projekt(
                (int)newList[0],
                (string)newList[1],
                (string?)newList[2],
                (int?)newList[3],
                (int)newList[4]);
            return item;
        }

        // POST api/<ProjektyControllers>
        [HttpPost]
        public ObjectResult Post([FromHeader] string nazov_projektu, [FromHeader] string? popis_projektu, [FromHeader] int? id_veduci_projektu, [FromHeader] int id_divizia_projektu)
        {
            if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Divizie, id_divizia_projektu))
            {
                return BadRequest("Projekt musi byt priradeny do divizie. V databaze neexistuje divizia so zadanym ID.");
            }
            if (nazov_projektu == null)
            {
                return BadRequest("Na vytvorenie projektu musite zadat meno noveho projektu.");
            }

            string query = String.Format("INSERT INTO Projekty(nazov_projektu, id_divizia_projektu, popis_projektu, id_veduci_projektu) VALUES(" +
                            "@nazov_projektu," +
                            "@id_divizia_projektu,");

            if (popis_projektu != null)
            {
                query += "@popis_projektu,";
            }
            else
            {
                query += "null,";
            }
            if (id_veduci_projektu != null)
            {
                if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Zamestnanci, (int)id_veduci_projektu))
                {
                    return BadRequest("Zamestnanec so zadanym ID ako veduci projektu neexistuje.");
                }
                query += "@id_veduci_projektu)";
            }
            else
            {
                query += "null)";
            }
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            SqlCommand command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@nazov_projektu", SqlDbType.VarChar).Value = nazov_projektu;
            command.Parameters.AddWithValue("@id_divizia_projektu", SqlDbType.Int).Value = id_divizia_projektu;
            if (popis_projektu != null)
            {
                command.Parameters.AddWithValue("@popis_projektu", SqlDbType.VarChar).Value = popis_projektu;
            }
            if (id_veduci_projektu != null)
            {
                command.Parameters.AddWithValue("@id_veduci_projektu", SqlDbType.Int).Value = id_veduci_projektu;
            }
            command.ExecuteNonQuery();
            return Ok("Projekt bol plne uspesne zavedeny do databazy.");
        }

        // PUT api/<ProjektyControllers>/5
        [HttpPut("{id}")]
        public ObjectResult Put(int id, [FromHeader] string? nazov_projektu, [FromHeader] string? popis_projektu, [FromHeader] int? id_veduci_projektu, [FromHeader] int? id_divizia_projektu)
        {
            //Praca so stringami. Davat pozor pri zmene.
            if (nazov_projektu == null && popis_projektu == null && id_veduci_projektu == null && nazov_projektu == null && id_divizia_projektu == null)
            {
                return BadRequest("Neprisli ziadne zadane hodnoty na upravu. Zaslite hodnoty ktore chcete upravit cez header.");
            }

            if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Projekty, id))
            {
                return BadRequest("Neexistuje projekt s danym ID!");
            }

            string query = String.Format("UPDATE Projekty SET ");

            if (nazov_projektu != null)
            {
                query += "nazov_projektu = @nazov_projektu, ";

            }

            if (popis_projektu != null)
            {

                query += "popis_projektu = @popis_projektu, ";

            }

            if (id_veduci_projektu != null)
            {
                if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Zamestnanci, (int)id_veduci_projektu))
                {
                    return BadRequest("Neexistuje zamestnanec ktory ma byt priradeny ako veduci projektu.");
                }
                query += "id_veduci_projektu = @id_veduci_projektu,";

            }

            if (id_divizia_projektu != null)
            {
                if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Divizie, (int)id_divizia_projektu))
                {
                    return BadRequest("Neexistuje divizia ktora ma byt priradena ako nova divizia projektu.");
                }
                query += "id_divizia_projektu = @id_divizia_projektu ";
            }
            else {
                query = query.Remove(query.Length - 1);
            }
            query += " WHERE id_projektu = @id_projektu";

            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@id_projektu", SqlDbType.VarChar).Value = id;
            if (nazov_projektu != null)
            {
                command.Parameters.AddWithValue("@nazov_projektu", SqlDbType.VarChar).Value = nazov_projektu;
            }
            if (popis_projektu != null)
            {
                command.Parameters.AddWithValue("@popis_projektu", SqlDbType.VarChar).Value = popis_projektu;
            }
            if (id_veduci_projektu != null)
            {
                command.Parameters.AddWithValue("@id_veduci_projektu", SqlDbType.VarChar).Value = id_veduci_projektu;
            }
            if (id_divizia_projektu != null)
            {
                command.Parameters.AddWithValue("@id_divizia_projektu", SqlDbType.VarChar).Value = id_divizia_projektu;
            }
            command.ExecuteNonQuery();
            _connection.Close();
            return Ok("Projekt s danym ID bol upraveny.");
        }

        // DELETE api/<ProjektyControllers>/5
        [HttpDelete("{id}")]
        public ObjectResult Delete(int id)
        {
            if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Projekty, id))
            {
                return BadRequest("Neexistuje projekt s danym ID!");
            }
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("DELETE FROM Projekty WHERE id_projektu = @id_projektu", _connection);
            command.Parameters.AddWithValue("@id_projektu", SqlDbType.Int).Value = id;
            command.ExecuteNonQuery();
            _connection.Close();
            return Ok("Projekt so zadanym ID bol zmazany.");
        }
    }
}
