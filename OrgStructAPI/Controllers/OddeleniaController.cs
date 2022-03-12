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
        List<int> IDs = getListOfIDs();


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
                if (reader[2] is DBNull)
                {
                    Oddelenie item = new Oddelenie((int)reader[0], (string)reader[1], null, (int)reader[3]);
                    _oddelenia.Add(item);
                }
                else
                {
                    Oddelenie item = new Oddelenie((int)reader[0], (string)reader[1], (int)reader[2], (int)reader[3]);
                    _oddelenia.Add(item);
                }
            }
            _connection.Close();
            return _oddelenia;
        }

        // GET api/<OddeleniaControllers>/5
        [HttpGet("{id}")]
        public Object? Get(int id)
        {
            if (IDs.Contains(id))
            {
                _connection.ConnectionString = _connectionString;
                _connection.Open();
                var command = new SqlCommand("SELECT * FROM Oddelenia WHERE id_oddelenia = @id_oddelenia", _connection);
                command.Parameters.AddWithValue("@id_oddelenia", SqlDbType.Int).Value = id;
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    if (reader[2] is DBNull)
                    {
                        Oddelenie item = new Oddelenie((int)reader[0], (string)reader[1], null, (int)reader[3]);
                        _connection.Close();
                        return item;
                    }
                    else
                    {
                        Oddelenie item = new Oddelenie((int)reader[0], (string)reader[1], (int)reader[2], (int)reader[3]);
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
                return BadRequest("Neexistuje oddelenie s danym ID!");
            }
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

            if (ControlaExisten.ExistujePodlaID(4, (int)id_projektu_oddelenia))
            {
                if (id_veduceho_oddelenia != null)
                {
                    if (ControlaExisten.ExistujePodlaID(2, (int)id_veduceho_oddelenia))
                    {
                        _connection.ConnectionString = _connectionString;
                        _connection.Open();
                        var command = new SqlCommand("INSERT INTO Oddelenia (nazov_oddelenia,id_veduceho_oddelenia,id_projektu_oddelenia) VALUES(@nazov_oddelenia,@id_veduceho_oddelenia,@id_projektu_oddelenia)", _connection);
                        command.Parameters.AddWithValue("@nazov_oddelenia", SqlDbType.VarChar).Value = nazov_oddelenia;
                        command.Parameters.AddWithValue("@id_veduceho_oddelenia", SqlDbType.Int).Value = id_veduceho_oddelenia;
                        command.Parameters.AddWithValue("@id_projektu_oddelenia", SqlDbType.Int).Value = id_projektu_oddelenia;
                        command.ExecuteNonQuery();
                        _connection.Close();
                        return Ok("Nove oddelenie uspesne zaregistrovane.");

                    }
                    else
                    {
                        return BadRequest("Zamestnanec zaslany s id ako veduci oddelenia neexistuje.");
                    }


                }
                else
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    var command = new SqlCommand("INSERT INTO Oddelenia (nazov_oddelenia,id_veduceho_oddelenia,id_projektu_oddelenia) VALUES(@nazov_oddelenia,null,@id_projektu_oddelenia)", _connection);
                    command.Parameters.AddWithValue("@nazov_oddelenia", SqlDbType.VarChar).Value = nazov_oddelenia;
                    command.Parameters.AddWithValue("@id_projektu_oddelenia", SqlDbType.Int).Value = id_projektu_oddelenia;
                    command.ExecuteNonQuery();
                    _connection.Close();
                    return Ok("Nove oddelenie uspesne zaregistrovane.");

                }
            }
            else
            {
                return BadRequest("Projekt zaslany cez id ako projekt oddelenia neexistuje.");

            }
        }

        // PUT api/<OddeleniaControllers>/5
        [HttpPut("{id}")]
        public ObjectResult Put(int id, [FromHeader] string? nazov_oddelenia, [FromHeader] int? id_veduceho_oddelenia, [FromHeader] int? id_projektu_oddelenia)
        {
            if (IDs.Contains(id))
            {
                if (nazov_oddelenia != null)
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    var command = new SqlCommand("UPDATE Oddelenia SET nazov_oddelenia = @nazov_oddelenia WHERE id_oddelenia = @id_oddelenia", _connection);
                    command.Parameters.AddWithValue("@nazov_oddelenia", SqlDbType.VarChar).Value = nazov_oddelenia;
                    command.Parameters.AddWithValue("@id_oddelenia", SqlDbType.Int).Value = id;
                    command.ExecuteNonQuery();
                    _connection.Close();
                }

                if (id_veduceho_oddelenia != null)
                {
                    if (ControlaExisten.ExistujePodlaID(2, (int)id_veduceho_oddelenia))
                    {
                        _connection.ConnectionString = _connectionString;
                        _connection.Open();
                        var command = new SqlCommand("UPDATE Oddelenia SET id_veduceho_oddelenia = @id_veduceho_oddelenia WHERE id_oddelenia = @id_oddelenia", _connection);
                        command.Parameters.AddWithValue("@id_veduceho_oddelenia", SqlDbType.VarChar).Value = id_veduceho_oddelenia;
                        command.Parameters.AddWithValue("@id_oddelenia", SqlDbType.Int).Value = id;
                        command.ExecuteNonQuery();
                        _connection.Close();
                    }
                    else
                    {
                        return BadRequest("Neexistuje zamestnanec ktory ma byt priradeny ako veduci oddelenia.");
                    }
                }

                if (id_projektu_oddelenia != null)
                {
                    if (ControlaExisten.ExistujePodlaID(4, (int)id_projektu_oddelenia))
                    {
                        _connection.ConnectionString = _connectionString;
                        _connection.Open();
                        var command = new SqlCommand("UPDATE Oddelenia SET id_projektu_oddelenia = @id_projektu_oddelenia WHERE id_oddelenia = @id_oddelenia", _connection);
                        command.Parameters.AddWithValue("@id_projektu_oddelenia", SqlDbType.VarChar).Value = id_projektu_oddelenia;
                        command.Parameters.AddWithValue("@id_oddelenia", SqlDbType.Int).Value = id;
                        command.ExecuteNonQuery();
                        _connection.Close();
                    }
                    else
                    {
                        return BadRequest("Projekt zaslany cez id ako projekt oddelenia neexistuje.");
                    }
                }
                if (id_projektu_oddelenia == null && id_veduceho_oddelenia == null && nazov_oddelenia == null)
                {
                    return BadRequest("Neprisli ziadne udaje na upravu, zaslite udaje podla ktorych chcete upravovat v headeri.");
                }
                return Ok("Oddelenie s danym ID bolo upravene.");
            }
            return BadRequest("Neexistuje oddelenie s danym ID!");

        }

        // DELETE api/<OddeleniaControllers>/5
        [HttpDelete("{id}")]
        public ObjectResult Delete(int id)
        {
            if (IDs.Contains(id))
            {
                _connection.ConnectionString = _connectionString;
                _connection.Open();
                var command = new SqlCommand("DELETE FROM Oddelenia WHERE id_oddelenia = @id_oddelenia", _connection);
                command.Parameters.AddWithValue("@id_oddelenia", SqlDbType.Int).Value = id;
                command.ExecuteNonQuery();
                _connection.Close();
                return Ok("Oddelenia s danym ID bolo zmazane.");
            }
            else
            {
                return BadRequest("Neexistuje oddelenie s danym ID!");
            }
        }


        private static List<int> getListOfIDs()
        {
            List<int> list = new List<int>();
            SqlConnection _connection = new SqlConnection();
            string _connectionString = Startup.GetConnectionString();
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("SELECT id_oddelenia FROM Oddelenia", _connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(reader.GetInt32(0));
            }
            return list;
        }
    }
}
