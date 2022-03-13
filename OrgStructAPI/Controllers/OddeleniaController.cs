using Microsoft.AspNetCore.Mvc;
using OrgStructAPI.Models;
using System.Data;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace OrgStructAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OddeleniaController : ControllerBase
    {
        private SqlConnection _connection = new SqlConnection();
        string _connectionString = Startup.GetConnectionString();

        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<Oddelenie> Get()
        {
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("SELECT * FROM Oddelenia", _connection);
            SqlDataReader reader = command.ExecuteReader();
            List<Oddelenie> _oddelenia = new List<Oddelenie>();
            while (reader.Read())
            {
                var newList = ControlaExisten.replaceDBNullsInReaderRow(reader);
                Oddelenie item = new Oddelenie(
                       (int)newList[0],
                       (string)newList[1],
                       (int?)newList[2],
                       (int)newList[3]);
                _oddelenia.Add(item);
            }
            _connection.Close();
            return _oddelenia;
        }

        // GET api/<OddeleniaControllers>/5
        [HttpGet("{id}")]
        public Object? Get(int id)
        {
            if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Oddelenia, id))
            {
                return BadRequest("Neexistuje oddelenie s danym ID!");
            }
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("SELECT * FROM Oddelenia WHERE id_oddelenia = @id_oddelenia", _connection);
            command.Parameters.AddWithValue("@id_oddelenia", SqlDbType.Int).Value = id;
            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return Conflict("Naslo sa ID, ale databaza nenavratila hodnoty, asi bude chyba v SQL query.");
            }
            var newList = ControlaExisten.replaceDBNullsInReaderRow(reader);
            Oddelenie item = new Oddelenie(
                   (int)newList[0],
                   (string)newList[1],
                   (int?)newList[2],
                   (int)newList[3]);
            return (item);
        }

        // POST api/<OddeleniaControllers>
        [HttpPost]
        public ObjectResult Post([FromHeader] string? nazov_oddelenia, [FromHeader] int? id_veduceho_oddelenia, [FromHeader] int? id_projektu_oddelenia)
        {
            if (nazov_oddelenia == null)
            {
                return BadRequest("Na zaregistrovanie noveho oddelenia je potreba uviest nazov. Poslite nazov v headeri.");
            }
            if (id_projektu_oddelenia == null)
            {
                return BadRequest("Na zaregistrovanie noveho oddelenia je potreba uviest id projektu daneho oddelenia. Poslite nazov v headeri.");
            }
            if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Projekty, (int)id_projektu_oddelenia))
            {
                return BadRequest("Projekt zaslany cez id ako projekt oddelenia neexistuje.");
            }
            string query = ("INSERT INTO Oddelenia (nazov_oddelenia,id_veduceho_oddelenia, id_projektu_oddelenia) VALUES(" +
                            "@nazov_oddelenia");

            if (id_veduceho_oddelenia != null)
            {
                if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Zamestnanci, (int)id_veduceho_oddelenia))
                {
                    return BadRequest("Zamestnanec zaslany s id ako veduci oddelenia neexistuje.");
                }
                query += ",@id_veduceho_oddelenia ";
            }

            if (id_veduceho_oddelenia == null)
            {
                query += ",null ";
            }

            query += ",@id_projektu_oddelenia)";

            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@nazov_oddelenia", SqlDbType.VarChar).Value = nazov_oddelenia;
            if (id_veduceho_oddelenia != null)
            {
                command.Parameters.AddWithValue("@id_veduceho_oddelenia", SqlDbType.Int).Value = id_veduceho_oddelenia;
            }
            command.Parameters.AddWithValue("@id_projektu_oddelenia", SqlDbType.Int).Value = id_projektu_oddelenia;
            command.ExecuteNonQuery();
            _connection.Close();

            return Ok("Nove oddelenie uspesne zaregistrovane.");
        }

        // PUT api/<OddeleniaControllers>/5
        [HttpPut("{id}")]
        public ObjectResult Put(int id, [FromHeader] string? nazov_oddelenia, [FromHeader] int? id_veduceho_oddelenia, [FromHeader] int? id_projektu_oddelenia)
        {
            if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Oddelenia, id))
            {
                return BadRequest("Neexistuje oddelenie s danym ID!");
            }

            if (id_projektu_oddelenia == null && id_veduceho_oddelenia == null && nazov_oddelenia == null)
            {
                return BadRequest("Neprisli ziadne udaje na upravu, zaslite udaje podla ktorych chcete upravovat v headeri.");
            }

            string query = String.Format("UPDATE Oddelenia SET ");

            if (nazov_oddelenia != null)
            {
                query += " nazov_oddelenia = @nazov_oddelenia,";
            }
            if (id_veduceho_oddelenia != null)
            {
                if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Zamestnanci, (int)id_veduceho_oddelenia))
                {
                    return BadRequest("Neexistuje zamestnanec ktory ma byt priradeny ako veduci oddelenia.");
                }
                query += " id_veduceho_oddelenia = @id_veduceho_oddelenia,";
            }

            if (id_projektu_oddelenia != null)
            {
                if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Oddelenia, (int)id_projektu_oddelenia))
                {
                    return BadRequest("Projekt zaslany cez id ako projekt oddelenia neexistuje.");
                }
                query += " id_projektu_oddelenia = @id_projektu_oddelenia ";
            }
            if (id_projektu_oddelenia == null)
            {
                query = query.Remove(query.Length - 1);
            }
            query += " WHERE id_oddelenia = @id_oddelenia";
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@id_oddelenia", SqlDbType.Int).Value = id;
            if (nazov_oddelenia != null)
            {
                command.Parameters.AddWithValue("@nazov_oddelenia", SqlDbType.VarChar).Value = nazov_oddelenia;
            }
            if (id_veduceho_oddelenia != null)
            {
                command.Parameters.AddWithValue("@id_veduceho_oddelenia", SqlDbType.Int).Value = id_veduceho_oddelenia;
            }
            if (id_projektu_oddelenia != null)
            {
                command.Parameters.AddWithValue("@id_projektu_oddelenia", SqlDbType.Int).Value = id_projektu_oddelenia;
            }
            command.ExecuteNonQuery();
            _connection.Close();
            return Ok("Oddelenie s danym ID bolo upravene.");
        }

        // DELETE api/<OddeleniaControllers>/5
        [HttpDelete("{id}")]
        public ObjectResult Delete(int id)
        {
            if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Oddelenia, id))
            {
                return BadRequest("Neexistuje oddelenie s danym ID!");
            }

            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("DELETE FROM Oddelenia WHERE id_oddelenia = @id_oddelenia", _connection);
            command.Parameters.AddWithValue("@id_oddelenia", SqlDbType.Int).Value = id;
            command.ExecuteNonQuery();
            _connection.Close();
            return Ok("Oddelenie s danym ID bolo zmazane.");
        }
    }
}
