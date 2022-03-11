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
        List<int> IDs = getListOfIDs();


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
                if (reader[2] is DBNull)
                {
                    Divizia item = new Divizia((int)reader[0], (string)reader[1], null, (int)reader[3]);
                    _divizie.Add(item);
                }
                else
                {
                    Divizia item = new Divizia((int)reader[0], (string)reader[1], (int)reader[2], (int)reader[3]);
                    _divizie.Add(item);
                }
            }
            _connection.Close();
            return _divizie;
        }

        // GET api/<DivizieController>/5
        [HttpGet("{id}")]
        public Object? Get(int id)
        { //Navrati ObjectResult ConflictResult/BadRequestResult alebo instanciu Divizie
            if (IDs.Contains(id))
            {
                _connection.ConnectionString = _connectionString;
                _connection.Open();
                var command = new SqlCommand("SELECT * FROM Divizie WHERE id_divizie = @id_divizie", _connection);
                command.Parameters.Add("@id_divizie", SqlDbType.Int);
                command.Parameters["@id_divizie"].Value = id;

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    if (reader[2] is DBNull)
                    {
                        Divizia item = new Divizia((int)reader[0], (string)reader[1], null, (int)reader[3]);
                        _connection.Close();
                        return item;
                    }
                    else
                    {
                        Divizia item = new Divizia((int)reader[0], (string)reader[1], (int)reader[2], (int)reader[3]);
                        _connection.Close();
                        return item;
                    }
                }
                else
                {
                    return Conflict("Naslo sa ID, ale databaza nenavratila hodnoty, asi bude chyba v SQL query.");
                }
            }
            else
            {
                return BadRequest("Neexistuje divizia s danym ID!");
            }
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
            if (id_veduceho_divizie != null)
            {
                _connection.ConnectionString = _connectionString;
                _connection.Open();
                var command = new SqlCommand("INSERT INTO Divizie (nazov_divizie,id_veduceho_divizie,id_firmy_divizie) VALUES(" +
                    "@nazov_divizie," +
                    "@id_veduceho_divizie," +
                    "@id_firmy_divizie" +
                    ")", _connection);
                command.Parameters.AddWithValue("@nazov_divizie", SqlDbType.VarChar).Value = nazov_divizie;
                command.Parameters.AddWithValue("@id_veduceho_divizie", SqlDbType.Int).Value = id_veduceho_divizie;
                command.Parameters.AddWithValue("@id_firmy_divizie", SqlDbType.Int).Value = id_firmy_divizie;
                command.ExecuteNonQuery();
                _connection.Close();
                return Ok("Nova divizia uspesne registrovana.");
            }
            else {
                _connection.ConnectionString = _connectionString;
                _connection.Open();
                var command = new SqlCommand("INSERT INTO Divizie (nazov_divizie,id_veduceho_divizie,id_firmy_divizie) VALUES(" +
                    "@nazov_divizie," +
                    "null," +
                    "@id_firmy_divizie" +
                    ")", _connection);
                command.Parameters.AddWithValue("@nazov_divizie", SqlDbType.VarChar).Value = nazov_divizie;
                command.Parameters.AddWithValue("@id_firmy_divizie", SqlDbType.Int).Value = id_firmy_divizie;
                command.ExecuteNonQuery();
                _connection.Close();
                return Ok("Nova divizia uspesne registrovana bez vedenia.");
            }
        }

        // PUT api/<DivizieController>/5
        [HttpPut("{id}")]
        public ObjectResult Put(int id, [FromHeader] string? nazov_divizie, [FromHeader] int? id_veduceho_divizie, [FromHeader] int? id_firmy_divizie)
        {
            if (IDs.Contains(id))
            {
                if (nazov_divizie != null)
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    var command = new SqlCommand("UPDATE Divizie SET nazov_divizie = @nazov_divizie WHERE id_divizie = @id_divizie", _connection);
                    command.Parameters.AddWithValue("@nazov_divizie", SqlDbType.VarChar).Value = nazov_divizie;
                    command.Parameters.AddWithValue("@id_divizie", SqlDbType.Int).Value = id;
                    command.ExecuteNonQuery();
                    _connection.Close();
                }
                if (id_veduceho_divizie != null)
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    var command = new SqlCommand("UPDATE Divizie SET id_veduceho_divizie = @id_veduceho_divizie WHERE id_divizie = @id_divizie", _connection);
                    command.Parameters.AddWithValue("@id_veduceho_divizie", SqlDbType.Int).Value = id_veduceho_divizie;
                    command.Parameters.AddWithValue("@id_divizie", SqlDbType.Int).Value = id;
                    command.ExecuteNonQuery();
                    _connection.Close();
                }
                if (id_firmy_divizie != null)
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    var command = new SqlCommand("UPDATE Divizie SET id_firmy_divizie = @id_firmy_divizie WHERE id_divizie = @id_divizie", _connection);
                    command.Parameters.AddWithValue("@id_firmy_divizie", SqlDbType.Int).Value = id_firmy_divizie;
                    command.Parameters.AddWithValue("@id_divizie", SqlDbType.Int).Value = id;
                    command.ExecuteNonQuery();
                    _connection.Close();
                }
                if (nazov_divizie == null && id_veduceho_divizie == null && id_firmy_divizie == null)
                {
                    return BadRequest("Neprisli ziadne udaje na upravu, zaslite udaje podla ktorych chcete upravovat v headeri.");
                }
                return Ok("Uprava prebehla uspesne.");
            }
            return BadRequest("Neexistuje divizia s danym ID!");
        }

        // DELETE api/<DivizieController>/5
        [HttpDelete("{id}")]
        public ObjectResult Delete(int id)
        {
            if (IDs.Contains(id))
            {
                _connection.ConnectionString = _connectionString;
                _connection.Open();
                var command = new SqlCommand("DELETE FROM Divizie WHERE id_divizie = @id_divizie", _connection);
                command.Parameters.AddWithValue("@id_divizie", SqlDbType.Int).Value = id;
                command.ExecuteNonQuery();
                _connection.Close();
                return Ok("Divizia s danym ID bola zmazana.");
            }
            else
            {
                return BadRequest("Neexistuje divizia s danym ID!");
            }
        }

        private static List<int> getListOfIDs()
        {
            List<int> list = new List<int>();
            SqlConnection _connection = new SqlConnection();
            string _connectionString = Startup.GetConnectionString();
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("SELECT id_divizie FROM Divizie", _connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(reader.GetInt32(0));
            }
            return list;
        }
    }
}
