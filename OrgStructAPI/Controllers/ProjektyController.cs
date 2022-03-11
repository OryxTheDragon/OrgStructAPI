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
        List<int> IDs = getListOfIDs();


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
                if (reader[2] is DBNull)
                {
                    if (reader[3] is DBNull)
                    {
                        Projekt item = new Projekt((int)reader[0], (string)reader[1], null, null, (int)reader[4]);
                        _projekty.Add(item);

                    }
                    else
                    {
                        Projekt item = new Projekt((int)reader[0], (string)reader[1], null, (int)reader[3], (int)reader[4]);
                        _projekty.Add(item);

                    }
                }
                else if (reader[3] is DBNull)
                {
                    Projekt item = new Projekt((int)reader[0], (string)reader[1], (string)reader[2], null, (int)reader[4]);
                    _projekty.Add(item);
                }
                else
                {
                    Projekt item = new Projekt((int)reader[0], (string)reader[1], (string)reader[2], (int)reader[3], (int)reader[4]);
                    _projekty.Add(item);
                }
            }
            _connection.Close();
            return _projekty;
        }

        // GET api/<ProjektyControllers>/5
        [HttpGet("{id}")]
        public Object? Get(int id)
        {
            if (IDs.Contains(id))
            {
                _connection.ConnectionString = _connectionString;
                _connection.Open();
                var command = new SqlCommand("SELECT * FROM Projekty WHERE id_projektu = @id_projektu", _connection);
                command.Parameters.AddWithValue("@id_projektu", SqlDbType.Int).Value = id;
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    if (reader[2] is DBNull)
                    {
                        if (reader[3] is DBNull)
                        {
                            Projekt item = new Projekt((int)reader[0], (string)reader[1], null, null, (int)reader[4]);
                            return item;
                        }
                        else
                        {
                            Projekt item = new Projekt((int)reader[0], (string)reader[1], null, (int)reader[3], (int)reader[4]);
                            return item;
                        }
                    }
                    else if (reader[3] is DBNull)
                    {
                        Projekt item = new Projekt((int)reader[0], (string)reader[1], (string)reader[2], null, (int)reader[4]);
                        return item;
                    }
                    else
                    {
                        Projekt item = new Projekt((int)reader[0], (string)reader[1], (string)reader[2], (int)reader[3], (int)reader[4]);
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
                return BadRequest("Neexistuje projekt s danym ID!");
            }
        }

        // POST api/<ProjektyControllers>
        [HttpPost]
        public ObjectResult Post([FromHeader] string nazov_projektu, [FromHeader] string? popis_projektu, [FromHeader] int? id_veduci_projektu, [FromHeader] int id_divizia_projektu)
        {
            List<int> list = new();
            SqlConnection _connection = new SqlConnection();
            string _connectionString = Startup.GetConnectionString();
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("SELECT id_divizie FROM Divizie", _connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add((int)reader[0]);
            }
            _connection.Close();

            if (list.Contains(id_divizia_projektu))
            {
                if (popis_projektu == null)
                {
                    if (id_veduci_projektu == null)
                    {
                        _connection.ConnectionString = _connectionString;
                        _connection.Open();
                        command = new SqlCommand("INSERT INTO Projekty (nazov_projektu,popis_projektu,id_veduci_projektu,id_divizia_projektu) VALUES(" +
                            "@nazov_projektu," +
                            "null," +
                            "null," +
                            "@id_divizia_projektu" +
                            ")", _connection);
                        command.Parameters.AddWithValue("@nazov_projektu", SqlDbType.VarChar).Value = nazov_projektu;
                        command.Parameters.AddWithValue("@id_divizia_projektu", SqlDbType.Int).Value = id_divizia_projektu;
                        command.ExecuteNonQuery();
                        _connection.Close();
                        return Ok("Projekt bol uspesne zavedeny do databazy bez popisu a bez veduceho zamestnanca.");
                    }
                    else
                    {
                        _connection.ConnectionString = _connectionString;
                        _connection.Open();
                        command = new SqlCommand("INSERT INTO Projekty (nazov_projektu,popis_projektu,id_veduci_projektu,id_divizia_projektu) VALUES(" +
                            "@nazov_projektu," +
                            "null," +
                            "@id_veduci_projektu," +
                            "@id_divizia_projektu" +
                            ")", _connection);
                        command.Parameters.AddWithValue("@nazov_projektu", SqlDbType.VarChar).Value = nazov_projektu;
                        command.Parameters.AddWithValue("@id_veduci_projektu", SqlDbType.Int).Value = id_veduci_projektu;
                        command.Parameters.AddWithValue("@id_divizia_projektu", SqlDbType.Int).Value = id_divizia_projektu;
                        command.ExecuteNonQuery();
                        _connection.Close();
                        return Ok("Projekt bol uspesne zavedeny do databazy bez popisu.");
                    }
                }
                else if (id_veduci_projektu == null)
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    command = new SqlCommand("INSERT INTO Projekty (nazov_projektu,popis_projektu,id_veduci_projektu,id_divizia_projektu) VALUES(" +
                        "@nazov_projektu," +
                        "@popis_projektu," +
                        "null," +
                        "@id_divizia_projektu" +
                        ")", _connection);
                    command.Parameters.AddWithValue("@nazov_projektu", SqlDbType.VarChar).Value = nazov_projektu;
                    command.Parameters.AddWithValue("@popis_projektu", SqlDbType.VarChar).Value = popis_projektu;
                    command.Parameters.AddWithValue("@id_divizia_projektu", SqlDbType.Int).Value = id_divizia_projektu;
                    command.ExecuteNonQuery();
                    _connection.Close();
                    return Ok("Projekt bol uspesne zavedeny do databazy bez hodnoty veduceho zamestnanca.");
                }
                else {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    command = new SqlCommand("INSERT INTO Projekty (nazov_projektu,popis_projektu,id_veduci_projektu,id_divizia_projektu) VALUES(" +
                        "@nazov_projektu," +
                        "@popis_projektu," +
                        "@id_veduci_projektu," +
                        "@id_divizia_projektu" +
                        ")", _connection);
                    command.Parameters.AddWithValue("@nazov_projektu", SqlDbType.VarChar).Value = nazov_projektu;
                    command.Parameters.AddWithValue("@popis_projektu", SqlDbType.VarChar).Value = popis_projektu;
                    command.Parameters.AddWithValue("@id_veduci_projektu", SqlDbType.Int).Value = id_veduci_projektu;
                    command.Parameters.AddWithValue("@id_divizia_projektu", SqlDbType.Int).Value = id_divizia_projektu;
                    command.ExecuteNonQuery();
                    _connection.Close();
                    return Ok("Projekt bol plne uspesne zavedeny do databazy.");
                }
            }
            else
            {
                return BadRequest("Projekt musi byt priradeny do divizie. V databaze neexistuje divizia so zadanym ID.");
            }
        }

        // PUT api/<ProjektyControllers>/5
        [HttpPut("{id}")]
        public ObjectResult Put(int id, [FromHeader] string? nazov_projektu, [FromHeader] string? popis_projektu, [FromHeader] int? id_veduci_projektu, [FromHeader] int? id_divizia_projektu)
        {
            if (IDs.Contains(id))
            {
                if (nazov_projektu != null)
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    var command = new SqlCommand("UPDATE Projekty SET nazov_projektu = @nazov_projektu WHERE id_projektu = @id_projektu", _connection);
                    command.Parameters.AddWithValue("@nazov_projektu", SqlDbType.VarChar).Value = nazov_projektu;
                    command.Parameters.AddWithValue("@id_projektu", SqlDbType.VarChar).Value = id;
                    command.ExecuteNonQuery();
                    _connection.Close();
                }
                if (popis_projektu != null)
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    var command = new SqlCommand("UPDATE Projekty SET popis_projektu = @popis_projektu WHERE id_projektu = @id_projektu", _connection);
                    command.Parameters.AddWithValue("@popis_projektu", SqlDbType.VarChar).Value = popis_projektu;
                    command.Parameters.AddWithValue("@id_projektu", SqlDbType.VarChar).Value = id;
                    command.ExecuteNonQuery();
                    _connection.Close();
                }
                if (id_veduci_projektu != null)
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    var command = new SqlCommand("UPDATE Projekty SET id_veduci_projektu = @id_veduci_projektu WHERE id_projektu = @id_projektu", _connection);
                    command.Parameters.AddWithValue("@id_veduci_projektu", SqlDbType.VarChar).Value = id_veduci_projektu;
                    command.Parameters.AddWithValue("@id_projektu", SqlDbType.VarChar).Value = id;
                    command.ExecuteNonQuery();
                    _connection.Close();
                }
                if (nazov_projektu != null)
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    var command = new SqlCommand("UPDATE Projekty SET id_divizia_projektu = @id_divizia_projektu WHERE id_projektu = @id_projektu", _connection);
                    command.Parameters.AddWithValue("@id_divizia_projektu", SqlDbType.VarChar).Value = id_divizia_projektu;
                    command.Parameters.AddWithValue("@id_projektu", SqlDbType.VarChar).Value = id;
                    command.ExecuteNonQuery();
                    _connection.Close();
                }
                if (nazov_projektu == null && popis_projektu == null && id_veduci_projektu == null && nazov_projektu == null )
                {
                    return BadRequest("Neprisli ziadne zadane hodnoty na upravu. Zaslite hodnoty ktore chcete upravit cez header.");
                }
                return Ok("Projekt s danym ID bol upraveny.");
            }
            else { 
                return BadRequest("Neexistuje projekt s danym ID!");
            }

        }

        // DELETE api/<ProjektyControllers>/5
        [HttpDelete("{id}")]
        public ObjectResult Delete(int id)
        {
            if (IDs.Contains(id))
            {
                _connection.ConnectionString = _connectionString;
                _connection.Open();
                var command = new SqlCommand("DELETE FROM Projekty WHERE id_projektu = @id_projektu", _connection);
                command.Parameters.AddWithValue("@id_projektu", SqlDbType.Int).Value = id;
                command.ExecuteNonQuery();
                _connection.Close();
                return Ok("Projekt so zadanym ID bol zmazany.");
            }
            else
            {
                return BadRequest("Neexistuje projekt s danym ID!");
            }
        }


        private static List<int> getListOfIDs()
        {
            List<int> list = new List<int>();
            SqlConnection _connection = new SqlConnection();
            string _connectionString = Startup.GetConnectionString();
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("SELECT id_projektu FROM Projekty", _connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(reader.GetInt32(0));
            }
            return list;
        }
    }
}
