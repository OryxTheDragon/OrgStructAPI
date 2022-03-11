using Microsoft.AspNetCore.Mvc;
using OrgStructAPI.Models;
using System.Data;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OrgStructAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZamestnanciController : ControllerBase
    {
        private SqlConnection _connection = new SqlConnection();
        string _connectionString = Startup.GetConnectionString();
        List<int> IDs = getListOfIDs();


        // GET: api/<ZamestnanciController>
        [HttpGet]
        public IEnumerable<Zamestnanec> Get()
        {
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("SELECT * FROM Zamestnanci", _connection);
            SqlDataReader reader = command.ExecuteReader();
            List<Zamestnanec> _zamestnanci = new List<Zamestnanec>();
            while (reader.Read())
            {
                //Sekcia 3
                if (reader[3] is DBNull)
                {
                    if (reader[4] is DBNull)
                    {

                        Zamestnanec item = new Zamestnanec((int)reader[0], (string)reader[1], (string)reader[2], (int)reader[5], null, null);
                        _zamestnanci.Add(item);
                    }
                    else
                    {

                        Zamestnanec item = new Zamestnanec((int)reader[0], (string)reader[1], (string)reader[2], (int)reader[5], null, (string)reader[4]);
                        _zamestnanci.Add(item);
                    }
                }
                //Sekcia 4
                else if (reader[4] is DBNull)
                {
                    Zamestnanec item = new Zamestnanec((int)reader[0], (string)reader[1], (string)reader[2], (int)reader[5], (string)reader[3], null);
                    _zamestnanci.Add(item);
                }
                else
                {
                    Zamestnanec item = new Zamestnanec((int)reader[0], (string)reader[1], (string)reader[2], (int)reader[5], (string)reader[3], (string)reader[4]);
                    _zamestnanci.Add(item);
                }

            }
            _connection.Close();
            return _zamestnanci;
        }

        // GET api/<ZamestnanciController>/5
        [HttpGet("{id}")]
        public object? Get(int id)
        {//Vrati bud zamestnanca, alebo BadResponseResult ako potomok ObjectResult.
            if (IDs.Contains(id))
            {
                Zamestnanec? item = null;
                _connection.ConnectionString = _connectionString;
                _connection.Open();
                var command = new SqlCommand("SELECT * FROM Zamestnanci WHERE id_zamestnanca = @id_zamestnanca", _connection);
                command.Parameters.Add("@id_zamestnanca", SqlDbType.Int);
                command.Parameters["@id_zamestnanca"].Value = id;
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    //Sekcia 3
                    if (reader[3] is DBNull)
                    {
                        if (reader[4] is DBNull)
                        {
                            item = new Zamestnanec((int)reader[0], (string)reader[1], (string)reader[2], (int)reader[5], null, null);
                        }
                        else
                        {
                            item = new Zamestnanec((int)reader[0], (string)reader[1], (string)reader[2], (int)reader[5], null, (string)reader[4]);
                        }
                    }
                    //Sekcia 4
                    else if (reader[4] is DBNull)
                    {
                        item = new Zamestnanec((int)reader[0], (string)reader[1], (string)reader[2], (int)reader[5], (string)reader[3], null);
                    }
                    else
                    {
                        item = new Zamestnanec((int)reader[0], (string)reader[1], (string)reader[2], (int)reader[5], (string)reader[3], (string)reader[4]);
                    }
                }
                if (item == null)
                {
                    return BadRequest("Doslo k chybe, zamestnanec s danym ID sa nasiel, ale data sa neprecitali. Pravdepodobne bude chyba v SQL query.");
                }
                _connection.Close();
                return item;
            }
            return BadRequest("Neexistuje zamestnanec so zadanym ID!");
        }

        // POST api/<ZamestnanciController>
        [HttpPost]
        public ObjectResult Post([FromHeader] string? meno, [FromHeader] string? priezvisko, [FromHeader] int? id_firmy_zamestnanca)
        {//navrati BadResult alebo OKResult
            List<int> list = new();
            SqlConnection _connection = new SqlConnection();
            string _connectionString = Startup.GetConnectionString();
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("SELECT id_firmy FROM Firmy", _connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add((int)reader[0]);
            }
            _connection.Close();
            if (meno == null)
            {
                return BadRequest("Novy zakaznik musi mat meno. Poslite nazov zakaznika v headeri.");
            }
            if (priezvisko == null)
            {
                return BadRequest("Novy zakaznik musi mat priezvisko. Poslite priezvisko zakaznika v headeri.");
            }
            if (id_firmy_zamestnanca != null && id_firmy_zamestnanca is int)
            {
                if (list.Contains((int)id_firmy_zamestnanca))
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    command = new SqlCommand("INSERT INTO Zamestnanci (meno,priezvisko,id_firmy_zamestnanca,phone_num,title) VALUES(" +
                        "@meno," +
                        "@priezvisko," +
                        "@id_firmy_zamestnanca," +
                        "null," +
                        "null" +
                        ")", _connection);
                    command.Parameters.AddWithValue("@meno", SqlDbType.VarChar).Value = meno;
                    command.Parameters.AddWithValue("@priezvisko", SqlDbType.VarChar).Value = priezvisko;
                    command.Parameters.AddWithValue("@id_firmy_zamestnanca", SqlDbType.Int).Value = id_firmy_zamestnanca;
                    command.ExecuteNonQuery();
                    _connection.Close();
                    return Ok("Zamestnanec bol uspesne zavedeny do databazy.");
                }
                else
                {
                    return BadRequest("Novy zamestnanec musi byt priradeny do firmy. V databaze neexistuje firma so sadanym ID.");
                }
            }
            return BadRequest("Pri registracii zamestnanca musi byt priradeny firme. Nezadali ste id firmy. Poslite id v headeri.");
        }

        // PUT api/<ZamestnanciController>/5
        [HttpPut("{id}")]
        public ObjectResult Put(int id, [FromHeader] string? meno, [FromHeader] string? priezvisko, [FromHeader] string? phone_num, [FromHeader] string? title, [FromHeader] int? id_firmy_zamestnanca)
        {//navrati BadResult alebo OKResult

            if (IDs.Contains(id))
            {
                if (meno != null)
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    var command = new SqlCommand("UPDATE Zamestnanci SET meno = @meno WHERE id_zamestnanca = @id_zamestnanca", _connection);
                    command.Parameters.AddWithValue("@meno", SqlDbType.VarChar).Value = meno;
                    command.Parameters.AddWithValue("@id_zamestnanca", SqlDbType.Int).Value = id;
                    command.ExecuteNonQuery();
                    _connection.Close();
                }
                if (priezvisko != null)
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    var command = new SqlCommand("UPDATE Zamestnanci SET priezvisko = @priezvisko WHERE id_zamestnanca = @id_zamestnanca", _connection);
                    command.Parameters.AddWithValue("@priezvisko", SqlDbType.VarChar).Value = priezvisko;
                    command.Parameters.AddWithValue("@id_zamestnanca", SqlDbType.Int).Value = id;
                    command.ExecuteNonQuery();
                    _connection.Close();
                }
                if (id_firmy_zamestnanca != null)
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    var command = new SqlCommand("UPDATE Zamestnanci SET id_firmy_zamestnanca = @id_firmy_zamestnanca WHERE id_zamestnanca = @id_zamestnanca", _connection);
                    command.Parameters.AddWithValue("@id_firmy_zamestnanca", SqlDbType.VarChar).Value = id_firmy_zamestnanca;
                    command.Parameters.AddWithValue("@id_zamestnanca", SqlDbType.Int).Value = id;
                    command.ExecuteNonQuery();
                    _connection.Close();
                }
                if (phone_num != null)
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    var command = new SqlCommand("UPDATE Zamestnanci SET phone_num = @phone_num WHERE id_zamestnanca = @id_zamestnanca", _connection);
                    command.Parameters.AddWithValue("@phone_num", SqlDbType.VarChar).Value = phone_num;
                    command.Parameters.AddWithValue("@id_zamestnanca", SqlDbType.Int).Value = id;
                    command.ExecuteNonQuery();
                    _connection.Close();
                }
                if (title != null)
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    var command = new SqlCommand("UPDATE Zamestnanci SET title = @title WHERE id_zamestnanca = @id_zamestnanca", _connection);
                    command.Parameters.AddWithValue("@title", SqlDbType.VarChar).Value = title;
                    command.Parameters.AddWithValue("@id_zamestnanca", SqlDbType.Int).Value = id;
                    command.ExecuteNonQuery();
                    _connection.Close();
                }
                if (meno == null && priezvisko == null && id_firmy_zamestnanca == null && phone_num == null && title == null)
                {
                    return BadRequest("Neboli zaslane ziadne udaje na upravu. Poslite udaje na zmenu cez header.");
                }
                return Ok("Zamestnanec bol uspesne upraveny.");
            }
            else
            {
                return BadRequest("Neexistuje zamestnanec so zadanym ID!");
            }
        }

        // DELETE api/<ZamestnanciController>/5
        [HttpDelete("{id}")]
        public ObjectResult Delete(int id)
        {
            if (IDs.Contains(id))
            {
                _connection.ConnectionString = _connectionString;
                _connection.Open();
                var command = new SqlCommand("DELETE FROM Zamestnanci WHERE id_zamestnanca = @id_zamestnanca", _connection);
                command.Parameters.AddWithValue("@id_zamestnanca", SqlDbType.Int).Value = id;
                command.ExecuteNonQuery();
                _connection.Close();

                return Ok("Zamestnanec uspesne zmazany.");
            }
            else
            {
                return BadRequest("Neexistuje zamestnanec so zadanym ID!");
            }
        }


        private static List<int> getListOfIDs()
        {
            List<int> list = new();
            SqlConnection _connection = new SqlConnection();
            string _connectionString = Startup.GetConnectionString();
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("SELECT id_zamestnanca FROM Zamestnanci", _connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add((int)reader[0]);
            }
            return list;
        }
    }
}
