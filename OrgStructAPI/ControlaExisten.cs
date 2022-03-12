using System.Data.SqlClient;

namespace OrgStructAPI
{
    public static class ControlaExisten
    {
        /* 
         * TableId:
         * Firmy        - 1
         * Zamestnanci  - 2
         * Divizie      - 3
         * Projekty     - 4
         * Oddelenia    - 5
         * id: Id objektu ktory hladame.
         */
        public static bool ExistujePodlaID(int? tableId, int id)
        {
            SqlConnection _connection = new SqlConnection();
            string _connectionString = Startup.GetConnectionString();
            _connection.ConnectionString = _connectionString;
            _connection.Open();
            SqlCommand? command;
            Console.WriteLine("Pred switchom.");
            switch (tableId)
            {
                case 1:
                    command = new SqlCommand("SELECT id_firmy FROM Firmy", _connection);
                    break;
                case 2:
                    command = new SqlCommand("SELECT id_zamestnanca FROM Zamestnanci", _connection);
                    break;
                case 3:
                    command = new SqlCommand("SELECT id_divizie FROM Divizie", _connection);
                    break;
                case 4:
                    command = new SqlCommand("SELECT id_projektu FROM Projekty", _connection);
                    break;
                case 5:
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
    }
}
