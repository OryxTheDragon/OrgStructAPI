using System.Data.SqlClient;

namespace OrgStructAPI
{
    public static class ControlaExisten
    {
        public enum TableID
        {
            Firmy,
            Zamestnanci,
            Divizie,
            Projekty,
            Oddelenia
        }

        public static bool ExistujePodlaID(TableID? tableId, int id)
        {
            SqlConnection _connection = new SqlConnection();
            string _connectionString = Startup.GetConnectionString();
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            SqlCommand? command;
            switch (tableId)
            {
                case TableID.Firmy:
                    command = new SqlCommand("SELECT id_firmy FROM Firmy", _connection);
                    break;
                case TableID.Zamestnanci:
                    command = new SqlCommand("SELECT id_zamestnanca FROM Zamestnanci", _connection);
                    break;
                case TableID.Divizie:
                    command = new SqlCommand("SELECT id_divizie FROM Divizie", _connection);
                    break;
                case TableID.Projekty:
                    command = new SqlCommand("SELECT id_projektu FROM Projekty", _connection);
                    break;
                case TableID.Oddelenia:
                    command = new SqlCommand("SELECT id_oddelenia FROM Oddelenia", _connection);
                    break;
                default:
                    return false;
            }
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (reader.GetInt32(0) == id)
                {
                    return true;
                }
            }
            return false;
        }

        public static List<Object?> replaceDBNullsInReaderRow(SqlDataReader reader) {
            List<Object?> list = new List<Object?>();
            if (reader.HasRows == false)
            {
                Console.WriteLine("DBNULL replace reader has no rows.");
                return list;
            }
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.IsDBNull(i))
                {
                    list.Add(null);
                }
                else {
                    list.Add(reader[i]);
                }
            }
            return list;
        }
    }
}
