using Microsoft.AspNetCore.Mvc;
using OrgStructAPI.Models;
using OrgStructAPI;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel;
using System.Text.Json;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace OrgStructAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirmyController : ControllerBase
    {
        private SqlConnection _connection = new SqlConnection();
        string _connectionString = Startup.GetConnectionString();

        // GET: api/<FirmyController>
        [HttpGet]
        public IEnumerable<Firma> Get()
        {
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("SELECT id_firmy,nazov_firmy,id_riaditel_firmy FROM FIRMY", _connection);
            SqlDataReader reader = command.ExecuteReader();
            List<Firma> _firmy = new List<Firma>();
            while (reader.Read())
            {
                var newList = ControlaExisten.replaceDBNullsInReaderRow(reader);
                Firma item = new Firma(
                        (int)newList[0],
                        (string)newList[1],
                        (int?)newList[2]);
                _firmy.Add(item);
            }
            _connection.Close();
            return _firmy;
        }
        //TODO namiesto ID riaditela vypisat cez GET meno spojenim tabulky
        // GET api/<FirmyController>/5
        [HttpGet("{id}")]
        public Object? Get(int id)
        {// navrati ConflictResult alebo BadRequestResult ako potomkov ObjectResult, alebo instanciu triedy Firma
            if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Firmy, id))
            {
                return BadRequest("Neexistuje firma s danym ID!");
            }
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("SELECT id_firmy,nazov_firmy,id_riaditel_firmy FROM FIRMY WHERE id_firmy = @id_firmy", _connection);
            command.Parameters.Add("@id_firmy", SqlDbType.Int);
            command.Parameters["@id_firmy"].Value = id;

            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return Conflict("Naslo sa ID, ale databaza nenavratila hodnoty, asi bude chyba v SQL query.");
            }
            var newList = ControlaExisten.replaceDBNullsInReaderRow(reader);
            Firma item = new Firma(
                    (int)newList[0],
                    (string)newList[1],
                    (int?)newList[2]);
            return item;
        }

        // POST api/<FirmyController>
        [HttpPost]
        public ObjectResult Post([FromHeader] string? nazov_firmy, [FromHeader] int? id_riaditel_firmy)
        {
            if (nazov_firmy == null)
            {
                return BadRequest("Firma nema nazov. Na registraciu firmy potrebujete nazov firmy. Poslite nazov v headeri.");
            }
            string query = "INSERT INTO Firmy (nazov_firmy,id_riaditel_firmy) VALUES(@nazov_firmy";

            if (id_riaditel_firmy != null)
            {
                if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Zamestnanci, (int)id_riaditel_firmy))
                {
                    return BadRequest("Nenasiel sa zamestnanec zaslany ako riaditel novej firmy.");
                }
                query += ",@id_riaditel_firmy)";
            }
            if (id_riaditel_firmy == null)
            {
                query += ",null)";
            }
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@nazov_firmy", SqlDbType.VarChar).Value = nazov_firmy;
            if (id_riaditel_firmy != null)
            {
                command.Parameters.AddWithValue("@id_riaditel_firmy", SqlDbType.Int).Value = id_riaditel_firmy;
            }
            command.ExecuteNonQuery();
            _connection.Close();
            return Ok("Registracia firmy prebehla uspesne.");
        }

        // PUT api/<FirmyController>/5
        [HttpPut("{id}")]
        public ObjectResult Put(int id, [FromHeader] string? nazov_firmy, [FromHeader] int? id_riaditel_firmy)
        {// navrati OKResponse alebo BadRequestResponse
            if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Firmy, id))
            {
                return BadRequest("Neexistuje firma s danym ID!");
            }

            string query = String.Format("UPDATE Firmy SET ");

            if (nazov_firmy != null)
            {
                query += " nazov_firmy = @nazov_firmy,";
            }

            if (id_riaditel_firmy != null)
            {
                if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Zamestnanci, (int)id_riaditel_firmy))
                {
                    return BadRequest("Neexistuje zamestnanec ktoreho chcete priradit ako riaditela firmy.");
                }
                query += " id_riaditel_firmy = @id_riaditel_firmy";
            }
            if (id_riaditel_firmy == null)
            {
                query = query.Remove(query.Length - 1);
            }
            query += " WHERE id_firmy = @id_firmy";

            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@id_firmy", SqlDbType.Int).Value = id;
            if (nazov_firmy != null)
            {
                command.Parameters.AddWithValue("@nazov_firmy", SqlDbType.VarChar).Value = nazov_firmy;
            }
            if (id_riaditel_firmy != null)
            {
                command.Parameters.AddWithValue("@id_riaditel_firmy", SqlDbType.Int).Value = id_riaditel_firmy;
            }
            command.ExecuteNonQuery();
            _connection.Close();
            return Ok("Firma s danym ID bola upravena.");
        }

        // DELETE api/<FirmyController>/5
        [HttpDelete("{id}")]
        public ObjectResult Delete(int id)
        {
            if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Firmy, id))
            {
                return BadRequest("Neexistuje firma s danym ID!");
            }
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("DELETE FROM Firmy WHERE id_firmy = @id_firmy", _connection);
            command.Parameters.AddWithValue("@id_firmy", SqlDbType.Int).Value = id;
            command.ExecuteNonQuery();
            _connection.Close();
            return Ok("Firma s danym ID bola zmazana.");
        }
    }
}
