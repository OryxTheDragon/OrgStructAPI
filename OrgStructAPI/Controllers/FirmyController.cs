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
        List<int> IDs = getListOfIDs();

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
                if (reader[2] is DBNull)
                {
                    Firma item = new Firma((int)reader[0], (string)reader[1], null);
                    _firmy.Add(item);
                }
                else
                {
                    Firma item = new Firma((int)reader[0], (string)reader[1], (int)reader[2]);
                    _firmy.Add(item);
                }
            }
            _connection.Close();
            return _firmy;
        }

        // GET api/<FirmyController>/5
        [HttpGet("{id}")]
        public Object? Get(int id)
        {// navrati ConflictResult alebo BadRequestResult ako potomkov ObjectResult, alebo instanciu triedy Firma
            if (IDs.Contains(id))
            {
                _connection.ConnectionString = _connectionString;
                _connection.Open();
                var command = new SqlCommand("SELECT id_firmy,nazov_firmy,id_riaditel_firmy FROM FIRMY WHERE id_firmy = @id_firmy", _connection);
                command.Parameters.Add("@id_firmy", SqlDbType.Int);
                command.Parameters["@id_firmy"].Value = id;

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Firma item = new Firma((int)reader[0], (string)reader[1], (int?)reader[2]);
                    _connection.Close();
                    return item;
                }
                else {
                    return Conflict("Naslo sa ID, ale databaza nenavratila hodnoty, asi bude chyba v SQL query.");
                }
            }
            else
            {
                return BadRequest("Neexistuje firma s danym ID!");
            }
        }

        // POST api/<FirmyController>
        [HttpPost]
        public void Post([FromHeader] string nazov_firmy)
        {
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("INSERT INTO Firmy (nazov_firmy,id_riaditel_firmy) VALUES(@nazov_firmy,null)", _connection);
            command.Parameters.AddWithValue("@nazov_firmy", SqlDbType.VarChar).Value = nazov_firmy;
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            _connection.Close();
        }

        // PUT api/<FirmyController>/5
        [HttpPut("{id}")]
        public ObjectResult Put(int id, [FromHeader] string? nazov_firmy, [FromHeader] int? id_riaditel_firmy)
        {// navrati OKResponse alebo BadRequestResponse
            if (IDs.Contains(id))
            {
                if (id_riaditel_firmy != null)
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    var command = new SqlCommand("UPDATE Firmy SET id_riaditel_firmy = @id_riaditel_firmy WHERE id_firmy = @id_firmy", _connection);
                    command.Parameters.AddWithValue("@id_riaditel_firmy", SqlDbType.VarChar).Value = id_riaditel_firmy;
                    command.Parameters.AddWithValue("@id_firmy", SqlDbType.VarChar).Value = id;
                    command.ExecuteNonQuery();
                    _connection.Close();
                }

                if (nazov_firmy != null)
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    var command = new SqlCommand("UPDATE Firmy SET nazov_firmy = @nazov_firmy WHERE id_firmy = @id_firmy", _connection);
                    command.Parameters.AddWithValue("@nazov_firmy", SqlDbType.VarChar).Value = nazov_firmy;
                    command.Parameters.AddWithValue("@id_firmy", SqlDbType.VarChar).Value = id;
                    command.ExecuteNonQuery();
                    _connection.Close();
                }
                return Ok("Firma s danym ID bola upravena.");
            }
            return BadRequest("Neexistuje firma s danym ID!");

        }

        // DELETE api/<FirmyController>/5
        [HttpDelete("{id}")]
        public ObjectResult Delete(int id)
        {
            if (IDs.Contains(id))
            {
                _connection.ConnectionString = _connectionString;
                _connection.Open();
                var command = new SqlCommand("DELETE FROM Firmy WHERE id_firmy = @id_firmy", _connection);
                command.Parameters.AddWithValue("@id_firmy", SqlDbType.Int).Value = id;
                command.ExecuteNonQuery();
                _connection.Close();
                return Ok("Firma s danym ID bola zmazana.");
            }
            else {
                return BadRequest("Neexistuje firma s danym ID!");
            }
        }

        private static List<int> getListOfIDs()
        {
            List<int> list = new List<int>();
            SqlConnection _connection = new SqlConnection();
            string _connectionString = Startup.GetConnectionString();
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("SELECT id_firmy FROM Firmy", _connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(reader.GetInt32(0));
            }
            return list;
        }
    }

}
