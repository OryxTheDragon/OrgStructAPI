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
                //TODO argument ordering je koli klukatemu vyvoju kodu, je to tu nechane ako autenticka legacy code experience
                var newList = ControlaExisten.replaceDBNullsInReaderRow(reader);
                Zamestnanec item = new Zamestnanec(
                    (int)newList[0],
                    (string)newList[1],
                    (string)newList[2],
                    (int)newList[5],
                    (string?)newList[3],
                    (string?)newList[4],
                    (int?)newList[6]);
                _zamestnanci.Add(item);
            }
            _connection.Close();
            return _zamestnanci;
        }

        // GET api/<ZamestnanciController>/5
        [HttpGet("{id}")]
        public object? Get(int id)
        {//Vrati bud zamestnanca, alebo BadResponseResult ako potomok ObjectResult.
            if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Zamestnanci, id))
            {
                return BadRequest("Neexistuje zamestnanec so zadanym ID!");
            }
            Zamestnanec? item = null;
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("SELECT * FROM Zamestnanci WHERE id_zamestnanca = @id_zamestnanca", _connection);
            command.Parameters.Add("@id_zamestnanca", SqlDbType.Int);
            command.Parameters["@id_zamestnanca"].Value = id;
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var newList = ControlaExisten.replaceDBNullsInReaderRow(reader);
                item = new Zamestnanec(
                    (int)newList[0],
                    (string)newList[1],
                    (string)newList[2],
                    (int)newList[5],
                    (string?)newList[3],
                    (string?)newList[4],
                    (int?)newList[6]);
            }
            if (item == null)
            {
                return BadRequest("Doslo k chybe, zamestnanec s danym ID sa nasiel," +
                    " ale data sa neprecitali. Pravdepodobne bude chyba v SQL query.");
            }
            _connection.Close();
            return item;
        }

        // POST api/<ZamestnanciController>
        [HttpPost]
        public ObjectResult Post([FromHeader] string? meno, [FromHeader] string? priezvisko, [FromHeader] int? id_firmy_zamestnanca, [FromHeader] string? phone_num, [FromHeader] string? title, [FromHeader] int? id_oddelenia_zamestnanca)
        {//navrati BadResult alebo OKResult


            if (meno == null)
            {
                return BadRequest("Novy zakaznik musi mat meno. Poslite nazov zakaznika v headeri.");
            }
            if (priezvisko == null)
            {
                return BadRequest("Novy zakaznik musi mat priezvisko. Poslite priezvisko zakaznika v headeri.");
            }
            if (id_firmy_zamestnanca == null)
            {
                return BadRequest("Pri registracii zamestnanca musi byt priradeny firme. Nezadali ste id firmy. Poslite id v headeri.");
            }
            if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Firmy, (int)id_firmy_zamestnanca))
            {
                return BadRequest("Novy zamestnanec musi byt priradeny do firmy. V databaze neexistuje firma so sadanym ID.");
            }

            string query = String.Format("INSERT INTO Zamestnanci(meno, priezvisko, id_firmy_zamestnanca, phone_num, title, id_oddelenia_zamestnanca) VALUES(" +
                            "@meno," +
                            "@priezvisko," +
                            "@id_firmy_zamestnanca");

            if (phone_num != null)
            {
                foreach (char c in phone_num)
                {
                    if (c < '0' || c > '9')
                    {
                        return BadRequest("Zadane telefonne cislo moze obsahovat len cisla, nie pismena.");
                    }
                }
                if (phone_num.Length != 10)
                {
                    return BadRequest("Zadane telefonne cislo musi mat 10 cifier.");

                }
                query += ",@phone_num";
            }
            else
            {
                query += ",null";
            }
            if (title != null)
            {
                if (title.Length > 5)
                {
                    return BadRequest("Titul musi byt oznaceny dohromady maximalne 5 charaktermi.");
                }
                query += ",@title";
            }
            else
            {
                query += ",null";
            }
            if (id_oddelenia_zamestnanca != null)
            {
                if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Oddelenia, (int)id_oddelenia_zamestnanca))
                {
                    return BadRequest("Oddelenie zadane cez id ako oddelenie v ktorom bude pracovat zamestnanec neexistuje.");
                }

                query += ",@id_oddelenia_zamestnanca)";
            }
            else
            {
                query += ",null)";
            }
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@meno", SqlDbType.VarChar).Value = meno;
            command.Parameters.AddWithValue("@priezvisko", SqlDbType.VarChar).Value = priezvisko;
            command.Parameters.AddWithValue("@id_firmy_zamestnanca", SqlDbType.Int).Value = id_firmy_zamestnanca;
            if (phone_num != null)
            {
                command.Parameters.AddWithValue("@phone_num", SqlDbType.VarChar).Value = phone_num;
            }
            if (title != null)
            {
                command.Parameters.AddWithValue("@title", SqlDbType.VarChar).Value = title;
            }
            if (id_oddelenia_zamestnanca != null)
            {
                command.Parameters.AddWithValue("@id_oddelenia_zamestnanca", SqlDbType.Int).Value = id_oddelenia_zamestnanca;
            }
            command.ExecuteNonQuery();
            _connection.Close();
            return Ok("Zamestnanec bol uspesne zavedeny do databazy.");
        }

        // PUT api/<ZamestnanciController>/5
        [HttpPut("{id}")]
        public ObjectResult Put(int id, [FromHeader] string? meno, [FromHeader] string? priezvisko, [FromHeader] string? phone_num, [FromHeader] string? title, [FromHeader] int? id_firmy_zamestnanca, [FromHeader] int? id_oddelenia_zamestnanca)
        {//navrati BadResult alebo OKResult
            if (meno == null && priezvisko == null && id_firmy_zamestnanca == null && phone_num == null && title == null && id_oddelenia_zamestnanca == null)
            {
                return BadRequest("Neboli zaslane ziadne udaje na upravu. Poslite udaje na zmenu cez header.");
            }
            if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Zamestnanci, id))
            {
                return BadRequest("Neexistuje zamestnanec so zadanym ID!");
            }
            //Zaciatok tvorby SQL query
            string query = String.Format("UPDATE Zamestnanci SET ");

            if (meno != null)
            {
                query += "meno = @meno,";
            }
            if (priezvisko != null)
            {
                query += "priezvisko = @priezvisko,";
            }
            if (id_firmy_zamestnanca != null)
            {
                if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Firmy, (int)id_firmy_zamestnanca))
                {
                    return BadRequest("Firma do zaslana cez ide do ktorej ma byt zamestnanec registrovany neexistuje.");
                }
                query += "id_firmy_zamestnanca = @id_firmy_zamestnanca ,";
            }
            if (phone_num != null)
            {
                foreach (char c in phone_num)
                {
                    if (c < '0' || c > '9')
                    {
                        return BadRequest("Zadane telefonne cislo moze obsahovat len cisla, nie pismena.");
                    }
                }
                if (phone_num.Length != 10)
                {
                    return BadRequest("Zadane telefonne cislo musi mat 10 cifier.");
                }
                query += "phone_num = @phone_num,";
            }
            if (title != null)
            {
                if (title.Length > 5)
                {
                    return BadRequest("Titul musi byt oznaceny dohromady maximalne 5 charaktermi.");
                }
                query += "title = @title,";
            }
            if (id_oddelenia_zamestnanca != null)
            {
                if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Oddelenia, (int)id_oddelenia_zamestnanca))
                {
                    return BadRequest("Oddelenie zadane cez id ako oddelenie v ktorom bude pracovat zamestnanec neexistuje.");
                }
                query += "id_oddelenia_zamestnanca = @id_oddelenia_zamestnanca ";
            }
            else {
                query = query.Remove(query.Length - 1);
            }
            query += " WHERE id_zamestnanca = @id_zamestnanca";
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand(query, _connection);
            if (meno != null)
            {
                command.Parameters.AddWithValue("@meno", SqlDbType.VarChar).Value = meno;
            }
            if (priezvisko != null)
            {
                command.Parameters.AddWithValue("@priezvisko", SqlDbType.VarChar).Value = priezvisko;
            }
            if (id_firmy_zamestnanca != null)
            {
                command.Parameters.AddWithValue("@id_firmy_zamestnanca", SqlDbType.Int).Value = id_firmy_zamestnanca;
            }
            if (phone_num != null)
            {
                command.Parameters.AddWithValue("@phone_num", SqlDbType.VarChar).Value = phone_num;
            }
            if (title != null)
            {
                command.Parameters.AddWithValue("@title", SqlDbType.VarChar).Value = title;
            }
            if (id_oddelenia_zamestnanca != null)
            {
                command.Parameters.AddWithValue("@id_oddelenia_zamestnanca", SqlDbType.Int).Value = id_oddelenia_zamestnanca;
            }
            command.Parameters.AddWithValue("@id_zamestnanca", SqlDbType.Int).Value = id;
            Console.WriteLine(query);
            command.ExecuteNonQuery();
            _connection.Close();
            return Ok("Zamestnanec bol uspesne upraveny.");
        }

        // DELETE api/<ZamestnanciController>/5
        [HttpDelete("{id}")]
        public ObjectResult Delete(int id)
        {
            if (!ControlaExisten.ExistujePodlaID(ControlaExisten.TableID.Zamestnanci, id))
            {
                return BadRequest("Neexistuje zamestnanec so zadanym ID!");
            }
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            var command = new SqlCommand("DELETE FROM Zamestnanci WHERE id_zamestnanca = @id_zamestnanca", _connection);
            command.Parameters.AddWithValue("@id_zamestnanca", SqlDbType.Int).Value = id;
            command.ExecuteNonQuery();
            _connection.Close();

            return Ok("Zamestnanec uspesne zmazany.");
        }

    }
}
