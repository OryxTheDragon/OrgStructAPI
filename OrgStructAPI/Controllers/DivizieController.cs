using Microsoft.AspNetCore.Mvc;
using OrgStructAPI.Models;
using System.Data;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace OrgStructAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DivizieController : ControllerBase
    {
        private SqlConnection _connection = new SqlConnection();
        string _connectionString = Startup.GetConnectionString();

        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<Divizia> Get()
        {
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("SELECT * FROM Divizie", _connection);
            SqlDataReader reader = command.ExecuteReader();
            List<Divizia> _divizie = new List<Divizia>();
            while (reader.Read())
            {
                var newList = ControlaExisten.replaceDBNullsInReaderRow(reader);
                Divizia item = new Divizia(
                        (int)newList[0],
                        (string)newList[1],
                        (int?)newList[2],
                        (int)newList[3]);
                _divizie.Add(item);
            }
            _connection.Close();
            return _divizie;
        }

        // GET api/<DivizieController>/5
        [HttpGet("{id}")]
        public Object? Get(int id)
        { //Navrati ObjectResult ConflictResult/BadRequestResult alebo instanciu Divizie
            if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Divizie, id))
            {
                return BadRequest("Neexistuje divizia s danym ID!");
            }
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("SELECT * FROM Divizie WHERE id_divizie = @id_divizie", _connection);
            command.Parameters.Add("@id_divizie", SqlDbType.Int);
            command.Parameters["@id_divizie"].Value = id;

            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return Conflict("Naslo sa ID, ale databaza nenavratila hodnoty, asi bude chyba v SQL query.");
            }
            var newList = ControlaExisten.replaceDBNullsInReaderRow(reader);
            Divizia item = new Divizia(
                    (int)newList[0],
                    (string)newList[1],
                    (int?)newList[2],
                    (int)newList[3]);
            return item;
        }


        // POST api/<DivizieController>
        [HttpPost]
        public ObjectResult Post([FromHeader] string? nazov_divizie, [FromHeader] int? id_veduceho_divizie, [FromHeader] int? id_firmy_divizie)
        {

            if (nazov_divizie == null)
            {
                return BadRequest("Neprisiel nazov divizie. Na vytvorenie novej divizie potrebujete zadat nazov divizie v headeri.");
            }
            if (id_firmy_divizie == null)
            {
                return BadRequest("Neprisielo id firmy do ktorej bude patrit nova divizia. Na vytvorenie novej divizie potrebujete zadat akej firme patri. Poslite id v headeri.");
            }
            if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Firmy, (int)id_firmy_divizie))
            {
                return BadRequest("Neexistuje firma so zadanym ID. Musite poslat id existujucej firmy ktorej patri divizia.");
            }

            string query = ("INSERT INTO Divizie (nazov_divizie,id_veduceho_divizie, id_firmy_divizie) VALUES(" +
                            "@nazov_divizie,");


            if (id_veduceho_divizie != null)
            {
                if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Zamestnanci, (int)id_veduceho_divizie))
                {
                    return BadRequest("Neexistuje zamestnanec zaslany ako novy veduci divizie.");
                }
                query += "@id_veduceho_divizie,";
            }
            if (id_veduceho_divizie == null)
            {
                query += "null,";
            }
            query += "@id_firmy_divizie)";

            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@nazov_divizie", SqlDbType.VarChar).Value = nazov_divizie;
            if (id_veduceho_divizie != null)
            {
                command.Parameters.AddWithValue("@id_veduceho_divizie", SqlDbType.Int).Value = id_veduceho_divizie;
            }
            command.Parameters.AddWithValue("@id_firmy_divizie", SqlDbType.Int).Value = id_firmy_divizie;
            command.ExecuteNonQuery();
            _connection.Close();
            return Ok("Nova divizia uspesne registrovana bez vedenia.");
        }

        // PUT api/<DivizieController>/5
        [HttpPut("{id}")]
        public ObjectResult Put(int id, [FromHeader] string? nazov_divizie, [FromHeader] int? id_veduceho_divizie, [FromHeader] int? id_firmy_divizie)
        {
            if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Divizie, id))
            {
                return BadRequest("Neexistuje divizia s danym ID!");
            }
            if (nazov_divizie == null && id_veduceho_divizie == null && id_firmy_divizie == null)
            {
                return BadRequest("Neprisli ziadne udaje na upravu, zaslite udaje podla ktorych chcete upravovat v headeri.");
            }

            string query = "UPDATE Divizie SET ";

            if (nazov_divizie != null)
            {
                query += " nazov_divizie = @nazov_divizie,";
            }
            if (id_veduceho_divizie != null)
            {
                if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Zamestnanci, (int)id_veduceho_divizie))
                {
                    return BadRequest("Neexistuje zamestnanec ktory ma byt priradeny ako veduci divizie.");
                }
                query += " id_veduceho_divizie = @id_veduceho_divizie,";
            }
            if (id_firmy_divizie != null)
            {
                if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Firmy, (int)id_firmy_divizie))
                {
                    return BadRequest("Neexistuje firma ktory ktora ma byt priradena ako firma divizie.");
                }
                query += " id_firmy_divizie = @id_firmy_divizie";
            }
            if (id_firmy_divizie == null)
            {
                query = query.Remove(query.Length - 1);
            }
            query += " WHERE id_divizie = @id_divizie";

            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand(query, _connection);
            if (nazov_divizie != null)
            {
                command.Parameters.AddWithValue("@nazov_divizie", SqlDbType.VarChar).Value = nazov_divizie;
            }
            if (id_veduceho_divizie != null)
            {
                command.Parameters.AddWithValue("@id_veduceho_divizie", SqlDbType.Int).Value = id_veduceho_divizie;
            }
            if (id_firmy_divizie != null)
            {
                command.Parameters.AddWithValue("@id_firmy_divizie", SqlDbType.Int).Value = id_firmy_divizie;
            }
            command.Parameters.AddWithValue("@id_divizie", SqlDbType.Int).Value = id;
            command.ExecuteNonQuery();
            _connection.Close();
            return Ok("Uprava prebehla uspesne.");
        }

        // DELETE api/<DivizieController>/5
        [HttpDelete("{id}")]
        public ObjectResult Delete(int id)
        {
            if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Divizie, id))
            {
                return BadRequest("Neexistuje divizia s danym ID!");
            }
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("DELETE FROM Divizie WHERE id_divizie = @id_divizie", _connection);
            command.Parameters.AddWithValue("@id_divizie", SqlDbType.Int).Value = id;
            command.ExecuteNonQuery();
            _connection.Close();
            return Ok("Divizia s danym ID bola zmazana.");
        }
    }
}
